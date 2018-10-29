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
using SmartSql.Abstractions.Cache;
using SmartSql.Utils;
using SmartSql.DataReaderDeserializer;

namespace SmartSql
{
    public class SmartSqlMapper : ISmartSqlMapper
    {
        private readonly ILogger _logger;
        public SmartSqlOptions SmartSqlOptions { get; internal set; }
        public IDbConnectionSessionStore SessionStore { get { return SmartSqlOptions.DbSessionStore; } }
        public IDataSourceFilter DataSourceFilter { get { return SmartSqlOptions.DataSourceFilter; } }
        public IConfigLoader ConfigLoader { get { return SmartSqlOptions.ConfigLoader; } }
        public ICommandExecuter CommandExecuter { get { return SmartSqlOptions.CommandExecuter; } }
        public IDataReaderDeserializerFactory DeserializerFactory { get { return SmartSqlOptions.DataReaderDeserializerFactory; } }
        public ILoggerFactory LoggerFactory { get { return SmartSqlOptions.LoggerFactory; } }
        public ICacheManager CacheManager { get { return SmartSqlOptions.CacheManager; } }
        public ISqlBuilder SqlBuilder { get { return SmartSqlOptions.SqlBuilder; } }
        public SmartSqlMapper(String sqlMapConfigFilePath = Consts.DEFAULT_SMARTSQL_CONFIG_PATH) : this(NoneLoggerFactory.Instance, sqlMapConfigFilePath)
        {

        }
        public SmartSqlMapper(
             ILoggerFactory loggerFactory,
             String sqlMapConfigFilePath = Consts.DEFAULT_SMARTSQL_CONFIG_PATH
        ) : this(new SmartSqlOptions
        {
            LoggerFactory = loggerFactory,
            ConfigPath = sqlMapConfigFilePath
        })
        {

        }

        private readonly IDataReaderDeserializer _dataReaderDeserializer;
        private readonly NestedObjectConverter _nestedObjectConverter;
        public SmartSqlMapper(SmartSqlOptions options)
        {
            SmartSqlOptions = options;
            SmartSqlOptions.Setup();
            _logger = LoggerFactory.CreateLogger<SmartSqlMapper>();
            _dataReaderDeserializer = DeserializerFactory.Create();
            NestedObjectConvertFactory convertFactory = new NestedObjectConvertFactory();
            _nestedObjectConverter = new NestedObjectConverter(convertFactory);
        }
        private void SetupRequestContext(RequestContext context, DataSourceChoice sourceChoice)
        {
            context.Setup(SmartSqlOptions);
            InitDataSource(context, sourceChoice);
        }
        private void InitDataSource(RequestContext context, DataSourceChoice sourceChoice)
        {
            if (context.DataSourceChoice == DataSourceChoice.Unknow)
            {
                context.DataSourceChoice = sourceChoice;
            }
        }

        #region Sync

        private T WrapWithTransaction<T>(RequestContext context, Func<IDbConnectionSession, T> executeFun)
        {
            var transaction = context.Statement?.Transaction;
            if (transaction.HasValue)
            {
                try
                {
                    var dbSession = BeginTransaction(transaction.Value);
                    var result = executeFun(dbSession);
                    CommitTransaction();
                    return result;
                }
                catch (Exception ex)
                {
                    RollbackTransaction();
                    throw ex;
                }
            }
            else
            {
                return executeFun(null);
            }
        }

        public T ExecuteWrap<T>(Func<IDbConnectionSession, T> execute, RequestContext context, DataSourceChoice sourceChoice = DataSourceChoice.Write)
        {
            SetupRequestContext(context, sourceChoice);
            if (CacheManager.TryGet<T>(context, out T cachedResult))
            {
                return cachedResult;
            }
            return WrapWithTransaction<T>(context, (dbSession) =>
             {
                 if (dbSession == null)
                 {
                     var dataSource = DataSourceFilter.Elect(context);
                     dbSession = SessionStore.GetOrAddDbSession(dataSource);
                 }
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
             });
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
                if (result == null)
                {
                    var defaultTypedVal = default(T);
                    if (defaultTypedVal != null) { throw new SmartSqlException($"Null can not convert to {typeof(T)}"); }
                    return defaultTypedVal;
                }
                var resultType = typeof(T);
                var convertType = Nullable.GetUnderlyingType(resultType) ?? resultType;
                return (T)Convert.ChangeType(result, convertType);
            }, context);
        }

        public IEnumerable<T> Query<T>(RequestContext context)
        {
            return ExecuteWrap((dbSession) =>
            {
                var dataReader = CommandExecuter.ExecuteReader(dbSession, context);
                return _dataReaderDeserializer.ToEnumerable<T>(context, dataReader);
            }, context, DataSourceChoice.Read);
        }
        public T QuerySingle<T>(RequestContext context)
        {
            return ExecuteWrap((dbSession) =>
            {
                var dataReader = CommandExecuter.ExecuteReader(dbSession, context);
                return _dataReaderDeserializer.ToSingle<T>(context, dataReader);
            }, context, DataSourceChoice.Read);
        }
        public IMultipleResult FillMultiple(RequestContext context, IMultipleResult multipleResult)
        {
            return ExecuteWrap((dbSession) =>
            {
                IDataReaderWrapper dataReader = null;
                try
                {
                    dataReader = CommandExecuter.ExecuteReader(dbSession, context);
                    return multipleResult.InitData(context, dataReader, _dataReaderDeserializer);
                }
                finally
                {
                    DisposeReader(dataReader);
                }
            }, context, DataSourceChoice.Read);
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
                    DisposeReader(dataReader);
                }

            }, context, DataSourceChoice.Read);
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
                    DisposeReader(dataReader);
                }
            }, context, DataSourceChoice.Read);
        }
        public DbTable GetDbTable(RequestContext context)
        {
            return ExecuteWrap((dbSession) =>
            {
                IDataReader dataReader = null;
                try
                {
                    dataReader = CommandExecuter.ExecuteReader(dbSession, context);
                    return DataReaderConvert.ToDbTable(dataReader);
                }
                finally
                {
                    DisposeReader(dataReader);
                }

            }, context, DataSourceChoice.Read);
        }

        public DbSet GetDbSet(RequestContext context)
        {
            return ExecuteWrap((dbSession) =>
            {
                IDataReader dataReader = null;
                try
                {
                    dataReader = CommandExecuter.ExecuteReader(dbSession, context);
                    return DataReaderConvert.ToDbSet(dataReader);
                }
                finally
                {
                    DisposeReader(dataReader);
                }
            }, context, DataSourceChoice.Read);
        }
        public T GetNested<T>(RequestContext context)
        {
            return ExecuteWrap<T>((dbSession) =>
            {
                IDataReaderWrapper dataReader = null;
                try
                {
                    dataReader = CommandExecuter.ExecuteReader(dbSession, context);
                    return _nestedObjectConverter.ToNested<T>(context, dataReader, _dataReaderDeserializer);
                }
                finally
                {
                    DisposeReader(dataReader);
                }
            }, context, DataSourceChoice.Read);
        }
        #endregion
        #region Async
        private async Task<T> WrapWithTransactionAsync<T>(RequestContext context, Func<IDbConnectionSession, Task<T>> executeFun)
        {
            var transaction = context.Statement?.Transaction;
            if (transaction.HasValue)
            {
                try
                {
                    var dbSession = BeginTransaction(transaction.Value);
                    var result = await executeFun(dbSession);
                    CommitTransaction();
                    return result;
                }
                catch (Exception ex)
                {
                    RollbackTransaction();
                    throw ex;
                }
            }
            else
            {
                return await executeFun(null);
            }
        }
        public async Task<T> ExecuteWrapAsync<T>(Func<IDbConnectionSession, Task<T>> execute, RequestContext context, DataSourceChoice sourceChoice = DataSourceChoice.Write)
        {
            SetupRequestContext(context, sourceChoice);
            if (CacheManager.TryGet<T>(context, out T cachedResult))
            {
                return cachedResult;
            }

            return await WrapWithTransactionAsync<T>(context, async (dbSession) =>
           {
               if (dbSession == null)
               {
                   var dataSource = DataSourceFilter.Elect(context);
                   dbSession = SessionStore.GetOrAddDbSession(dataSource);
               }
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
           });


        }
        public Task<int> ExecuteAsync(RequestContext context)
        {
            return ExecuteWrapAsync((dbSession) =>
          {
              return CommandExecuter.ExecuteNonQueryAsync(dbSession, context);
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
                return await _dataReaderDeserializer.ToEnumerableAsync<T>(context, dataReader);
            }, context, DataSourceChoice.Read);
        }
        public async Task<T> QuerySingleAsync<T>(RequestContext context)
        {
            return await ExecuteWrapAsync(async (dbSession) =>
            {
                var dataReader = await CommandExecuter.ExecuteReaderAsync(dbSession, context);
                return await _dataReaderDeserializer.ToSingleAsync<T>(context, dataReader);
            }, context, DataSourceChoice.Read);
        }

        public async Task<IMultipleResult> FillMultipleAsync(RequestContext context, IMultipleResult multipleResult)
        {
            return await ExecuteWrapAsync(async (dbSession) =>
            {
                IDataReaderWrapper dataReader = null;
                try
                {
                    dataReader = CommandExecuter.ExecuteReader(dbSession, context);
                    return await multipleResult.InitDataAsync(context, dataReader, _dataReaderDeserializer);
                }
                finally
                {
                    DisposeReader(dataReader);
                }
            }, context, DataSourceChoice.Read);
        }

        public async Task<DataTable> GetDataTableAsync(RequestContext context)
        {
            return await ExecuteWrapAsync(async (dbSession) =>
            {
                IDataReaderWrapper dataReader = null;
                try
                {
                    dataReader = await CommandExecuter.ExecuteReaderAsync(dbSession, context);
                    return await DataReaderConvert.ToDataTableAsync(dataReader);
                }
                finally
                {
                    DisposeReader(dataReader);
                }
            }, context, DataSourceChoice.Read);
        }

        public async Task<DataSet> GetDataSetAsync(RequestContext context)
        {
            return await ExecuteWrapAsync(async (dbSession) =>
            {
                IDataReaderWrapper dataReader = null;
                try
                {
                    dataReader = await CommandExecuter.ExecuteReaderAsync(dbSession, context);
                    return await DataReaderConvert.ToDataSetAsync(dataReader);
                }
                finally
                {
                    DisposeReader(dataReader);
                }
            }, context, DataSourceChoice.Read);
        }
        public async Task<DbTable> GetDbTableAsync(RequestContext context)
        {
            return await ExecuteWrapAsync(async (dbSession) =>
            {
                IDataReaderWrapper dataReader = null;
                try
                {
                    dataReader = await CommandExecuter.ExecuteReaderAsync(dbSession, context);
                    return await DataReaderConvert.ToDbTableAsync(dataReader);
                }
                finally
                {
                    DisposeReader(dataReader);
                }
            }, context, DataSourceChoice.Read);
        }

        public async Task<DbSet> GetDbSetAsync(RequestContext context)
        {
            return await ExecuteWrapAsync(async (dbSession) =>
            {
                IDataReaderWrapper dataReader = null;
                try
                {
                    dataReader = await CommandExecuter.ExecuteReaderAsync(dbSession, context);
                    return await DataReaderConvert.ToDbSetAsync(dataReader);
                }
                finally
                {
                    DisposeReader(dataReader);
                }
            }, context, DataSourceChoice.Read);
        }
        public Task<T> GetNestedAsync<T>(RequestContext context)
        {
            return ExecuteWrapAsync<T>(async (dbSession) =>
            {
                IDataReaderWrapper dataReader = null;
                try
                {
                    dataReader = await CommandExecuter.ExecuteReaderAsync(dbSession, context);
                    return _nestedObjectConverter.ToNested<T>(context, dataReader, _dataReaderDeserializer);
                }
                finally
                {
                    DisposeReader(dataReader);
                }
            }, context, DataSourceChoice.Read);
        }
        private void DisposeReader(IDataReader dataReader)
        {
            if (dataReader != null)
            {
                dataReader.Dispose();
                dataReader = null;
            }
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
            InitDataSource(context, DataSourceChoice.Write);
            return BeginTransaction(context, IsolationLevel.Unspecified);
        }

        public IDbConnectionSession BeginTransaction(RequestContext context, IsolationLevel isolationLevel)
        {
            InitDataSource(context, DataSourceChoice.Write);
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
            InitDataSource(context, DataSourceChoice.Read);
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
