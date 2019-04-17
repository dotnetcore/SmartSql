using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace SmartSql.Reflection.TypeConstants
{
    public static class TypeType
    {
        public static readonly Type Type = typeof(Type);
        public static class Method
        {
            public static readonly MethodInfo GetTypeFromHandle = TypeType.Type.GetMethod(nameof(Type.GetTypeFromHandle));
        }
    }
}
