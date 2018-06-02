using SmartSql.Abstractions.DbSession;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmartSql.Abstractions.DataSource
{
    /// <summary>
    /// 数据源筛选器
    /// </summary>
    public interface IDataSourceFilter
    {
        IDataSource Elect(RequestContext context);
    }
}
