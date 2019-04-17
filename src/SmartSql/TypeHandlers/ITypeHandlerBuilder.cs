using System;
using System.Collections.Generic;

namespace SmartSql.TypeHandlers
{
    public interface ITypeHandlerBuilder
    {
        ITypeHandler Build(Type genericTypeHandlerType, Type propertyType,Type fieldType, IDictionary<string, object> parameters);
        ITypeHandler Build(Type genericTypeHandlerType, Type genericType, IDictionary<string, object> parameters);
        ITypeHandler Build(Type typeHandlerType, IDictionary<string, object> parameters);
    }
}