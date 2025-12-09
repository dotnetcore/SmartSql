using System;
using System.Collections.Generic;
using System.Reflection;

namespace SmartSql.Reflection
{
    internal interface IPropertyHolder
    {
        PropertyInfo Property { get; }
        String TypeHandler { get; set; }

        Type PropertyType { get; }

        bool CanWrite { get; }

        MethodInfo SetMethod { get; }

        bool IsChain { get; }

        IReadOnlyList<PropertyInfo> PropertyChain { get; }
    }
}