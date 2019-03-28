using System;
using System.Collections.Generic;
using SmartSql.Reflection.ObjectFactoryBuilder;

namespace SmartSql.TypeHandlers
{
    public class TypeHandlerBuilder : ITypeHandlerBuilder
    {
        public ITypeHandler Build(Type genericTypeHandlerType, Type propertyType, Type fieldType, IDictionary<string, object> parameters)
        {
            var typeHandlerType = genericTypeHandlerType.MakeGenericType(propertyType, fieldType);
            return Build(typeHandlerType, parameters);
        }

        public ITypeHandler Build(Type genericTypeHandlerType, Type propertyType, IDictionary<string, object> parameters)
        {
            var typeHandlerType = genericTypeHandlerType.MakeGenericType(propertyType);
            return Build(typeHandlerType, parameters);
        }

        public ITypeHandler Build(Type typeHandlerType, IDictionary<string, object> parameters)
        {
            var typeHandlerObj = ObjectFactoryBuilderManager.Expression.GetObjectFactory(typeHandlerType, Type.EmptyTypes)(null) as ITypeHandler;
            typeHandlerObj.Initialize(parameters);
            return typeHandlerObj;
        }
    }
}
