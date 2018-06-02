using System;
using System.Collections.Generic;
using System.Text;
using SmartSql.Abstractions.DbSession;

namespace SmartSql.Abstractions.DataSource
{
    /// <summary>
    /// 数据源管理器
    /// </summary>
    public interface IDataSourceManager
    {
        /// <summary>
        /// 获取数据源
        /// </summary>
        /// <param name="sourceChoice"></param>
        /// <returns></returns>
        IDataSource GetDataSource(DataSourceChoice sourceChoice);

    }


}
