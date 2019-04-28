using SmartSql.TypeHandlers;
using System;
using System.Reflection;

namespace SmartSql.Reflection.TypeConstants
{
    public static class TypeHandlerCacheType
    {
        public static readonly Type GenericType = typeof(TypeHandlerCache<,>);

        public static Type MakeGenericType(Type propertyType, Type fieldType)
        {
            return GenericType.MakeGenericType(propertyType, fieldType);
        }
        public static MethodInfo GetSetHandlerMethod(Type propertyType, Type fieldType)
        {
            return MakeGenericType(propertyType, fieldType).
                GetMethod("SetHandler", BindingFlags.Static | BindingFlags.NonPublic);
        }
        public static MethodInfo GetGetValueMethod(Type propertyType, Type fieldType)
        {
            return MakeGenericType(propertyType, fieldType).
                GetMethod("GetValue");
        }
        public static void SetHandler(ITypeHandler typeHandler)
        {
            GetSetHandlerMethod(typeHandler.PropertyType, typeHandler.FieldType)
                .Invoke(null, new object[] { typeHandler });
        }
        public static ITypeHandler GetHandler(Type propertyType, Type fieldType)
        {
            return GetHandlerMethod(propertyType, fieldType)
                .Invoke(null, null) as ITypeHandler;
        }
        public static MethodInfo GetHandlerMethod(Type propertyType, Type fieldType)
        {
            return MakeGenericType(propertyType, fieldType)
                .GetMethod("get_Handler");
        }
    }
}
