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

        public ISqlParameterCollection ToSqlParameters(object sourceObj, bool ignoreNameCase)
        {
            return GetToSqlParametersFunc(sourceObj.GetType(), ignoreNameCase)(sourceObj);
        }

        public Func<object, ISqlParameterCollection> GetToSqlParametersFunc(Type sourceType, bool ignoreCase)
        {
            if (ignoreCase)
            {
                return CacheUtil<TypeWrapper<RequestContext, IgnoreCaseType>, Type, Func<object, ISqlParameterCollection>>
                     .GetOrAdd(sourceType, _ => RequestConvertCacheType.GetConvert(_, ignoreCase));
            }
            return CacheUtil<RequestContext, Type, Func<object, ISqlParameterCollection>>
                .GetOrAdd(sourceType, _ => RequestConvertCacheType.GetConvert(_, ignoreCase)); ;
        }
    }
}
