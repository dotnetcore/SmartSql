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
        public String TypeName { get; set; }
        public TableType Type
        {
            get
            {
                switch (TypeName.Trim())
                {
                    case "U": return TableType.Table;
                    case "V": return TableType.View;
                    default: throw new ArgumentException("参数错误！", "Table.TypeName");
                }
            }
        }
        /// <summary>
        /// 描述
        /// </summary>
        public String Description { get; set; }
        public IEnumerable<Column> Columns { get; set; }

        public enum TableType
        {
            Table,
            View
        }
    }
}
