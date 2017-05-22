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
using SmartSql.Abstractions.Cache;

namespace SmartSql
{
    public class SmartSqlMapper : ISmartSqlMapper
    {
        private static readonly ILog _logger = LogManager.GetLogger(typeof(SmartSqlMapper));
        public SmartSqlMapConfig SqlMapConfig { get; private set; }
        public DbProviderFactory DbProviderFactory { get; }
        public IDbConnectionSessionStore SessionStore { get; }
        public ISqlBuilder SqlBuilder { get; }
        public IDataSourceManager DataSourceManager { get; }
        public ICacheManager CacheManager { get; }
        public IConfigLoader ConfigLoader { get; }
        private SqlRuner _sqlRuner;
        public SmartSqlMapper(
             String sqlMapConfigFilePath = "SmartSqlMapConfig.xml"
        )
        {
            ConfigLoader = new LocalFileConfigLoader();
            ConfigLoader.Load(sqlMapConfigFilePath, this);
            DbProviderFactory = SqlMapConfig.Database.DbProvider.DbProviderFactory;
            SessionStore = new DbConnectionSessionStore(this.GetHashCode().ToString());
            SqlBuilder = new SqlBuilder(this);
            DataSourceManager = new DataSourceManager(this);
            CacheManager = new CacheManager(this);
            _sqlRuner = new SqlRuner(SqlBuilder, this);
        }
        public SmartSqlMapper(String sqlMapConfigFilePath, IConfigLoader configLoader)
        {
            ConfigLoader = configLoader;
            ConfigLoader.Load(sqlMapConfigFilePath, this);
            DbProviderFactory = SqlMapConfig.Database.DbProvider.DbProviderFactory;
            SessionStore = new DbConnectionSessionStore(this.GetHashCode().ToString());
            SqlBuilder = new SqlBuilder(this);
            DataSourceManager = new DataSourceManager(this);
            CacheManager = new CacheManager(this);
            _sqlRuner = new SqlRuner(SqlBuilder, this);
        }

        public SmartSqlMapper(
             String sqlMapConfigFilePath
            , IDbConnectionSessionStore sessionStore
            , IDataSourceManager dataSourceManager
            , ICacheManager cacheManager
            , ISqlBuilder sqlBuilder
            , IConfigLoader configLoader
            )
        {
            configLoader.Load(sqlMapConfigFilePath, this);
            DbProviderFactory = SqlMapConfig.Database.DbProvider.DbProviderFactory;
            SessionStore = sessionStore;
            SqlBuilder = sqlBuilder;
            DataSourceManager = dataSourceManager;
            CacheManager = cacheManager;
            _sqlRuner = new SqlRuner(SqlBuilder, this);
        }

        public void LoadConfig(SmartSqlMapConfig smartSqlMapConfig)
        {
            SqlMapConfig = smartSqlMapConfig;
        }
        #region Sync
        public int Execute(RequestContext context)
        {
            int result = _sqlRuner.Run<int>(context, DataSourceChoice.Write, (sqlStr, session) =>
            {
                return session.Connection.Execute(sqlStr, context.Request, session.Transaction);
            });
            CacheManager.TriggerFlush(context);
            return result;
        }
        public T ExecuteScalar<T>(RequestContext context)
        {
            T result = _sqlRuner.Run<T>(context, DataSourceChoice.Write, (sqlStr, session) =>
             {
                 return session.Connection.ExecuteScalar<T>(sqlStr, context.Request, session.Transaction);
             });
            CacheManager.TriggerFlush(context);
            return result;
        }
        public IEnumerable<T> Query<T>(RequestContext context)
        {
            return Query<T>(context, DataSourceChoice.Read);
        }
        public IEnumerable<T> Query<T>(RequestContext context, DataSourceChoice sourceChoice)
        {
            var cache = CacheManager[context, typeof(IEnumerable<T>)];
            if (cache != null)
            {
                return (IEnumerable<T>)cache;
            }

            IDbConnectionSession session = SessionStore.LocalSession;

            if (session == null)
            {
                session = CreateDbSession(sourceChoice);
            }
            string sqlStr = SqlBuilder.BuildSql(context);
            try
            {
                var result = session.Connection.Query<T>(sqlStr, context.Request, session.Transaction);
                CacheManager[context, typeof(IEnumerable<T>)] = result;
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (session.LifeCycle == DbSessionLifeCycle.Transient)
                {
                    session.CloseConnection();
                }
            }
        }

        public T QuerySingle<T>(RequestContext context)
        {
            return QuerySingle<T>(context, DataSourceChoice.Read);
        }
        public T QuerySingle<T>(RequestContext context, DataSourceChoice sourceChoice)
        {
            var cache = CacheManager[context, typeof(T)];
            if (cache != null)
            {
                return (T)cache;
            }
            var result = _sqlRuner.Run<T>(context, sourceChoice, (sqlStr, session) =>
             {
                 return session.Connection.QuerySingleOrDefault<T>(sqlStr, context.Request, session.Transaction);
             });
            CacheManager[context, typeof(T)] = result;
            return result;
        }
        #endregion
        #region Async
        public async Task<int> ExecuteAsync(RequestContext context)
        {
            int result = await _sqlRuner.RunAsync<int>(context, DataSourceChoice.Write, (sqlStr, _session) =>
            {
                return _session.Connection.ExecuteAsync(sqlStr, context.Request, _session.Transaction);
            });
            CacheManager.TriggerFlush(context);
            return result;
        }
        public async Task<T> ExecuteScalarAsync<T>(RequestContext context)
        {
            T result = await _sqlRuner.RunAsync<T>(context, DataSourceChoice.Write, (sqlStr, _session) =>
           {
               return _session.Connection.ExecuteScalarAsync<T>(sqlStr, context.Request, _session.Transaction);
           });
            CacheManager.TriggerFlush(context);
            return result;
        }
        public async Task<IEnumerable<T>> QueryAsync<T>(RequestContext context)
        {
            return await QueryAsync<T>(context, DataSourceChoice.Read);
        }

        public async Task<IEnumerable<T>> QueryAsync<T>(RequestContext context, DataSourceChoice sourceChoice)
        {
            var cache = CacheManager[context, typeof(IEnumerable<T>)];
            if (cache != null)
            {
                return (IEnumerable<T>)cache;
            }
            IDbConnectionSession session = SessionStore.LocalSession;
            if (session == null)
            {
                session = CreateDbSession(sourceChoice);
            }
            string sqlStr = SqlBuilder.BuildSql(context);
            try
            {
                var result = await session.Connection.QueryAsync<T>(sqlStr, context.Request, session.Transaction);
                CacheManager[context, typeof(IEnumerable<T>)] = result;
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (session.LifeCycle == DbSessionLifeCycle.Transient)
                {
                    session.CloseConnection();
                }
            }
        }
        public async Task<T> QuerySingleAsync<T>(RequestContext context)
        {
            return await QuerySingleAsync<T>(context, DataSourceChoice.Read);
        }
        public async Task<T> QuerySingleAsync<T>(RequestContext context, DataSourceChoice sourceChoice)
        {
            var cache = CacheManager[context, typeof(IEnumerable<T>)];
            if (cache != null)
            {
                return (T)cache;
            }
            var result = await _sqlRuner.RunAsync<T>(context, sourceChoice, (sqlStr, _session) =>
            {
                return _session.Connection.QuerySingleOrDefaultAsync<T>(sqlStr, context.Request, _session.Transaction);
            });
            CacheManager[context, typeof(T)] = result;
            return result;
        }
        #endregion
        #region Transaction
        public IDbConnectionSession BeginTransaction()
        {
            var session = BeginSession(DataSourceChoice.Write);
            session.BeginTransaction();
            _logger.Debug($"SmartSqlMapper.BeginTransaction DbSession.Id:{session.Id}");
            return session;
        }
        public IDbConnectionSession BeginTransaction(IsolationLevel isolationLevel)
        {
            var session = BeginSession(DataSourceChoice.Write);
            session.BeginTransaction(isolationLevel);
            _logger.Debug($"SmartSqlMapper.BeginTransaction DbSession.Id:{session.Id}");
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
        #region Scoped Session
        public IDbConnectionSession BeginSession(DataSourceChoice sourceChoice = DataSourceChoice.Write)
        {
            if (SessionStore.LocalSession != null)
            {
                throw new SmartSqlException("SmartSqlMapper could not invoke BeginSession(). A LocalSession is already existed.");
            }
            var session = CreateDbSession(sourceChoice);
            session.LifeCycle = DbSessionLifeCycle.Scoped;
            session.OpenConnection();
            SessionStore.Store(session);
            return session;
        }

        public void EndSession()
        {
            var session = SessionStore.LocalSession;
            if (session == null)
            {
                throw new SmartSqlException("SmartSqlMapper could not invoke EndSession(). No LocalSession was existed. ");
            }
            session.LifeCycle = DbSessionLifeCycle.Transient;
            session.CloseConnection();
            SessionStore.Dispose();
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
            ConfigLoader.Dispose();
            if (SessionStore.LocalSession != null)
            {
                SessionStore.LocalSession.Dispose();
            }
            SessionStore.Dispose();
        }


    }
}
