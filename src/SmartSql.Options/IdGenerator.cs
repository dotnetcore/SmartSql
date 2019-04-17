using System;
using System.Collections.Generic;
using System.Text;

namespace SmartSql.Options
{
    public class IdGenerator
    {
        public String Name { get; set; }
        public String Type { get; set; }
        public IDictionary<String, object> Properties { get; set; }
    }
}
