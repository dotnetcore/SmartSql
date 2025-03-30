using System;
using System.Collections.Generic;
using System.Reflection;

namespace SmartSql.Reflection
{
    internal interface IPropertyHolder
    {
        PropertyInfo Property { get; set; }
        String TypeHandler { get; set; }

        Type PropertyType { get; }

        bool CanWrite { get; }

        MethodInfo SetMethod { get; }

        /// <summary>
        /// 是否为属性链（如 User.Address.City）
        /// </summary>
        bool IsChain { get; }

        /// <summary>
        /// 属性链中的中间属性（仅当 IsChain = true 时有效）
        /// </summary>
        IReadOnlyList<PropertyInfo> PropertyChain { get; }
    }
}