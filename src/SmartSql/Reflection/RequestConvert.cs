using SmartSql.Data;
using SmartSql.Reflection.TypeConstants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using SmartSql.Utils;

namespace SmartSql.Reflection
{
    public class RequestConvert
    {
        public static RequestConvert Instance = new RequestConvert();

        public SqlParameterCollection ToSqlParameters(object sourceObj, bool ignoreNameCase)
        {
            return GetToSqlParametersFunc(sourceObj.GetType(), ignoreNameCase)(sourceObj);
        }

        public Func<object, SqlParameterCollection> GetToSqlParametersFunc(Type sourceType, bool ignoreCase)
        {
            if (ignoreCase)
            {
                return CacheUtil<TypeWrapper<RequestContext, IgnoreCaseType>, Type, Func<object, SqlParameterCollection>>
                     .GetOrAdd(sourceType, _ => RequestConvertCacheType.GetConvert(_, ignoreCase));
            }
            return CacheUtil<RequestContext, Type, Func<object, SqlParameterCollection>>
                .GetOrAdd(sourceType, _ => RequestConvertCacheType.GetConvert(_, ignoreCase)); ;
        }
    }
}
