using System.Collections.Generic;
using SmartSql.Configuration;

namespace SmartSql.Options
{
    public class Database
    {
        public DbProvider DbProvider { get; set; }

        public WriteDataSource Write { get; set; }

        public List<ReadDataSource> Read { get; set; }
    }
}