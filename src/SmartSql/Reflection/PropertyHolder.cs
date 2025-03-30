using System;
using System.Collections.Generic;
using System.Reflection;

namespace SmartSql.Reflection
{
    public class PropertyHolder : IPropertyHolder
    {
        public PropertyInfo Property { get; set; }
        public String TypeHandler { get; set; }

        public Type PropertyType => Property.PropertyType;

        public bool CanWrite => Property.CanWrite;
        
        public MethodInfo SetMethod => Property.SetMethod;

        public bool IsChain => false;

        public IReadOnlyList<PropertyInfo> PropertyChain => throw new NotSupportedException();

    }
}