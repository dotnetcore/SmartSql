using SmartSql.Data;
using SmartSql.Reflection.TypeConstants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace SmartSql.Reflection
{
    public class RequestConvert
    {
        public static RequestConvert Instance = new RequestConvert();

        private readonly Dictionary<string, Func<object, SqlParameterCollection>> _cachedConvert = new Dictionary<string, Func<object, SqlParameterCollection>>();
        private String GenerateCacheKey(Type sourceType, bool ignoreNameCase)
        {
            return $"{sourceType.FullName}_{ignoreNameCase}";
        }
        public SqlParameterCollection ToSqlParameters(object sourceObj, bool ignoreNameCase)
        {
            return GetToSqlParametersFunc(sourceObj.GetType(), ignoreNameCase)(sourceObj);
        }

        public Func<object, SqlParameterCollection> GetToSqlParametersFunc(Type sourceType, bool ignoreNameCase)
        {
            string key = GenerateCacheKey(sourceType, ignoreNameCase);
            if (!_cachedConvert.ContainsKey(key))
            {
                lock (this)
                {
                    if (!_cachedConvert.ContainsKey(key))
                    {
                        var impl = RequestConvertCacheType.GetConvert(sourceType, ignoreNameCase);
                        _cachedConvert.Add(key, impl);
                    }
                }
            }
            return _cachedConvert[key];
        }
    }
}
