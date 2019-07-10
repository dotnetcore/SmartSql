using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace SmartSql.Reflection.PropertyAccessor
{
    public interface ISetAccessorFactory
    {
        Action<object, object> Create(Type targetType, string propertyName);
        Action<object, object> Create(PropertyInfo propertyInfo);
    }
}
