using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Collections;
using System.Data.Common;
using System.Threading.Tasks;
using SmartSql.Abstractions.DbSession;
using SmartSql.Abstractions.DataSource;
using SmartSql.Abstractions.Cache;
using SmartSql.Abstractions.Config;
using SmartSql.Configuration;

namespace SmartSql.Abstractions
{
    /// <summary>
    /// SmartSql 映射器
    /// </summary>
    public interface ISmartSqlMapper : ISmartSqlMapperAsync, ISession, ITransaction,  IDisposable
    {
        int Execute(RequestContext context);
        T ExecuteScalar<T>(RequestContext context);
        IEnumerable<T> Query<T>(RequestContext context);
        T QuerySingle<T>(RequestContext context);

        DataTable GetDataTable(RequestContext context);
        DataSet GetDataSet(RequestContext context);
    }

    public enum DataSourceChoice
    {
        Write,
        Read
    }
}
