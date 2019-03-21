using System;
using System.Collections.Generic;
using System.Text;

namespace SmartSql.Options
{
    public class DataSource
    {
        /// <summary>
        /// 数据源名称
        /// </summary>
        public String Name { get; set; }
        /// <summary>
        /// 数据源链接字符串
        /// </summary>
        public String ConnectionString { get; set; }
        /// <summary>
        /// 权重
        /// </summary>
        public int Weight { get; set; }
    }
}
