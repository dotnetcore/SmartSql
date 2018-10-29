using System;
using System.Collections.Generic;
using System.Text;

namespace SmartSql
{
    public class DbSet
    {
        public DbSet()
        {
            Tables = new List<DbTable>();
        }
        public IList<DbTable> Tables { get; set; }
    }
}
