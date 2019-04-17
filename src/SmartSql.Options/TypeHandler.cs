using System;
using System.Collections.Generic;
using System.Text;

namespace SmartSql.Options
{
    public class TypeHandler
    {
        public String Name { get; set; }
        public String PropertyType { get; set; }
        public String FieldType { get; set; }
        public String Type { get; set; }
        public IDictionary<String, Object> Properties { get; set; }
    }
}
