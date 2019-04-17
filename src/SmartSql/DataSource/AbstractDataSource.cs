using System;
using System.Collections.Generic;
using System.Text;

namespace SmartSql.DataSource
{
    /// <summary>
    /// 数据源
    /// </summary>
    public abstract class AbstractDataSource
    {
        /// <summary>
        /// 数据源名称
        /// </summary>
        public String Name { get; set; }
        /// <summary>
        /// 数据源链接字符串
        /// </summary>
        public String ConnectionString { get; set; }

        public DbProvider DbProvider { get; set; }
    }
}
