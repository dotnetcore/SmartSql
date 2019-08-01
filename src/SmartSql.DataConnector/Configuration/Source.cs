using System;

namespace SmartSql.DataConnector.Configuration
{
    public class Source
    {
        public String Scope { get; set; }
        public String[] SqlIds { get; set; }
        public Column PrimaryKey { get; set; }
    }
}