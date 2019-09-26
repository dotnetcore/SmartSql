using System;
using System.Reflection;

namespace SmartSql.Reflection
{
    public class PropertyHolder
    {
        public PropertyInfo Property { get; set; }
        public String TypeHandler { get; set; }

        public Type PropertyType => Property.PropertyType;

        public bool CanWrite => Property.CanWrite;
        
        public MethodInfo SetMethod => Property.SetMethod;
    }
}