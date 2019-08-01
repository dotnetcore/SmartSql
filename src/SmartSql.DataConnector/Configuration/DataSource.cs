using System;
using System.Collections.Generic;

namespace SmartSql.DataConnector.Configuration
{
    public class DataSource
    {
        public String Type { get; set; }
        public Dictionary<String, object> Parameters { get; set; } = new Dictionary<String, object>();

        public SmartSqlBuilder Instance { get; set; }
    }
}