using System;
using System.Collections.Generic;
using System.Text;

namespace SmartSql.Utils.PropertyAccessor
{
    public class PropertyValue
    {
        public static PropertyValue NullTarget = new PropertyValue { Status = GetStatus.NullTarget };
        public static PropertyValue NotFindProperty = new PropertyValue { Status = GetStatus.NotFindProperty };
        public PropertyValue() { }
        public PropertyValue(object val)
        {
            Value = val;
        }
        public GetStatus Status { get; set; } = GetStatus.Ok;
        public object Value { get; set; }

        public enum GetStatus
        {
            Ok = 1,
            NullTarget = 1 << 1,
            NotFindProperty = 1 << 2
        }
    }
    public interface IGetAccessorFactory
    {
        Func<object, PropertyValue> CreateGet(Type targetType, string propertyName, bool ignorePropertyCase);
    }
}
