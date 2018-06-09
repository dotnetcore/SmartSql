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
    public interface ISmartSqlMapper : ISmartSqlMapperAsync, IDisposable
    {
        int Execute(RequestContext context);
        T ExecuteScalar<T>(RequestContext context);
        IEnumerable<T> Query<T>(RequestContext context);
        T QuerySingle<T>(RequestContext context);

        DataTable GetDataTable(RequestContext context);
        DataSet GetDataSet(RequestContext context);

        #region Transaction
        IDbConnectionSession BeginTransaction();
        IDbConnectionSession BeginTransaction(RequestContext context);
        IDbConnectionSession BeginTransaction(IsolationLevel isolationLevel);
        IDbConnectionSession BeginTransaction(RequestContext context, IsolationLevel isolationLevel);
        void CommitTransaction();
        void RollbackTransaction();
        #endregion
        #region Scoped Session
        IDbConnectionSession BeginSession(RequestContext context);
        void EndSession();
        #endregion
    }

    public enum DataSourceChoice
    {
        Write,
        Read
    }
}
