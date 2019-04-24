using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace SmartSql.Reflection.TypeConstants
{
    public static class DefaultType
    {
        public static readonly Type GenericType = typeof(Default<>);

        public static Type MakeGenericType(Type type)
        {
            return GenericType.MakeGenericType(type);
        }
        public static FieldInfo GetDefaultField(Type type)
        {
            return MakeGenericType(type).GetField("Value");
        }
    }
}
