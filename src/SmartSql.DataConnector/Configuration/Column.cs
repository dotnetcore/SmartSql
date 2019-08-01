using System;
using SmartSql.Annotations;

namespace SmartSql.DataConnector.Configuration
{
    public class Column
    {
        public String Name { get; set; }
        public bool IsPrimaryKey { get; set; }
        public bool IsAutoIncrement { get; set; }
    }
}