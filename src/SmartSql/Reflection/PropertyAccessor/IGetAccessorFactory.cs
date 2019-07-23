using System;
using System.Reflection;

namespace SmartSql.Reflection.PropertyAccessor
{
    public interface IGetAccessorFactory
    {
        bool TryCreate(Type targetType, PropertyTokenizer propertyTokenizer, out Func<object, object> getMethodImpl);
        bool TryCreate(Type targetType, string fullName, out Func<object, object> getMethodImpl);
        Func<object, object> Create(PropertyInfo propertyInfo);
    }
}