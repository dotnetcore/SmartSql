using SmartSql.Exceptions;
using SmartSql.Reflection;
using SmartSql.Reflection.TypeConstants;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using SmartSql.Reflection.Convert;

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

        public SqlParameter this[string key] { get => _sqlParameters[key]; set => _sqlParameters[key] = value; }

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

        public static SqlParameterCollection Create<TRequest>(RequestContext<TRequest> requestContext)
            where TRequest : class
        {
            var sourceRequest = requestContext.Request;
            bool ignoreParameterCase = requestContext.ExecutionContext.SmartSqlConfig.Settings.IgnoreParameterCase;
            if (TryCreate<object>(sourceRequest, ignoreParameterCase, out var sqlParameters))
            {
                return sqlParameters;
            }

            if (typeof(TRequest) == CommonType.Object)
            {
                return RequestConvert.Instance.ToSqlParameters(sourceRequest, ignoreParameterCase);
            }
            return ignoreParameterCase ? RequestConvertCache<TRequest, IgnoreCaseType>.Convert(sourceRequest)
                : RequestConvertCache<TRequest>.Convert(sourceRequest);
        }


        private static bool TryCreate<TRequest>(TRequest sourceRequest, bool ignoreParameterCase, out SqlParameterCollection sqlParameters)
            where TRequest : class
        {
            switch (sourceRequest)
            {
                case null:
                    {
                        sqlParameters = new SqlParameterCollection(ignoreParameterCase);
                        return true;
                    }
                case SqlParameterCollection sourceRequestParams:
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
        public void Add(SqlParameter sqlParameter)
        {
            _sqlParameters.Add(sqlParameter.Name, sqlParameter);
            sqlParameter.OnSetSourceParameter = (sqlParam) =>
            {
                if (!DbParameters.ContainsKey(sqlParam.SourceParameter.ParameterName))
                {
                    DbParameters.Add(sqlParam.SourceParameter.ParameterName, sqlParam.SourceParameter);
                }
            };
        }
        public void Add(string key, SqlParameter value)
        {
            Add(value);
        }
        public void Add(KeyValuePair<string, SqlParameter> item)
        {
            Add(item.Value);
        }
        public bool TryAdd(string propertyName, object paramVal)
        {
            return TryAdd(new SqlParameter(propertyName, paramVal,
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
        public bool TryGetValue(string key, out SqlParameter value) => _sqlParameters.TryGetValue(key, out value);

        public bool TryGetParameterValue<T>(string propertyName, out T paramVal)
        {
            if (TryGetValue(propertyName, out var sqlParameter))
            {
                paramVal = (T)(
                    sqlParameter.SourceParameter == null
                    ? sqlParameter.Value : sqlParameter.SourceParameter.Value
                    );
                return true;
            }
            paramVal = default;
            return false;
        }

        #endregion
        #region Contains
        public bool ContainsKey(string key) => _sqlParameters.ContainsKey(key);
        public bool Contains(KeyValuePair<string, SqlParameter> item) => _sqlParameters.Contains(item);

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
