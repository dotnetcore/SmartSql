using SmartSql.TypeHandlers;
using System;
using System.Reflection;

namespace SmartSql.Reflection.TypeConstants
{
    public static class TypeHandlerType
    {
        public static readonly Type Type = typeof(ITypeHandler);
        public static class Method
        {
            public static readonly MethodInfo GetObjectValue = Type.GetMethod("GetObjectValue", new Type[] { DataType.DataReaderWrapper, CommonType.Int32 });
        }
    }
}
