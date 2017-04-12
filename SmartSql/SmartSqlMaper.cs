using SmartSql.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;
using SmartSql.SqlMap;
using System.Data.Common;
using System.Threading.Tasks;
using System.Data;
using SmartSql.Abstractions.DbSession;
using Dapper;
using SmartSql.Exceptions;
using SmartSql.DbSession;
using SmartSql.Abstractions.DataSource;
using SmartSql.Common;
using SmartSql.Abstractions.Logging;

namespace SmartSql
{
    public class SmartSqlMapper : ISmartSqlMapper
    {
        private static readonly ILog _logger = LogManager.GetLogger(typeof(SqlBuilder));
        public SmartSqlMapConfig SqlMapConfig { get; private set; }
        public DbProviderFactory DbProviderFactory { get; }
        public IDbConnectionSessionStore SessionStore { get; }
        public ISqlBuilder SqlBuilder { get; }
        public IDataSourceManager DataSourceManager { get; }
        private SqlRuner _sqlRuner;
        public SmartSqlMapper(
             String sqlMapConfigFilePath = "SmartSqlMapConfig.xml"
        )
        {
            SmartSqlMapConfig.Load(sqlMapConfigFilePath, this);
            DbProviderFactory = SqlMapConfig.Database.DbProvider.DbProviderFactory;
            SessionStore = new DbConnectionSessionStore(this.GetHashCode().ToString());
            SqlBuilder = new SqlBuilder(this);
            DataSourceManager = new DataSourceManager(this);
            _sqlRuner = new SqlRuner(SqlBuilder, this);
        }

        public SmartSqlMapper(
             String sqlMapConfigFilePath
            , IDbConnectionSessionStore sessionStore
            , IDataSourceManager dataSourceManager
           , ISqlBuilder sqlBuilder
            )
        {
            SmartSqlMapConfig.Load(sqlMapConfigFilePath, this);
            DbProviderFactory = SqlMapConfig.Database.DbProvider.DbProviderFactory;
            SessionStore = sessionStore;
            SqlBuilder = sqlBuilder;
            DataSourceManager = dataSourceManager;
            _sqlRuner = new SqlRuner(SqlBuilder, this);
        }

        public void LoadConfig(SmartSqlMapConfig smartSqlMapConfig)
        {
            SqlMapConfig = smartSqlMapConfig;
        }
        #region Sync
        public int Execute(IRequestContext context)
        {
            return _sqlRuner.Run<int>(context, DataSourceChoice.Write, (sqlStr, session) =>
           {
               return session.Connection.Execute(sqlStr, context.Request, session.Transaction);
           });
        }
        public T ExecuteScalar<T>(IRequestContext context)
        {
            return _sqlRuner.Run<T>(context, DataSourceChoice.Write, (sqlStr, session) =>
            {
                return session.Connection.ExecuteScalar<T>(sqlStr, context.Request, session.Transaction);
            });
        }
        public IEnumerable<T> Query<T>(IRequestContext context)
        {
            return Query<T>(context, DataSourceChoice.Read);
        }
        public IEnumerable<T> Query<T>(IRequestContext context, DataSourceChoice sourceChoice)
        {
            IDbConnectionSession session = SessionStore.LocalSession;

            if (session == null)
            {
                session = CreateDbSession(sourceChoice);
            }
            string sqlStr = SqlBuilder.BuildSql(context);
            try
            {
                return session.Connection.Query<T>(sqlStr, context.Request, session.Transaction);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (!session.IsTransactionOpen)
                {
                    session.CloseConnection();
                }
            }
        }

        public T QuerySingle<T>(IRequestContext context)
        {
            return QuerySingle<T>(context, DataSourceChoice.Read);
        }
        public T QuerySingle<T>(IRequestContext context, DataSourceChoice sourceChoice)
        {
            return _sqlRuner.Run<T>(context, sourceChoice, (sqlStr, session) =>
            {
                return session.Connection.QuerySingle<T>(sqlStr, context.Request, session.Transaction);
            });
        }
        #endregion
        #region Async
        public async Task<int> ExecuteAsync(IRequestContext context)
        {
            return await _sqlRuner.RunAsync<int>(context,  DataSourceChoice.Write, (sqlStr, _session) =>
            {
                return _session.Connection.ExecuteAsync(sqlStr, context.Request, _session.Transaction);
            });
        }
        public async Task<T> ExecuteScalarAsync<T>(IRequestContext context)
        {
            return await _sqlRuner.RunAsync<T>(context,  DataSourceChoice.Write, (sqlStr, _session) =>
            {
                return _session.Connection.ExecuteScalarAsync<T>(sqlStr, context.Request, _session.Transaction);
            });
        }
        public async Task<IEnumerable<T>> QueryAsync<T>(IRequestContext context)
        {
            return await QueryAsync<T>(context,  DataSourceChoice.Read);
        }

        public async Task<IEnumerable<T>> QueryAsync<T>(IRequestContext context, DataSourceChoice sourceChoice)
        {
            IDbConnectionSession session = SessionStore.LocalSession;
            if (session == null)
            {
                session = CreateDbSession(sourceChoice);
            }
            string sqlStr = SqlBuilder.BuildSql(context);
            try
            {
                return await session.Connection.QueryAsync<T>(sqlStr, context.Request, session.Transaction);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                //if (!session.IsTransactionOpen)
                //{
                //    session.CloseConnection();
                //}
            }
        }
        public async Task<T> QuerySingleAsync<T>(IRequestContext context)
        {
            return await QuerySingleAsync<T>(context,  DataSourceChoice.Read);
        }
        public async Task<T> QuerySingleAsync<T>(IRequestContext context, DataSourceChoice sourceChoice)
        {
            return await _sqlRuner.RunAsync<T>(context,  sourceChoice, (sqlStr, _session) =>
            {
                return _session.Connection.QuerySingleAsync<T>(sqlStr, context.Request, _session.Transaction);
            });
        }
        #endregion
        #region Transaction
        public IDbConnectionSession BeginTransaction()
        {
            if (SessionStore.LocalSession != null)
            {
                throw new SmartSqlException("SmartSqlMapper could not invoke BeginTransaction(). A Transaction is already started. Call CommitTransaction() or RollbackTransaction first.");
            }
            var session = CreateDbSession(DataSourceChoice.Write);
            SessionStore.Store(session);
            session.BeginTransaction();
            _logger.Debug($"SmartSqlMapper.BeginTransaction DbSession.Id:{session.Id}");
            return session;
        }
        public IDbConnectionSession BeginTransaction(IsolationLevel isolationLevel)
        {
            if (SessionStore.LocalSession != null)
            {
                throw new SmartSqlException("SmartSqlMapper could not invoke BeginTransaction(). A Transaction is already started. Call CommitTransaction() or RollbackTransaction first.");
            }
            var session = CreateDbSession(DataSourceChoice.Write);
            SessionStore.Store(session);
            session.BeginTransaction(isolationLevel);
            return session;
        }
        public void CommitTransaction()
        {
            var session = SessionStore.LocalSession;
            if (session == null)
            {
                throw new SmartSqlException("SmartSqlMapper could not invoke CommitTransaction(). No Transaction was started. Call BeginTransaction() first.");
            }
            try
            {
                _logger.Debug($"SmartSqlMapper.CommitTransaction DbSession.Id:{session.Id}");
                session.CommitTransaction();
            }
            finally
            {
                SessionStore.Dispose();
            }
        }

        public void RollbackTransaction()
        {
            var session = SessionStore.LocalSession;
            if (session == null)
            {
                throw new SmartSqlException("SmartSqlMapper could not invoke RollBackTransaction(). No Transaction was started. Call BeginTransaction() first.");
            }
            try
            {
                _logger.Debug($"SmartSqlMapper.RollbackTransaction DbSession.Id:{session.Id}");
                session.RollbackTransaction();
            }
            finally
            {
                SessionStore.Dispose();
            }
        }

        #endregion
        public IDbConnectionSession CreateDbSession(DataSourceChoice sourceChoice)
        {
            IDataSource dataSource = DataSourceManager.GetDataSource(sourceChoice);
            IDbConnectionSession session = new DbConnectionSession(DbProviderFactory, dataSource);

            session.CreateConnection();
            return session;
        }
        public void Dispose()
        {
            FileWatcherLoader.Instance.Clear();
            if (SessionStore.LocalSession != null)
            {
                SessionStore.LocalSession.Dispose();
            }
            SessionStore.Dispose();
        }
    }
}
