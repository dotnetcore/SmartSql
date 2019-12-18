using SmartSql.Exceptions;
using SmartSql.Reflection;
using SmartSql.Reflection.TypeConstants;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using SmartSql.Reflection.Convert;
using SmartSql.Reflection.PropertyAccessor;

namespace SmartSql.Data
{
    public class SqlParameterCollection : ISqlParameterCollection
    {
        private readonly IDictionary<string, SqlParameter> _sqlParameters;
        public bool IgnoreCase { get; }
        public IDictionary<string, DbParameter> DbParameters { get; private set; }
        public ICollection<string> Keys => _sqlParameters.Keys;
        public ICollection<SqlParameter> Values => _sqlParameters.Values;
        public int Count => _sqlParameters.Count;
        public bool IsReadOnly => _sqlParameters.IsReadOnly;

        private readonly IGetAccessorFactory _getAccessorFactory = EmitGetAccessorFactory.Instance;

        public SqlParameter this[string key]
        {
            get => _sqlParameters[key];
            set => _sqlParameters[key] = value;
        }

        public SqlParameterCollection() : this(false)
        {
        }

        public SqlParameterCollection(bool ignoreCase)
        {
            IgnoreCase = ignoreCase;
            var paramComparer = IgnoreCase ? StringComparer.CurrentCultureIgnoreCase : StringComparer.CurrentCulture;
            _sqlParameters = new Dictionary<string, SqlParameter>(paramComparer);
            DbParameters = new Dictionary<String, DbParameter>(paramComparer);
        }

        public static ISqlParameterCollection Create<TRequest>(TRequest sourceRequest,bool ignoreParameterCase)
            where TRequest : class
        {
            if (TryCreate<object>(sourceRequest, ignoreParameterCase, out var sqlParameters))
            {
                return sqlParameters;
            }

            if (typeof(TRequest) == CommonType.Object)
            {
                return RequestConvert.Instance.ToSqlParameters(sourceRequest, ignoreParameterCase);
            }

            return ignoreParameterCase
                ? RequestConvertCache<TRequest, IgnoreCaseType>.Convert(sourceRequest)
                : RequestConvertCache<TRequest>.Convert(sourceRequest);
        }


        private static bool TryCreate<TRequest>(TRequest sourceRequest, bool ignoreParameterCase,
            out ISqlParameterCollection sqlParameters)
            where TRequest : class
        {
            switch (sourceRequest)
            {
                case null:
                {
                    sqlParameters = new SqlParameterCollection(ignoreParameterCase);
                    return true;
                }

                case ISqlParameterCollection sourceRequestParams:
                {
                    sqlParameters = sourceRequestParams;
                    return true;
                }

                case IEnumerable<KeyValuePair<string, object>> reqKVs:
                {
                    sqlParameters = new SqlParameterCollection(ignoreParameterCase);
                    foreach (var kv in reqKVs)
                    {
                        sqlParameters.TryAdd(kv.Key, kv.Value);
                    }

                    return true;
                }

                case IEnumerable<KeyValuePair<string, SqlParameter>> reqDbKVs:
                {
                    sqlParameters = new SqlParameterCollection(ignoreParameterCase);
                    foreach (var kv in reqDbKVs)
                    {
                        sqlParameters.TryAdd(kv.Key, kv.Value);
                    }

                    return true;
                }

                default:
                {
                    sqlParameters = null;
                    return false;
                }
            }
        }

        #region Add

        public void Add(string key, SqlParameter sqlParameter)
        {
            _sqlParameters.Add(key, sqlParameter);
            sqlParameter.OnSetSourceParameter = (sqlParam) =>
            {
                if (!DbParameters.ContainsKey(sqlParam.SourceParameter.ParameterName))
                {
                    DbParameters.Add(sqlParam.SourceParameter.ParameterName, sqlParam.SourceParameter);
                }
            };
        }

        public bool TryAdd(string key, SqlParameter sqlParameter)
        {
            if (ContainsKey(sqlParameter.Name))
            {
                return false;
            }

            Add(key, sqlParameter);
            return true;
        }

        public void Add(SqlParameter sqlParameter)
        {
            Add(sqlParameter.Name, sqlParameter);
        }


        public void Add(KeyValuePair<string, SqlParameter> item)
        {
            Add(item.Key, item.Value);
        }


        public bool TryAdd(string propertyName, object paramVal)
        {
            return TryAdd(propertyName, new SqlParameter(propertyName, paramVal,
                paramVal == null ? CommonType.Object : paramVal.GetType()));
        }

        public bool TryAdd(SqlParameter sqlParameter)
        {
            if (ContainsKey(sqlParameter.Name))
            {
                return false;
            }

            Add(sqlParameter);
            return true;
        }

        #endregion

        #region Get

        public bool TryGetValue(string key, out SqlParameter value)
        {
            if (_sqlParameters.TryGetValue(key, out value))
            {
                return true;
            }

            return TryCreateParameter(key, out value);
        }

        private bool TryCreateParameter(string key, out SqlParameter value)
        {
            value = default;
            PropertyTokenizer propertyTokenizer = new PropertyTokenizer(key);
            if (propertyTokenizer.FullName == propertyTokenizer.Name)
            {
                return false;
            }

            if (!_sqlParameters.TryGetValue(propertyTokenizer.Name, out var root))
            {
                return false;
            }

            object paramVal = null;
            if (!propertyTokenizer.MoveNext())
            {
                if (propertyTokenizer.Mode != AccessMode.IndexerGet)
                {
                    return false;
                }

                if (root.ParameterType.IsArray)
                {
                    int.TryParse(propertyTokenizer.Index, out var idx);
                    paramVal = ((Array) root.Value).GetValue(idx);
                }

                if (CommonType.Dictionary.IsAssignableFrom(root.ParameterType))
                {
                    var rootDic = ((IDictionary) root.Value);
                    if (!rootDic.Contains(propertyTokenizer.Index))
                    {
                        return false;
                    }
                    paramVal = rootDic[propertyTokenizer.Index];
                }
            }
            else
            {
                if (!_getAccessorFactory.TryCreate(root.ParameterType, propertyTokenizer.Current,
                    out var getMethodImpl))
                {
                    return false;
                }

                paramVal = getMethodImpl(root.Value);
            }


            var parameterName = key.Replace(".", "_").Replace("[", "_Idx_").Replace("]", "");
            value = new SqlParameter(parameterName, paramVal,
                paramVal == null ? CommonType.Object : paramVal.GetType());
            Add(key, value);
            return true;
        }

        public bool TryGetParameterValue<T>(string propertyName, out T paramVal)
        {
            if (TryGetValue(propertyName, out var sqlParameter))
            {
                paramVal = (T) (
                    sqlParameter.SourceParameter == null
                        ? sqlParameter.Value
                        : sqlParameter.SourceParameter.Value
                );
                return true;
            }

            paramVal = default;
            return false;
        }

        #endregion

        #region Contains

        /// <summary>
        /// 不再进行嵌套类检查
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool ContainsKey(string key)
        {
            return _sqlParameters.ContainsKey(key);
        }

        public bool Contains(KeyValuePair<string, SqlParameter> item)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Remove

        public bool Remove(string key)
        {
            DbParameters.Remove(key);
            return _sqlParameters.Remove(key);
        }

        public bool Remove(KeyValuePair<string, SqlParameter> item)
        {
            DbParameters.Remove(item.Key);
            return _sqlParameters.Remove(item);
        }

        #endregion

        public void Clear()
        {
            DbParameters.Clear();
            _sqlParameters.Clear();
        }

        public void CopyTo(KeyValuePair<string, SqlParameter>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<KeyValuePair<string, SqlParameter>> GetEnumerator()
        {
            return _sqlParameters.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _sqlParameters.GetEnumerator();
        }
    }
}