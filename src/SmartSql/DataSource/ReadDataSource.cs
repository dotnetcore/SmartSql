using System;
using System.Collections.Generic;
using System.Text;

namespace SmartSql.DataSource
{
    /// <summary>
    /// 数据源-读库
    /// </summary>
    public class ReadDataSource : AbstractDataSource
    {
        /// <summary>
        /// 权重
        /// </summary>
        public int Weight { get; set; }
    }
}
