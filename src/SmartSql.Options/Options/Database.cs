using System;
using System.Collections.Generic;
using System.Text;

namespace SmartSql.Configuration.Options
{
    public class Database
    {
        public DbProvider DbProvider { get; set; }

        public WriteDataSource Write { get; set; }

        public List<ReadDataSource> Read { get; set; }
    }
}