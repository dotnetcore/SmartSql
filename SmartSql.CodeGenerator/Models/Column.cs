using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartSql.CodeGenerator.Models
{
    public class Column
    {
        /// <summary>
        /// 编号
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 列名
        /// </summary>
        public String Name { get; set; }
        /// <summary>
        /// 类型
        /// </summary>
        public String Type { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public String Description { get; set; }
        /// <summary>
        /// 是否可为空
        /// </summary>
        public bool IsNullable { get; set; }
        /// <summary>
        /// 是否为标识变量
        /// </summary>
        public bool IsIdentity { get; set; }
        /// <summary>
        /// 是否为主键
        /// </summary>
        public bool IsPrimaryKey { get; set; }
    }
}
