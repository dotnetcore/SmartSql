using SmartSql.TypeHandlers;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace SmartSql.Reflection.TypeConstants
{
    public static class TypeHandlerFactoryType
    {
        public static readonly Type Type = typeof(ITypeHandlerFactory);
        public static class Method
        {
            public static readonly MethodInfo Get = Type.GetMethod(nameof(ITypeHandlerFactory.Get), new Type[] { TypeType.Type });
        }
    }
}
