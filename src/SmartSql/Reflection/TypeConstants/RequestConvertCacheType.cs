using SmartSql.Data;
using SmartSql.Reflection.Convert;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace SmartSql.Reflection.TypeConstants
{
    public static class RequestConvertCacheType
    {
        public static readonly Type GenericType = typeof(RequestConvertCache<,>);
        public static Type MakeGenericType(Type requestType, bool ignoreCase)
        {
            if (ignoreCase)
            {
                return GenericType.MakeGenericType(requestType, typeof(IgnoreCaseType));
            }
            return GenericType.MakeGenericType(requestType, typeof(object));
        }

        public static MethodInfo GetConvertMethod(Type requestType, bool ignoreCase)
        {
            return MakeGenericType(requestType, ignoreCase).
                GetMethod("get_Convert", BindingFlags.Static | BindingFlags.NonPublic);
        }
        public static Func<object, SqlParameterCollection> GetConvert(Type requestType, bool ignoreCase)
        {
            return MakeGenericType(requestType, ignoreCase)
                .GetProperty("Convert").GetValue(null, null) as Func<object, SqlParameterCollection>;
        }
    }
}
