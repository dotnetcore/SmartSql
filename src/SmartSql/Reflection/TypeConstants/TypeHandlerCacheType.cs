using SmartSql.TypeHandlers;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace SmartSql.Reflection.TypeConstants
{
    public static class TypeHandlerCacheType
    {
        public static readonly Type GenericType = typeof(TypeHandlerCache<>);
        public static Type MakeGenericType(Type mappedType)
        {
            return GenericType.MakeGenericType(mappedType);
        }
        public static MethodInfo GetSetHandlerMethod(Type mappedType)
        {
            return MakeGenericType(mappedType).
                GetMethod("SetHandler", BindingFlags.Static | BindingFlags.NonPublic);
        }
        public static MethodInfo GetGetValueMethod(Type mappedType)
        {
            return MakeGenericType(mappedType).
                GetMethod("GetValue");
        }
        public static MethodInfo GetGetObjectValueMethod(Type mappedType)
        {
            return MakeGenericType(mappedType).
                GetMethod("GetObjectValue");
        }
        public static void SetHandler(ITypeHandler typeHandler)
        {
            GetSetHandlerMethod(typeHandler.MappedType).Invoke(null, new object[] { typeHandler });
        }
        public static MethodInfo GetHandlerMethod(Type mappedType)
        {
            return MakeGenericType(mappedType).GetMethod("get_Handler");
        }
    }
}
