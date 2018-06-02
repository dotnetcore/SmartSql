using System;
using System.Collections.Generic;
using System.Text;

namespace SmartSql.Abstractions.DataSource
{
    /// <summary>
    /// 数据源
    /// </summary>
    public interface IDataSource
    {
        /// <summary>
        /// 数据源名称
        /// </summary>
        String Name { get; set; }
        /// <summary>
        /// 数据源链接字符串
        /// </summary>
        String ConnectionString { get; set; }
    }
}
