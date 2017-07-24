using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartSql.CodeGenerator.Models
{
    public class Project
    {
        public Author Author { get; set; }
        public String Name { get; set; }
        public IList<Database> Databases { get; set; }
    }
}
