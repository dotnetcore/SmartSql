
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SmartSql.Reflection
{
    /// <summary>
    /// Represents a chain of nested properties (e.g. User.Address.City)
    /// 表示嵌套属性链（例如 User.Address.City）
    /// </summary>
    public class PropertyChainHolder : IPropertyHolder
    {
        private readonly PropertyInfo property;

        /// <summary>
        /// Gets the final property in the chain (获取属性链的最后一个属性)
        /// Note: Setter is explicitly disabled for immutability (Setter 被显式禁用以保证不可变性)
        /// </summary>
        public PropertyInfo Property { get => property; set => throw new NotSupportedException(); }

        /// <summary>
        /// Type handler for property value conversion (属性值类型转换处理器)
        /// </summary>
        public string TypeHandler { get; set; }

        /// <summary>
        /// Property type of the final property (最终属性的类型)
        /// </summary>
        public Type PropertyType => Property.PropertyType;

        /// <summary>
        /// Indicates if all properties in the chain are writable (标识属性链中所有属性是否可写)
        /// Pre-calculated during initialization for performance (初始化时预计算以优化性能)
        /// </summary>
        public bool CanWrite { get; }

        /// <summary>
        /// Setter method of the final property (最终属性的设置方法)
        /// Essential for reflection-based value assignment (基于反射赋值的关键方法)
        /// </summary>
        public MethodInfo SetMethod => Property.SetMethod;

        /// <summary>
        /// Explicit marker for chain property type (明确标识这是链式属性类型)
        /// Always returns true for this implementation (在本实现中恒返回 true)
        /// </summary>
        public bool IsChain => true;

        /// <summary>
        /// Immutable list of property chain elements (不可变的属性链元素集合)
        /// Stored as read-only collection for thread safety (存储为只读集合以保证线程安全)
        /// </summary>
        public IReadOnlyList<PropertyInfo> PropertyChain { get; }

        /// <summary>
        /// Constructs a property chain holder (构造函数)
        /// </summary>
        /// <param name="propertyChain">
        /// Ordered list of properties in the chain (有序属性链列表)
        /// Must contain at least one element (必须包含至少一个元素)
        /// </param>
        /// <param name="typeHandler">
        /// Type conversion handler for the property (属性类型转换处理器)
        /// </param>
        public PropertyChainHolder(List<PropertyInfo> propertyChain, string typeHandler)
        {
            // Capture final property (捕获最终属性)
            property = propertyChain.Last();

            // Create defensive copy as read-only (创建防御性副本作为只读集合)
            PropertyChain = propertyChain.AsReadOnly();

            // Pre-calculate writability status (预计算可写状态)
            CanWrite = PropertyChain.All(property => property.CanWrite);

            // Store type handler (存储类型处理器)
            TypeHandler = typeHandler;
        }
    }
}