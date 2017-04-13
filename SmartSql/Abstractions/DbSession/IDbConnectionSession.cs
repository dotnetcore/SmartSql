using SmartSql.Abstractions.DataSource;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;

namespace SmartSql.Abstractions.DbSession
{
    /// <summary>
    /// 数据库连接会话
    /// </summary>
    public interface IDbConnectionSession : IDisposable
    {
        Guid Id { get; }
        DbProviderFactory DbProviderFactory { get; }
        IDataSource DataSource { get; }
        IDbConnection Connection { get; }
        IDbTransaction Transaction { get; }
        void CreateConnection();
        void OpenConnection();
        void CloseConnection();
        DbSessionLifeCycle LifeCycle { get; set; }
        void BeginTransaction();
        void BeginTransaction(IsolationLevel isolationLevel);
        void CommitTransaction();
        void RollbackTransaction();
    }
    /// <summary>
    /// Db会话生命周期
    /// </summary>
    public enum DbSessionLifeCycle
    {
        /// <summary>
        /// 瞬态
        /// </summary>
        Transient = 1,
        /// <summary>
        /// 执行作用域
        /// [ Transaction | ... ]
        /// </summary>
        Scoped = 2
    }
}
