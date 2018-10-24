using SmartSql.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;
using SmartSql.Utils;
using SmartSql.Utils.PropertyAccessor;
using SmartSql.Utils.PropertyAccessor.Impl;
using System.Linq;

namespace SmartSql
{
    public class DbParameterCollection
    {
        private readonly IGetAccessorFactory _getAccessorFactory = GetAccessorFactory.Instance;
        public bool IgnoreParameterCase { get; }
        public object RequestParams { get; }
        /// <summary>
        /// Request参数属性名
        /// </summary>
        public ICollection<string> ParameterNames { get { return _dbParameters.Keys; } }
        IDictionary<string, DbParameter> _dbParameters;
        public DbParameterCollection() : this(false)
        {

        }
        public DbParameterCollection(bool ignoreParameterCase) : this(false, null)
        {

        }
        public DbParameterCollection(bool ignoreParameterCase, object reqParams)
        {
            IgnoreParameterCase = ignoreParameterCase;
            RequestParams = reqParams;
            var paramComparer = IgnoreParameterCase ? StringComparer.CurrentCultureIgnoreCase : StringComparer.CurrentCulture;
            if (reqParams == null)
            {
                _dbParameters = new Dictionary<string, DbParameter>(paramComparer);
                return;
            }
            if (reqParams is DbParameterCollection dbParameterCollection)
            {
                _dbParameters = new Dictionary<string, DbParameter>(dbParameterCollection._dbParameters, paramComparer);
                return;
            }
            if (reqParams is IEnumerable<KeyValuePair<string, object>> reqKVs)
            {
                _dbParameters = new Dictionary<string, DbParameter>(paramComparer);
                foreach (var kv in reqKVs)
                {
                    Add(kv.Key, kv.Value);
                }
                return;
            }
            if (reqParams is IEnumerable<KeyValuePair<string, DbParameter>> reqDbKVs)
            {
                _dbParameters = new Dictionary<string, DbParameter>(paramComparer);
                foreach (var kv in reqDbKVs)
                {
                    Add(kv.Value);
                }
                return;
            }
            _dbParameters = ObjectUtils.ToDicDbParameters(reqParams, ignoreParameterCase);
        }

        public DbParameter Get(string propertyName)
        {
            if (_dbParameters.TryGetValue(propertyName, out DbParameter dbParameter))
            {
                return dbParameter;
            }
            if (propertyName.IndexOf('.') > -1)
            {
                var paramVal = GetOrAddNestedObject(propertyName, out dbParameter);
            }
            return dbParameter;
        }
        public object GetValue(string propertyName)
        {
            var dbParameter = Get(propertyName);
            if (dbParameter == null)
            {
                return null;
            }
            return dbParameter.SourceParameter == null ? dbParameter.Value : dbParameter.SourceParameter.Value;
        }

        public T GetValue<T>(string propertyName)
        {
            var val = GetValue(propertyName);
            if (val == DBNull.Value)
            {
                if (default(T) != null)
                {
                    throw new SmartSqlException($"DBNull can not convert to {typeof(T)}");
                }
                return default(T);
            }
            return (T)val;
        }

        public void Add(string propertyName, object val)
        {
            Add(new DbParameter
            {
                Name = propertyName,
                Value = val
            });
        }

        public void Add(DbParameter dbParameter)
        {
            if (Contains(dbParameter.Name))
            {
                throw new SmartSqlException($"The parameter name:{dbParameter.Name} has already existed!");
            }
            _dbParameters.Add(dbParameter.Name, dbParameter);
        }
        private IDictionary<string, bool> _cachedNoFindProperty = new Dictionary<string, bool>();
        public bool Contains(string propertyName)
        {
            if (_dbParameters.ContainsKey(propertyName))
            {
                return true;
            }
            if (propertyName.IndexOf('.') > -1)
            {
                var paramVal = GetOrAddNestedObject(propertyName, out DbParameter dbParameter);
                return paramVal.Status == PropertyValue.GetStatus.Ok;
            }
            return false;
        }

        private PropertyValue GetOrAddNestedObject(string propertyName, out DbParameter dbParameter)
        {
            dbParameter = default(DbParameter);
            if (_cachedNoFindProperty.ContainsKey(propertyName))
            {
                return PropertyValue.NotFindProperty;
            }

            var reqParamNames = propertyName.Split('.');
            var reqRootParamName = reqParamNames[0];
            if (!_dbParameters.ContainsKey(reqRootParamName))
            {
                _cachedNoFindProperty.Add(propertyName, true);
                return PropertyValue.NotFindProperty;
            }
            var reqRootParam = _dbParameters[reqRootParamName];
            var nestedParamName = propertyName.Substring(reqRootParamName.Length + 1);
            PropertyValue paramVal = GetAccessorUtil.GetValue(reqRootParam.Value, nestedParamName, IgnoreParameterCase);
            if (paramVal.Status != PropertyValue.GetStatus.Ok)
            {
                _cachedNoFindProperty.Add(propertyName, true);
                return PropertyValue.NotFindProperty;
            }
            dbParameter = new DbParameter { Name = propertyName, Value = paramVal.Value };
            _dbParameters.Add(dbParameter.Name, dbParameter);
            return paramVal;
        }
    }
}
