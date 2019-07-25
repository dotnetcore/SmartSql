using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using SmartSql.TypeHandlers;

namespace SmartSql.Annotations
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class ColumnAttribute : Attribute
    {
        public ColumnAttribute()
        {
        }

        public ColumnAttribute(string name)
        {
            Name = name;
        }

        public int? Ordinal { get; set; }
        public PropertyInfo Property { get; set; }
        public Type FieldType { get; set; }
        public Func<object, object> GetPropertyValue { get; set; }
        public string Name { get; set; }
        public bool IsPrimaryKey { get; set; }
        public bool IsAutoIncrement { get; set; }
        public String TypeHandler { get; set; }
    }
}