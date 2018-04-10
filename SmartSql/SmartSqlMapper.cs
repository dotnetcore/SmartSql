using SmartSql.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Data.Common;
using System.Threading.Tasks;
using System.Data;
using SmartSql.Abstractions.DbSession;
using Dapper;
using SmartSql.Exceptions;
using SmartSql.DbSession;
using SmartSql.Abstractions.DataSource;
using SmartSql.Common;
using SmartSql.Abstractions.Cache;
using SmartSql.Abstractions.Config;
using Microsoft.Extensions.Logging;
using SmartSql.Abstractions.Logging;
using SmartSql.Configuration;

namespace SmartSql
{
    public class SmartSqlMapper : ISmartSqlMapper
    {
        private readonly ILoggerFactory _loggerFactory;
        private readonly ILogger _logger;
        public SmartSqlMapConfig SqlMapConfig { get; private set; }
        public DbProviderFactory DbProviderFactory { get; }
        public IDbConnectionSessionStore SessionStore { get; }
        public ISqlBuilder SqlBuilder { get; }
        public IDataSourceManager DataSourceManager { get; }
        public ICacheManager CacheManager { get; }
        public IConfigLoader ConfigLoader { get; }
        private SqlRuner _sqlRuner;

        public SmartSqlMapper(String sqlMapConfigFilePath = "SmartSqlMapConfig.xml") : this(NullLoggerFactory.Instance, sqlMapConfigFilePath)
        {

        }
        public SmartSqlMapper(
             ILoggerFactory loggerFactory,
             String sqlMapConfigFilePath = "SmartSqlMapConfig.xml"
        )
        {
            _loggerFactory = loggerFactory;
            _logger = loggerFactory.CreateLogger<SmartSqlMapper>();
            ConfigLoader = new LocalFileConfigLoader(sqlMapConfigFilePath, loggerFactory);
            SqlMapConfig = ConfigLoader.Load();
            DbProviderFactory = SqlMapConfig.Database.DbProvider.DbProviderFactory;
            SessionStore = new DbConnectionSessionStore(loggerFactory, this.GetHashCode().ToString());
            SqlBuilder = new SqlBuilder(loggerFactory, this);
            DataSourceManager = new DataSourceManager(loggerFactory, this);
            CacheManager = new CacheManager(loggerFactory, this);
            _sqlRuner = new SqlRuner(loggerFactory, SqlBuilder, this);

            ConfigLoader.OnChanged = SqlConfigOnChanged;
            SqlMapConfig.SetLogger(_loggerFactory.CreateLogger<SmartSqlMapConfig>());
        }
        public SmartSqlMapper(ILoggerFactory loggerFactory, String sqlMapConfigFilePath, IConfigLoader configLoader)
        {
            _loggerFactory = loggerFactory;
            _logger = loggerFactory.CreateLogger<SmartSqlMapper>();
            ConfigLoader = configLoader;
            SqlMapConfig = ConfigLoader.Load();
            DbProviderFactory = SqlMapConfig.Database.DbProvider.DbProviderFactory;
            SessionStore = new DbConnectionSessionStore(loggerFactory, this.GetHashCode().ToString());
            SqlBuilder = new SqlBuilder(loggerFactory, this);
            DataSourceManager = new DataSourceManager(loggerFactory, this);
            CacheManager = new CacheManager(loggerFactory, this);
            _sqlRuner = new SqlRuner(loggerFactory, SqlBuilder, this);

            ConfigLoader.OnChanged = SqlConfigOnChanged;
            SqlMapConfig.SetLogger(_loggerFactory.CreateLogger<SmartSqlMapConfig>());
        }

        public SmartSqlMapper(
            ILoggerFactory loggerFactory,
             String sqlMapConfigFilePath
            , IDbConnectionSessionStore sessionStore
            , IDataSourceManager dataSourceManager
            , ICacheManager cacheManager
            , ISqlBuilder sqlBuilder
            , IConfigLoader configLoader
            )
        {
            _loggerFactory = loggerFactory;
            _logger = loggerFactory.CreateLogger<SmartSqlMapper>();
            SqlMapConfig = ConfigLoader.Load();
            DbProviderFactory = SqlMapConfig.Database.DbProvider.DbProviderFactory;
            SessionStore = sessionStore;
            SqlBuilder = sqlBuilder;
            DataSourceManager = dataSourceManager;
            CacheManager = cacheManager;
            CacheManager.SmartSqlMapper = this;
            _sqlRuner = new SqlRuner(loggerFactory, SqlBuilder, this);

            ConfigLoader.OnChanged = SqlConfigOnChanged;
            SqlMapConfig.SetLogger(_loggerFactory.CreateLogger<SmartSqlMapConfig>());
        }

        public void SqlConfigOnChanged(ConfigChangedEvent configChangedEvent)
        {
            SqlMapConfig = configChangedEvent.SqlMapConfig;
            CacheManager.ResetMappedCaches();
        }
        #region Sync
        public int Execute(RequestContext context)
        {
            int result = _sqlRuner.Run<int>(context, DataSourceChoice.Write, (sqlStr, session) =>
            {
                return session.Connection.Execute(sqlStr, context.DapperParameters, session.Transaction);
            });
            CacheManager.TriggerFlush(context);
            return result;
        }
        public T ExecuteScalar<T>(RequestContext context)
        {
            T result = _sqlRuner.Run<T>(context, DataSourceChoice.Write, (sqlStr, session) =>
             {
                 return session.Connection.ExecuteScalar<T>(sqlStr, context.DapperParameters, session.Transaction);
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
                var result = session.Connection.Query<T>(sqlStr, context.DapperParameters, session.Transaction);
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
                 return session.Connection.QuerySingleOrDefault<T>(sqlStr, context.DapperParameters, session.Transaction);
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
                return _session.Connection.ExecuteAsync(sqlStr, context.DapperParameters, _session.Transaction);
            });
            CacheManager.TriggerFlush(context);
            return result;
        }
        public async Task<T> ExecuteScalarAsync<T>(RequestContext context)
        {
            T result = await _sqlRuner.RunAsync<T>(context, DataSourceChoice.Write, (sqlStr, _session) =>
           {
               return _session.Connection.ExecuteScalarAsync<T>(sqlStr, context.DapperParameters, _session.Transaction);
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
                var result = await session.Connection.QueryAsync<T>(sqlStr, context.DapperParameters, session.Transaction);
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
                return _session.Connection.QuerySingleOrDefaultAsync<T>(sqlStr, context.DapperParameters, _session.Transaction);
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
            _logger.LogDebug($"BeginTransaction DbSession.Id:{session.Id}");
            return session;
        }
        public IDbConnectionSession BeginTransaction(IsolationLevel isolationLevel)
        {
            var session = BeginSession(DataSourceChoice.Write);
            session.BeginTransaction(isolationLevel);
            _logger.LogDebug($"BeginTransaction DbSession.Id:{session.Id}");
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
                _logger.LogDebug($"CommitTransaction DbSession.Id:{session.Id}");
                CacheManager.FlushQueue();
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
                _logger.LogDebug($"RollbackTransaction DbSession.Id:{session.Id}");
                CacheManager.ClearQueue();
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
            IDbConnectionSession session = new DbConnectionSession(_loggerFactory, DbProviderFactory, dataSource);
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
            _logger.LogWarning($"SmartSqlMapper Dispose.");
        }
    }
}
