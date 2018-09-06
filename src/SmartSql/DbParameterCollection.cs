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

        public DbParameter Get(string paramName)
        {
            if (_dbParameters.TryGetValue(paramName, out DbParameter dbParameter))
            {
                return dbParameter;
            }
            if (paramName.IndexOf('.') > -1)
            {
                var paramVal = GetAccessorUtil.GetValue(RequestParams, paramName, IgnoreParameterCase);
                dbParameter = new DbParameter { Name = paramName, Value = paramVal.Value };
                Add(dbParameter);
            }
            return dbParameter;
        }
        public object GetValue(string paramName)
        {
            var dbParameter = Get(paramName);
            if (dbParameter == null)
            {
                return null;
            }
            return dbParameter.SourceParameter == null ? dbParameter.Value : dbParameter.SourceParameter.Value;
        }

        public T GetValue<T>(string paramName)
        {
            var val = GetValue(paramName);
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

        public void Add(string paramName, object val)
        {
            Add(new DbParameter
            {
                Name = paramName,
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

        public bool Contains(string paramName)
        {
            return _dbParameters.ContainsKey(paramName);
        }
    }
}
