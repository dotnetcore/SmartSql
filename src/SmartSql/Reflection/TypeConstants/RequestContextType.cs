using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace SmartSql.Reflection.TypeConstants
{
    public static class RequestContextType
    {
        public static readonly Type Type = typeof(RequestContext);
        public static readonly ConstructorInfo Ctor= Type.GetConstructor(Type.EmptyTypes);

        public static class Method
        {
            public static readonly MethodInfo SetDataSourceChoice = Type.GetMethod("set_DataSourceChoice");
            public static readonly MethodInfo SetCommandType = Type.GetMethod("set_CommandType");
            public static readonly MethodInfo SetScope = Type.GetMethod("set_Scope");
            public static readonly MethodInfo SetSqlId = Type.GetMethod("set_SqlId");
            public static readonly MethodInfo SetRequest = Type.GetMethod("set_Request");
            public static readonly MethodInfo SetRealSql = Type.GetMethod("set_RealSql");
        }
    }
}
