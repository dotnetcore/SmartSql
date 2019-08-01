using System;
using System.Collections.Generic;

namespace SmartSql.DataConnector.Configuration
{
    public class Task
    {
        public String Name { get; set; }
        public Subscriber Subscriber { get; set; }
        public DataSource DataSource { get; set; }
        public IDictionary<String,Job> Jobs { get; set; }
    }
}