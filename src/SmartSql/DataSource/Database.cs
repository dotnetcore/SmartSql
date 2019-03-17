using System;
using System.Collections.Generic;
using System.Text;

namespace SmartSql.DataSource
{
    public class Database
    {
        public DbProvider DbProvider { get; set; }
        public WriteDataSource Write { get; set; }
        public IDictionary<String, ReadDataSource> Reads { get; set; }
    }
}
