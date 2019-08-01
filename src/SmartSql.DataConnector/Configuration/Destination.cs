using System;
using System.Collections.Generic;
using System.Linq;

namespace SmartSql.DataConnector.Configuration
{
    public class Destination
    {
        public String Schema { get; set; }
        public String TableName { get; set; }
        public Column PrimaryKey => ColumnMapping.Values.FirstOrDefault(col => col.IsPrimaryKey);
        public Dictionary<String, Column> ColumnMapping { get; set; }
    }
}