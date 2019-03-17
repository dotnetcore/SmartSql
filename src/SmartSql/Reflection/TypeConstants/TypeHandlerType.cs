using SmartSql.TypeHandlers;
using System;
using System.Reflection;

namespace SmartSql.Reflection.TypeConstants
{
    public static class TypeHandlerType
    {
        public readonly static Type Type = typeof(ITypeHandler);
        public static class Method
        {
            public readonly static MethodInfo GetObjectValue = Type.GetMethod("GetObjectValue", new Type[] { DataType.DataReaderWrapper, CommonType.Int32 });
        }
    }
}
