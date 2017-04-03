using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Collections;
using SmartSql.SqlMap;
using System.Data.Common;
using System.Threading.Tasks;
using SmartSql.Abstractions.DbSession;
using SmartSql.Abstractions.DataSource;

namespace SmartSql.Abstractions
{
    /// <summary>
    /// SmartSql 映射器
    /// </summary>
    public interface ISmartSqlMapper : ISmartSqlMapperAsync,IDisposable
    {
        SmartSqlMapConfig SqlMapConfig { get; }
        IDataSourceManager DataSourceManager { get; }
        ISqlBuilder SqlBuilder { get; }
        DbProviderFactory DbProviderFactory { get; }
        IDbConnectionSessionStore SessionStore { get; }
        IDbConnectionSession CreateDbSession(DataSourceChoice commandMethod);

        void LoadConfig(SmartSqlMapConfig smartSqlMapConfig);

        int Execute(IRequestContext context);
        T ExecuteScalar<T>(IRequestContext context);
        IEnumerable<T> Query<T>(IRequestContext context);
        T QuerySingle<T>(IRequestContext context);
        #region Transaction
        IDbConnectionSession BeginTransaction();
        IDbConnectionSession BeginTransaction(IsolationLevel isolationLevel);
        void CommitTransaction();
        void RollbackTransaction();
        #endregion
    }

    public enum DataSourceChoice
    {
        Write,
        Read
    }
}
