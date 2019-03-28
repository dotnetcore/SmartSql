using SmartSql.DataSource;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmartSql.DataSource
{
    /// <summary>
    /// 数据源筛选器
    /// </summary>
    public interface IDataSourceFilter
    {
        AbstractDataSource Elect(AbstractRequestContext context);
    }
}
