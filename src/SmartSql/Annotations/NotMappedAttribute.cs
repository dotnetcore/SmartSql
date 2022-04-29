using System;

namespace SmartSql.Annotations
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class NotMappedAttribute : Attribute
    {
    }
}
