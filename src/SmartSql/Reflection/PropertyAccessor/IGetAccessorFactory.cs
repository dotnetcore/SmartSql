using System;
using System.Reflection;

namespace SmartSql.Reflection.PropertyAccessor
{
    public interface IGetAccessorFactory
    {
        Func<object, object> Create(Type targetType, string propertyName);
        Func<object, object> Create(PropertyInfo propertyInfo);
    }
}