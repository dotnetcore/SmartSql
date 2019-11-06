using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace SmartSql.Reflection.TypeConstants
{
    public static class RequestContextType
    {
        public static readonly Type Type = typeof(RequestContext);
        public static readonly Type AbstractType = typeof(AbstractRequestContext);
        public static readonly Type GenericType = typeof(RequestContext<>);
        public static readonly ConstructorInfo Ctor= Type.GetConstructor(Type.EmptyTypes);

        public static Type MakeGenericType(Type requestType)
        {
            return GenericType.MakeGenericType(requestType);
        }
        public static ConstructorInfo MakeGenericTypeCtor(Type requestType)
        {
            return MakeGenericType(requestType).GetConstructor(Type.EmptyTypes);
        }

        public static class Method
        {
            public static readonly MethodInfo SetDataSourceChoice = AbstractType.GetMethod("set_DataSourceChoice");
            public static readonly MethodInfo SetCommandType = AbstractType.GetMethod("set_CommandType");
            public static readonly MethodInfo SetTransaction = AbstractType.GetMethod("set_Transaction");
            public static readonly MethodInfo SetScope = AbstractType.GetMethod("set_Scope");
            public static readonly MethodInfo SetSqlId = AbstractType.GetMethod("set_SqlId");
            public static readonly MethodInfo SetRequest = AbstractType.GetMethod("SetRequest");
            public static readonly MethodInfo SetRealSql = AbstractType.GetMethod("set_RealSql");
            public static readonly MethodInfo SetCacheKeyTemplate = AbstractType.GetMethod("set_CacheKeyTemplate");
            public static readonly MethodInfo SetCacheId = AbstractType.GetMethod("set_CacheId");
            public static readonly MethodInfo SetReadDb = AbstractType.GetMethod("set_ReadDb");
            public static readonly MethodInfo SetCommandTimeout = AbstractType.GetMethod("set_CommandTimeout");
            public static readonly MethodInfo SetEnablePropertyChangedTrack = AbstractType.GetMethod("set_EnablePropertyChangedTrack");
            
        }
    }
}
