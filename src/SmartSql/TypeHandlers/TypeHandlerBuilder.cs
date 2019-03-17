using SmartSql.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;
using SmartSql.Reflection;
using SmartSql.Reflection.ObjectFactoryBuilder;
namespace SmartSql.TypeHandlers
{
    public class TypeHandlerBuilder : ITypeHandlerBuilder
    {
        public ITypeHandler Build(Type genericTypeHandlerType, Type genericType, IDictionary<string, object> parameters)
        {
            var typeHandlerType = genericTypeHandlerType.MakeGenericType(genericType);
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
