using SmartSql.TypeHandlers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmartSql.Configuration
{
    public class ResultMap
    {
        public string Id { get; set; }
        public Constructor Constructor { get; set; }
        /// <summary>
        /// Key:Column
        /// </summary>
        public IDictionary<String, Property> Properties { get; set; }
        public Property GetProperty(string columnName)
        {
            if (Properties.TryGetValue(columnName, out var property))
            {
                return property;
            }
            return null;
        }
    }

    public class Constructor
    {
        public IList<Arg> Args { get; set; }
    }

    public class Arg
    {
        public string Column { get; set; }
        public Type CSharpType { get; set; }
        public ITypeHandler Handler { get; set; }
    }

    public class Property
    {
        public string Name { get; set; }
        public string Column { get; set; }
        public String TypeHandler { get; set; }
    }
}
