using System;
using System.Collections.Generic;
using SmartSql.Annotations;

namespace SmartSql.DataConnector.Configuration
{
    public class Destination
    {
        public DataSource DataSource { get; set; }
        public String TableName { get; set; }
        public Dictionary<String, Column> ColumnMapping { get; set; }
    }
}