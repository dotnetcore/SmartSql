
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SmartSql.Reflection
{
    public class PropertyChainHolder : IPropertyHolder
    {
        private readonly PropertyInfo property;

        public PropertyInfo Property { get => property; }

        public string TypeHandler { get; set; }

        public Type PropertyType => Property.PropertyType;

        public bool CanWrite { get; }

        public MethodInfo SetMethod => Property.SetMethod;

        public bool IsChain => true;

        public IReadOnlyList<PropertyInfo> PropertyChain { get; }

        public PropertyChainHolder(List<PropertyInfo> propertyChain, string typeHandler)
        {
            property = propertyChain.Last();

            PropertyChain = propertyChain.AsReadOnly();

            CanWrite = PropertyChain.All(property => property.CanWrite);

            TypeHandler = typeHandler;
        }
    }
}