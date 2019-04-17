using SmartSql.Data;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace SmartSql.Reflection.TypeConstants
{
    public static class SqlParameterType
    {
        public static readonly Type SqlParameter = typeof(SqlParameter);
        public static readonly Type SqlParameterCollection = typeof(SqlParameterCollection);

        public static class Ctor
        {
            public static readonly ConstructorInfo SqlParameter = SqlParameterType.SqlParameter.GetConstructor(new Type[] { CommonType.String, CommonType.Object, TypeType.Type });
            public static readonly ConstructorInfo SqlParameterCollection = SqlParameterType.SqlParameterCollection.GetConstructor(new Type[] { CommonType.Boolean });
        }
        public static class Method
        {
            /// <summary>
            /// Add By SqlParameter
            /// </summary>
            public static readonly MethodInfo Add = SqlParameterCollection.GetMethod("Add", new Type[] { SqlParameterType.SqlParameter });
            public static readonly MethodInfo SetTypeHandler = SqlParameter.GetMethod("set_TypeHandler");
        }
    }
}
