using System;
using System.Collections.Generic;
using System.Text;

namespace SmartSql.Abstractions.DataSource
{
    /// <summary>
    /// 数据源-读库
    /// </summary>
    public interface IReadDataSource : IDataSource
    {
        /// <summary>
        /// 权重
        /// </summary>
        int Weight { get; set; }
    }
}
