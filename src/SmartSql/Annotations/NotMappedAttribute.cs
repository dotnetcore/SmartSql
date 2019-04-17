using System;
using System.Collections.Generic;
using System.Text;

namespace SmartSql.Annotations
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class NotMappedAttribute : Attribute
    {
    }
}
