using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SmartSql.Configuration;
using SmartSql.DbSession;
using SmartSql.Exceptions;

namespace SmartSql
{
    public class SqlMapper : ISqlMapper
    {
        private readonly ILogger _logger;
        public SmartSqlConfig SmartSqlConfig { get; }
        public IDbSessionStore SessionStore { get; }

        public SqlMapper(SmartSqlConfig smartSqlConfig)
        {
            _logger = smartSqlConfig.LoggerFactory.CreateLogger<SqlMapper>();
            SmartSqlConfig = smartSqlConfig;
            SessionStore = smartSqlConfig.SessionStore;
        }

        public DbTransaction BeginTransaction()
        {
            if (SessionStore.LocalSession != null)
            {
                throw new SmartSqlException(
                    "SmartSqlMapper could not invoke BeginTransaction(). A LocalSession is already existed.");
            }

            return SessionStore.Open().BeginTransaction();
        }

        public DbTransaction BeginTransaction(IsolationLevel isolationLevel)
        {
            if (SessionStore.LocalSession != null)
            {
                throw new SmartSqlException(
                    "SmartSqlMapper could not invoke BeginTransaction(). A LocalSession is already existed.");
            }

            return SessionStore.Open().BeginTransaction(isolationLevel);
        }

        public void CommitTransaction()
        {
            var session = SessionStore.LocalSession;
            if (session == null)
            {
                throw new SmartSqlException(
                    "SmartSqlMapper could not invoke CommitTransaction(). No Transaction was started. Call BeginTransaction() first.");
            }

            try
            {
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
                if (_logger.IsEnabled(LogLevel.Warning))
                {
                    _logger.LogWarning("Before RollbackTransaction,Please BeginTransaction first!");
                }
                return;
            }

            try
            {
                session.RollbackTransaction();
            }
            finally
            {
                SessionStore.Dispose();
            }
        }

        private TResult ExecuteImpl<TResult>(Func<IDbSession, TResult> executeFunc)
        {
            //Session 释放原则：谁开启，谁释放
            var dbSession = SessionStore.LocalSession;
            var ownSession = dbSession == null;
            try
            {
                if (ownSession)
                {
                    dbSession = SessionStore.Open();
                }

                return executeFunc(dbSession);
            }
            finally
            {
                if (ownSession)
                {
                    SessionStore.Dispose();
                }
            }
        }

        public int Execute(AbstractRequestContext requestContext)
        {
            return ExecuteImpl(dbSession => dbSession.Execute(requestContext));
        }

        public T ExecuteScalar<T>(AbstractRequestContext requestContext)
        {
            return ExecuteImpl(dbSession => dbSession.ExecuteScalar<T>(requestContext));
        }

        public IList<T> Query<T>(AbstractRequestContext requestContext)
        {
            return ExecuteImpl(dbSession => dbSession.Query<T>(requestContext));
        }

        public T QuerySingle<T>(AbstractRequestContext requestContext)
        {
            return ExecuteImpl(dbSession => dbSession.QuerySingle<T>(requestContext));
        }

        public DataSet GetDataSet(AbstractRequestContext requestContext)
        {
            return ExecuteImpl(dbSession => dbSession.GetDataSet(requestContext));
        }

        public DataTable GetDataTable(AbstractRequestContext requestContext)
        {
            return ExecuteImpl(dbSession => dbSession.GetDataTable(requestContext));
        }

        private async Task<TResult> ExecuteImplAsync<TResult>(Func<IDbSession, Task<TResult>> executeFunc)
        {
            //Session 释放原则：谁开启，谁释放
            var dbSession = SessionStore.LocalSession;
            var ownSession = dbSession == null;
            try
            {
                if (ownSession)
                {
                    dbSession = SessionStore.Open();
                }

                return await executeFunc(dbSession);
            }
            finally
            {
                if (ownSession)
                {
                    SessionStore.Dispose();
                }
            }
        }

        public async Task<int> ExecuteAsync(AbstractRequestContext requestContext)
        {
            return await ExecuteImplAsync(async dbSession => await dbSession.ExecuteAsync(requestContext));
        }

        public async Task<TResult> ExecuteScalarAsync<TResult>(AbstractRequestContext requestContext)
        {
            return await ExecuteImplAsync(
                async dbSession => await dbSession.ExecuteScalarAsync<TResult>(requestContext));
        }

        public async Task<IList<TResult>> QueryAsync<TResult>(AbstractRequestContext requestContext)
        {
            return await ExecuteImplAsync(async dbSession => await dbSession.QueryAsync<TResult>(requestContext));
        }

        public async Task<TResult> QuerySingleAsync<TResult>(AbstractRequestContext requestContext)
        {
            return await ExecuteImplAsync(dbSession => dbSession.QuerySingleAsync<TResult>(requestContext));
        }

        public async Task<DataSet> GetDataSetAsync(AbstractRequestContext requestContext)
        {
            return await ExecuteImplAsync(async dbSession => await dbSession.GetDataSetAsync(requestContext));
        }

        public async Task<DataTable> GetDataTableAsync(AbstractRequestContext requestContext)
        {
            return await ExecuteImplAsync(async dbSession => await dbSession.GetDataTableAsync(requestContext));
        }

        public void Dispose()
        {
            SessionStore.Dispose();
        }
    }
}