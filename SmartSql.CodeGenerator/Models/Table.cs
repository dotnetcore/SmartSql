using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartSql.CodeGenerator.Models
{
    public class Table
    {
        /// <summary>
        /// 编号
        /// </summary>
        public int Id { get; set; }
        
        /// <summary>
        /// 表名
        /// </summary>
        public String Name { get; set; }
        public TableType Type { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public String Description { get; set; }
        public Author Author { get; set; }
        public Database Database { get; set; }
        public IEnumerable<Column> Columns { get; set; }

        public enum TableType
        {
            Table,
            View
        }
    }
}
