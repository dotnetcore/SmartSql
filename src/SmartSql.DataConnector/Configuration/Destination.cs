using System;
using System.Collections.Generic;

namespace SmartSql.DataConnector.Configuration
{
    public class Destination
    {
        public String Schema { get; set; }
        public String TableName { get; set; }
        public Column PrimaryKey { get; set; }
        public Dictionary<String, Column> ColumnMapping { get; set; }
    }
}