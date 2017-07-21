using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartSql.CodeGenerator.Models
{
    public class Database
    {
        public String ConnectionString { get; set; }
        public IList<Table> Tables { get; set; }
    }
}
