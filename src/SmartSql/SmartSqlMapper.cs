using SmartSql.Abstractions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Data;
using SmartSql.Abstractions.DbSession;
using SmartSql.Abstractions.DataSource;
using SmartSql.Abstractions.Config;
using Microsoft.Extensions.Logging;
using SmartSql.Logging;
using SmartSql.Abstractions.Command;
using SmartSql.Abstractions.DataReaderDeserializer;
using SmartSql.Exceptions;
using System.Linq;
using SmartSql.Abstractions.Cache;
using System.Data.Common;
using SmartSql.Utils;

namespace SmartSql
{
    public class SmartSqlMapper : ISmartSqlMapper
    {
        private readonly SmartSqlOptions _smartSqlOptions;
        private readonly ILogger _logger;
        public IDbConnectionSessionStore SessionStore { get { return _smartSqlOptions.DbSessionStore; } }
        public IDataSourceFilter DataSourceFilter { get { return _smartSqlOptions.DataSourceFilter; } }
        public IConfigLoader ConfigLoader { get { return _smartSqlOptions.ConfigLoader; } }
        public ICommandExecuter CommandExecuter { get { return _smartSqlOptions.CommandExecuter; } }
        public IDataReaderDeserializerFactory DeserializerFactory { get { return _smartSqlOptions.DataReaderDeserializerFactory; } }
        public ILoggerFactory LoggerFactory { get { return _smartSqlOptions.LoggerFactory; } }
        public ICacheManager CacheManager { get { return _smartSqlOptions.CacheManager; } }
        public ISqlBuilder SqlBuilder { get { return _smartSqlOptions.SqlBuilder; } }
        public SmartSqlMapper(String sqlMapConfigFilePath = "SmartSqlMapConfig.xml") : this(NoneLoggerFactory.Instance, sqlMapConfigFilePath)
        {

        }
        public SmartSqlMapper(
             ILoggerFactory loggerFactory,
             String sqlMapConfigFilePath = "SmartSqlMapConfig.xml"
        ) : this(new SmartSqlOptions
        {
            LoggerFactory = loggerFactory,
            ConfigPath = sqlMapConfigFilePath
        })
        {

        }
        public SmartSqlMapper(SmartSqlOptions options)
        {
            _smartSqlOptions = options;
            _smartSqlOptions.Setup();
            _logger = LoggerFactory.CreateLogger<SmartSqlMapper>();
        }
        private void SetupRequestContext(RequestContext context)
        {
            context.Setup(_smartSqlOptions.SmartSqlContext, SqlBuilder);
        }
        #region Sync
        public T ExecuteWrap<T>(Func<IDbConnectionSession, T> execute, RequestContext context)
        {
            SetupRequestContext(context);
            if (CacheManager.TryGet<T>(context, out T cachedResult))
            {
                return cachedResult;
            }
            var dataSource = DataSourceFilter.Elect(context);
            var dbSession = SessionStore.GetOrAddDbSession(dataSource);
            try
            {
                var result = execute(dbSession);
                CacheManager.RequestExecuted(dbSession, context);
                CacheManager.TryAdd<T>(context, result);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.HelpLink, ex, ex.Message);
                throw ex;
            }
            finally
            {
                if (dbSession.LifeCycle == DbSessionLifeCycle.Transient)
                {
                    SessionStore.Dispose();
                }
            }
        }

        public int Execute(RequestContext context)
        {
            return ExecuteWrap((dbSession) =>
             {
                 return CommandExecuter.ExecuteNonQuery(dbSession, context);
             }, context);
        }
        public T ExecuteScalar<T>(RequestContext context)
        {
            return ExecuteWrap((dbSession) =>
            {
                var result = CommandExecuter.ExecuteScalar(dbSession, context);
                return (T)Convert.ChangeType(result, typeof(T));
            }, context);
        }

        public IEnumerable<T> Query<T>(RequestContext context)
        {
            return ExecuteWrap((dbSession) =>
            {
                var dataReader = CommandExecuter.ExecuteReader(dbSession, context);
                var deser = DeserializerFactory.Create();
                return deser.ToEnumerable<T>(context, dataReader).ToList();
            }, context);
        }
        public T QuerySingle<T>(RequestContext context)
        {
            return ExecuteWrap((dbSession) =>
            {
                var dataReader = CommandExecuter.ExecuteReader(dbSession, context);
                var deser = DeserializerFactory.Create();
                return deser.ToSingle<T>(context, dataReader);
            }, context);
        }

        public DataTable GetDataTable(RequestContext context)
        {
            return ExecuteWrap((dbSession) =>
            {
                IDataReader dataReader = null;
                try
                {
                    dataReader = CommandExecuter.ExecuteReader(dbSession, context);
                    return DataReaderConvert.ToDataTable(dataReader);
                }
                finally
                {
                    if (dataReader != null)
                    {
                        dataReader.Dispose();
                    }
                }

            }, context);
        }

        public DataSet GetDataSet(RequestContext context)
        {
            return ExecuteWrap((dbSession) =>
            {
                IDataReader dataReader = null;
                try
                {
                    dataReader = CommandExecuter.ExecuteReader(dbSession, context);
                    return DataReaderConvert.ToDataSet(dataReader);
                }
                finally
                {
                    if (dataReader != null)
                    {
                        dataReader.Dispose();
                    }
                }
            }, context);
        }
        #endregion
        #region Async
        public async Task<T> ExecuteWrapAsync<T>(Func<IDbConnectionSession, Task<T>> execute, RequestContext context)
        {
            SetupRequestContext(context);
            if (CacheManager.TryGet<T>(context, out T cachedResult))
            {
                return cachedResult;
            }
            var dataSource = DataSourceFilter.Elect(context);
            var dbSession = SessionStore.GetOrAddDbSession(dataSource);
            try
            {
                var result = await execute(dbSession).ConfigureAwait(false);
                CacheManager.RequestExecuted(dbSession, context);
                CacheManager.TryAdd<T>(context, result);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.HelpLink, ex, ex.Message);
                throw ex;
            }
            finally
            {
                if (dbSession.LifeCycle == DbSessionLifeCycle.Transient)
                {
                    SessionStore.Dispose();
                }
            }
        }
        public async Task<int> ExecuteAsync(RequestContext context)
        {
            return await ExecuteWrapAsync(async (dbSession) =>
           {
               return await CommandExecuter.ExecuteNonQueryAsync(dbSession, context);
           }, context);
        }
        public async Task<T> ExecuteScalarAsync<T>(RequestContext context)
        {
            return await ExecuteWrapAsync(async (dbSession) =>
            {
                var result = await CommandExecuter.ExecuteScalarAsync(dbSession, context);
                return (T)Convert.ChangeType(result, typeof(T));
            }, context);
        }
        public async Task<IEnumerable<T>> QueryAsync<T>(RequestContext context)
        {
            return await ExecuteWrapAsync(async (dbSession) =>
            {
                var dataReader = await CommandExecuter.ExecuteReaderAsync(dbSession, context);
                var deser = DeserializerFactory.Create();
                return await deser.ToEnumerableAsync<T>(context, dataReader);
            }, context);
        }
        public async Task<T> QuerySingleAsync<T>(RequestContext context)
        {
            return await ExecuteWrapAsync(async (dbSession) =>
            {
                var dataReader = await CommandExecuter.ExecuteReaderAsync(dbSession, context);
                var deser = DeserializerFactory.Create();
                return await deser.ToSingleAsync<T>(context, dataReader);
            }, context);
        }

        public async Task<DataTable> GetDataTableAsync(RequestContext context)
        {
            return await ExecuteWrapAsync(async (dbSession) =>
            {
                DbDataReader dataReader = null;
                try
                {
                    dataReader = await CommandExecuter.ExecuteReaderAsync(dbSession, context);
                    return await DataReaderConvert.ToDataTableAsync(dataReader);
                }
                finally
                {
                    if (dataReader != null)
                    {
                        dataReader.Dispose();
                    }
                }

            }, context);
        }

        public async Task<DataSet> GetDataSetAsync(RequestContext context)
        {
            return await ExecuteWrapAsync(async (dbSession) =>
            {
                DbDataReader dataReader = null;
                try
                {
                    dataReader = await CommandExecuter.ExecuteReaderAsync(dbSession, context);
                    return await DataReaderConvert.ToDataSetAsync(dataReader);
                }
                finally
                {
                    if (dataReader != null)
                    {
                        dataReader.Dispose();
                    }
                }
            }, context);
        }
        #endregion
        #region Transaction
        public IDbConnectionSession BeginTransaction()
        {
            return BeginTransaction(IsolationLevel.Unspecified);
        }

        public IDbConnectionSession BeginTransaction(IsolationLevel isolationLevel)
        {
            var reqContext = new RequestContext
            {
                DataSourceChoice = DataSourceChoice.Write
            };

            return BeginTransaction(reqContext, isolationLevel);
        }

        public IDbConnectionSession BeginTransaction(RequestContext context)
        {
            return BeginTransaction(context, IsolationLevel.Unspecified);
        }

        public IDbConnectionSession BeginTransaction(RequestContext context, IsolationLevel isolationLevel)
        {
            var dbSession = BeginSession(context);
            dbSession.BeginTransaction(isolationLevel);
            if (_logger.IsEnabled(LogLevel.Debug))
            {
                _logger.LogDebug($"BeginTransaction DbSession.Id:{dbSession.Id}");
            }
            return dbSession;
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
                if (_logger.IsEnabled(LogLevel.Debug))
                {
                    _logger.LogDebug($"CommitTransaction DbSession.Id:{session.Id}");
                }
                session.CommitTransaction();
                CacheManager.RequestCommitted(session);
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
                if (_logger.IsEnabled(LogLevel.Debug))
                {
                    _logger.LogDebug($"RollbackTransaction DbSession.Id:{session.Id}");
                }
                session.RollbackTransaction();
            }
            finally
            {
                SessionStore.Dispose();
            }
        }

        #endregion
        #region Scoped Session
        public IDbConnectionSession BeginSession()
        {
            var reqContext = new RequestContext
            {
                DataSourceChoice = DataSourceChoice.Read
            };
            var dbSession = BeginSession(reqContext);
            return dbSession;
        }
        public IDbConnectionSession BeginSession(RequestContext context)
        {
            if (SessionStore.LocalSession != null)
            {
                throw new SmartSqlException("SmartSqlMapper could not invoke BeginSession(). A LocalSession is already existed.");
            }
            var dataSource = DataSourceFilter.Elect(context);
            var dbSession = SessionStore.CreateDbSession(dataSource);
            dbSession.Begin();
            return dbSession;
        }

        public void EndSession()
        {
            var dbSession = SessionStore.LocalSession;
            if (dbSession == null)
            {
                throw new SmartSqlException("SmartSqlMapper could not invoke EndSession(). No LocalSession was existed. ");
            }
            dbSession.End();
            SessionStore.Dispose();
        }
        #endregion
        public void Dispose()
        {
            ConfigLoader.Dispose();
            SessionStore.Dispose();
            CacheManager.Dispose();
            if (_logger.IsEnabled(LogLevel.Warning))
            {
                _logger.LogWarning($"SmartSqlMapper Dispose.");
            }
        }




    }
}
