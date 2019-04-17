using SmartSql.Reflection.TypeConstants;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmartSql.Reflection
{
    public static class TypeExtensions
    {
        public static object Default(this Type type)
        {
            var nullUnderType = Nullable.GetUnderlyingType(type);
            if (nullUnderType != null) { return null; }
            if (type.IsEnum)
            {
                var enumUnderType = Enum.GetUnderlyingType(type);
                var defaultVal = enumUnderType.Default();
                return Enum.ToObject(type, defaultVal);
            }
            if (type.IsValueType)
            {
                if (type == CommonType.DateTime)
                {
                    return default(DateTime);
                }
                if (type == CommonType.TimeSpan)
                {
                    return default(TimeSpan);
                }
                if (type == CommonType.Guid)
                {
                    return default(Guid);
                }
                return 0;
            }
            return null;
        }
    }
}
