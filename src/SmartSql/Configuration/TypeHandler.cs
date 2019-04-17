using System;
using System.Collections.Generic;
using System.Text;

namespace SmartSql.Configuration
{
    public class TypeHandler
    {
        public String Name { get; set; }
        public Type PropertyType { get; set; }
        public Type FieldType { get; set; }
        public Type HandlerType { get; set; }
        public IDictionary<String,Object> Properties { get; set; }
    }
}
