using SmartSql.DataSource;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmartSql.Options
{
    public class Database
    {
        public DbProvider DbProvider { get; set; }
        public DataSource Write { get; set; }
        public IEnumerable<DataSource> Reads { get; set; }
    }
}
