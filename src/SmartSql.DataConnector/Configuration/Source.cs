using System;

namespace SmartSql.DataConnector.Configuration
{
    public class Source
    {
        public String SqlId { get; set; }
        public Column PrimaryKey { get; set; }
    }
}