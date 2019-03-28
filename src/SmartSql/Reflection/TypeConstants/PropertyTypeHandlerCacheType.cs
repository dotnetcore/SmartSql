using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using SmartSql.TypeHandlers;

namespace SmartSql.Reflection.TypeConstants
{
    public static class PropertyTypeHandlerCacheType
    {
        public static readonly Type GenericPropertyType = typeof(PropertyTypeHandlerCache<>);
        public static Type MakeGenericType(Type propertyType)
        {
            return GenericPropertyType.MakeGenericType(propertyType);
        }
        public static MethodInfo GetSetHandlerMethod(Type propertyType)
        {
            return MakeGenericType(propertyType).
                GetMethod("SetHandler", BindingFlags.Static | BindingFlags.NonPublic);
        }
        public static void SetHandler(ITypeHandler typeHandler)
        {
            GetSetHandlerMethod(typeHandler.PropertyType)
                .Invoke(null, new object[] { typeHandler });
        }
        public static ITypeHandler GetHandler(Type propertyType)
        {
            return GetHandlerMethod(propertyType)
                .Invoke(null, null) as ITypeHandler;
        }
        public static MethodInfo GetHandlerMethod(Type propertyType)
        {
            return MakeGenericType(propertyType).GetMethod("get_Handler");
        }
    }
}
