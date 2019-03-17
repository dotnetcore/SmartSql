using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace SmartSql.Annotations
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class ColumnAttribute : Attribute
    {
        public ColumnAttribute() { }
        public ColumnAttribute(string name)
        {
            Name = name;
        }
        public PropertyInfo Property { get; set; }
        public string Name { get; set; }
        public bool IsPrimaryKey { get; set; }
        public bool IsAutoIncrement { get; set; }
    }
}
