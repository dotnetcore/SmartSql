using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SmartSql.Configuration;
using SmartSql.DataSource;
using SmartSql.Diagnostics;
using SmartSql.Exceptions;

namespace SmartSql.DbSession
{
    public class DefaultDbSession : IDbSession
    {
        private ILogger<DefaultDbSession> _logger;

        public event DbSessionEventHandler Opened;
        public event DbSessionEventHandler TransactionBegan;
        public event DbSessionEventHandler Committed;
        public event DbSessionEventHandler Rollbacked;
        public event DbSessionEventHandler Disposed;

        public Guid Id { get; }
        public AbstractDataSource DataSource { get; private set; }
        public SmartSqlConfig SmartSqlConfig { get; }
        public DbConnection Connection { get; private set; }
        public DbTransaction Transaction { get; private set; }
        public IMiddleware Pipeline => SmartSqlConfig.Pipeline;
        private static readonly DiagnosticListener _diagnosticListener = SmartSqlDiagnosticListenerExtensions.Instance;
        public DefaultDbSession(SmartSqlConfig smartSqlConfig)
        {
            Id = Guid.NewGuid();
            _logger = smartSqlConfig.LoggerFactory.CreateLogger<DefaultDbSession>();
            SmartSqlConfig = smartSqlConfig;
        }
        private void EnsureDataSource()
        {
            if (DataSource == null)
            {
                DataSource = SmartSqlConfig.Database.Write;
            }
        }
        private void EnsureDbConnection()
        {
            EnsureDataSource();
            if (Connection != null) return;
            Connection = DataSource.DbProvider.Factory.CreateConnection();
            Connection.ConnectionString = DataSource.ConnectionString;
        }
        public void SetDataSource(AbstractDataSource dataSource)
        {
            DataSource = dataSource;
        }
        public void Open()
        {
            var operationId = Guid.Empty;
            try
            {
                operationId = _diagnosticListener.WriteDbSessionOpenBefore(this);
                #region Impl
                EnsureDbConnection();
                if (Connection.State == ConnectionState.Closed)
                {
                    if (_logger.IsEnabled(LogLevel.Debug))
                    {
                        _logger.LogDebug($"OpenConnection to {DataSource.Name} .");
                    }
                    Connection.Open();
                }
                Opened?.Invoke(this, DbSessionEventArgs.None);
                #endregion
                _diagnosticListener.WriteDbSessionOpenAfter(operationId, this);
            }
            catch (Exception ex)
            {
                _diagnosticListener.WriteDbSessionOpenError(operationId, this, ex);
                throw new SmartSqlException($"OpenConnection Unable to open connection to { DataSource.Name }.", ex);
            }
        }
        public async Task OpenAsync()
        {
            await OpenAsync(CancellationToken.None);
        }
        public async Task OpenAsync(CancellationToken cancellationToken)
        {
            var operationId = Guid.Empty;
            try
            {
                operationId = _diagnosticListener.WriteDbSessionOpenBefore(this);
                #region Impl
                EnsureDbConnection();
                if (Connection.State == ConnectionState.Closed)
                {
                    if (_logger.IsEnabled(LogLevel.Debug))
                    {
                        _logger.LogDebug($"OpenConnection to {DataSource.Name} .");
                    }
                    await Connection.OpenAsync(cancellationToken);
                }
                Opened?.Invoke(this, DbSessionEventArgs.None);
                #endregion
                _diagnosticListener.WriteDbSessionOpenAfter(operationId, this);
            }
            catch (Exception ex)
            {
                _diagnosticListener.WriteDbSessionOpenError(operationId, this, ex);
                throw new SmartSqlException($"OpenConnection Unable to open connection to { DataSource.Name }.", ex);
            }
        }
        public void BeginTransaction()
        {
            var operationId = Guid.Empty;
            try
            {
                operationId = _diagnosticListener.WriteDbSessionBeginTransactionBefore(this);
                #region Impl
                if (_logger.IsEnabled(LogLevel.Debug))
                {
                    _logger.LogDebug("BeginTransaction.");
                }
                EnsureDataSource();
                Open();
                Transaction = Connection.BeginTransaction();
                TransactionBegan?.Invoke(this, DbSessionEventArgs.None);
                #endregion
                _diagnosticListener.WriteDbSessionBeginTransactionAfter(operationId, this);
            }
            catch (Exception ex)
            {
                _diagnosticListener.WriteDbSessionBeginTransactionError(operationId, this, ex);
                throw ex;
            }
        }
        public void BeginTransaction(IsolationLevel isolationLevel)
        {
            var operationId = Guid.Empty;
            try
            {
                operationId = _diagnosticListener.WriteDbSessionBeginTransactionBefore(this);
                #region Impl
                if (_logger.IsEnabled(LogLevel.Debug))
                {
                    _logger.LogDebug("BeginTransaction.");
                }
                EnsureDataSource();
                Open();
                Transaction = Connection.BeginTransaction(isolationLevel);
                TransactionBegan?.Invoke(this, DbSessionEventArgs.None);
                #endregion
                _diagnosticListener.WriteDbSessionBeginTransactionAfter(operationId, this);
            }
            catch (Exception ex)
            {
                _diagnosticListener.WriteDbSessionBeginTransactionError(operationId, this, ex);
                throw ex;
            }
        }

        public void CommitTransaction()
        {
            var operationId = Guid.Empty;
            try
            {
                operationId = _diagnosticListener.WriteDbSessionCommitBefore(this);
                #region Impl
                if (_logger.IsEnabled(LogLevel.Debug))
                {
                    _logger.LogDebug("CommitTransaction.");
                }
                if (Transaction == null)
                {
                    if (_logger.IsEnabled(LogLevel.Error))
                    {
                        _logger.LogError("Before CommitTransaction,Please BeginTransaction first!");
                    }
                    throw new SmartSqlException("Before CommitTransaction,Please BeginTransaction first!");
                }
                Transaction.Commit();
                Committed?.Invoke(this, DbSessionEventArgs.None);
                #endregion
                _diagnosticListener.WriteDbSessionCommitAfter(operationId, this);
            }
            catch (Exception ex)
            {
                _diagnosticListener.WriteDbSessionCommitError(operationId, this, ex);
                throw ex;
            }
        }

        public void RollbackTransaction()
        {
            var operationId = Guid.Empty;
            try
            {
                operationId = _diagnosticListener.WriteDbSessionRollbackBefore(this);
                #region Impl
                if (_logger.IsEnabled(LogLevel.Debug))
                {
                    _logger.LogDebug("RollbackTransaction .");
                }
                if (Transaction == null)
                {
                    throw new SmartSqlException("Before RollbackTransaction,Please BeginTransaction first!");
                }
                Transaction.Rollback();
                Rollbacked?.Invoke(this, DbSessionEventArgs.None);
                #endregion
                _diagnosticListener.WriteDbSessionRollbackAfter(operationId, this);
            }
            catch (Exception ex)
            {
                _diagnosticListener.WriteDbSessionRollbackError(operationId, this, ex);
                throw ex;
            }
        }

        public void Dispose()
        {
            var operationId = Guid.Empty;
            try
            {
                operationId = _diagnosticListener.WriteDbSessionDisposeBefore(this);
                #region Impl
                if (_logger.IsEnabled(LogLevel.Debug))
                {
                    _logger.LogDebug($"Dispose. ");
                }

                if (Connection != null)
                {
                    Connection.Close();
                    Connection.Dispose();
                    Connection = null;
                }

                if (Transaction != null)
                {
                    Transaction.Dispose();
                    Transaction = null;
                }
                Disposed?.Invoke(this, DbSessionEventArgs.None);
                #endregion
                _diagnosticListener.WriteDbSessionDisposeAfter(operationId, this);
            }
            catch (Exception ex)
            {
                _diagnosticListener.WriteDbSessionDisposeError(operationId, this, ex);
                throw ex;
            }
        }
        public ExecutionContext Invoke<TResult>(RequestContext requestContext)
        {
            var operationId = Guid.Empty;
            var executionContext = new ExecutionContext
            {
                Request = requestContext,
                SmartSqlConfig = SmartSqlConfig,
                DbSession = this,
            };
            try
            {
                operationId = _diagnosticListener.WriteDbSessionInvokeBefore(executionContext);
                #region Impl
                switch (executionContext.Type)
                {
                    case ExecutionType.Execute:
                    case ExecutionType.ExecuteScalar:
                    case ExecutionType.QuerySingle:
                    case ExecutionType.GetDataSet:
                    case ExecutionType.GetDataTable:
                        {
                            executionContext.Result = new SingleResultContext<TResult>();
                            break;
                        }
                    case ExecutionType.Query:
                        {
                            executionContext.Result = new ListResultContext<TResult>();
                            break;
                        }
                }
                requestContext.ExecutionContext = executionContext;
                Pipeline.Invoke<TResult>(executionContext);
                #endregion
                _diagnosticListener.WriteDbSessionInvokeAfter(operationId, executionContext);
                return executionContext;
            }
            catch (Exception ex)
            {
                _diagnosticListener.WriteDbSessionInvokeError(operationId, executionContext, ex);
                throw ex;
            }
        }
        public async Task<ExecutionContext> InvokeAsync<TResult>(RequestContext requestContext)
        {
            Guid operationId = Guid.Empty;
            var executionContext = new ExecutionContext
            {
                Request = requestContext,
                SmartSqlConfig = SmartSqlConfig,
                DbSession = this,
            };
            try
            {
                operationId = _diagnosticListener.WriteDbSessionInvokeBefore(executionContext);
                #region Impl
                switch (executionContext.Type)
                {
                    case ExecutionType.Execute:
                    case ExecutionType.ExecuteScalar:
                    case ExecutionType.QuerySingle:
                    case ExecutionType.GetDataSet:
                    case ExecutionType.GetDataTable:
                        {
                            executionContext.Result = new SingleResultContext<TResult>();
                            break;
                        }
                    case ExecutionType.Query:
                        {
                            executionContext.Result = new ListResultContext<TResult>();
                            break;
                        }
                }
                requestContext.ExecutionContext = executionContext;
                await Pipeline.InvokeAsync<TResult>(executionContext);
                #endregion
                _diagnosticListener.WriteDbSessionInvokeAfter(operationId, executionContext);
                return executionContext;
            }
            catch (Exception ex)
            {
                _diagnosticListener.WriteDbSessionInvokeError(operationId, executionContext, ex);
                throw ex;
            }
        }
        public int Execute(RequestContext requestContext)
        {
            requestContext.ExecutionType = ExecutionType.Execute;
            var executionContext = Invoke<int>(requestContext);
            return ((SingleResultContext<int>)executionContext.Result).Data;
        }

        public TResult ExecuteScalar<TResult>(RequestContext requestContext)
        {
            requestContext.ExecutionType = ExecutionType.ExecuteScalar;
            var executionContext = Invoke<TResult>(requestContext);
            return ((SingleResultContext<TResult>)executionContext.Result).Data;
        }
        public TResult QuerySingle<TResult>(RequestContext requestContext)
        {
            requestContext.ExecutionType = ExecutionType.QuerySingle;
            var executionContext = Invoke<TResult>(requestContext);
            return ((SingleResultContext<TResult>)executionContext.Result).Data;
        }

        public DataSet GetDataSet(RequestContext requestContext)
        {
            requestContext.ExecutionType = ExecutionType.GetDataSet;
            var executionContext = Invoke<DataSet>(requestContext);
            return executionContext.Result.GetData() as DataSet;
        }

        public DataTable GetDataTable(RequestContext requestContext)
        {
            requestContext.ExecutionType = ExecutionType.GetDataTable;
            var executionContext = Invoke<DataTable>(requestContext);
            return executionContext.Result.GetData() as DataTable;
        }
        public IEnumerable<TResult> Query<TResult>(RequestContext requestContext)
        {
            requestContext.ExecutionType = ExecutionType.Query;
            var executionContext = Invoke<TResult>(requestContext);
            return executionContext.Result.GetData() as IEnumerable<TResult>;
        }

        public async Task<int> ExecuteAsync(RequestContext requestContext)
        {
            requestContext.ExecutionType = ExecutionType.Execute;
            var executionContext = await InvokeAsync<int>(requestContext);
            return ((SingleResultContext<int>)executionContext.Result).Data;
        }

        public async Task<TResult> ExecuteScalarAsync<TResult>(RequestContext requestContext)
        {
            requestContext.ExecutionType = ExecutionType.ExecuteScalar;
            var executionContext = await InvokeAsync<TResult>(requestContext);
            return ((SingleResultContext<TResult>)executionContext.Result).Data;
        }

        public async Task<IEnumerable<TResult>> QueryAsync<TResult>(RequestContext requestContext)
        {
            requestContext.ExecutionType = ExecutionType.Query;
            var executionContext = await InvokeAsync<TResult>(requestContext);
            return executionContext.Result.GetData() as IEnumerable<TResult>;
        }

        public async Task<TResult> QuerySingleAsync<TResult>(RequestContext requestContext)
        {
            requestContext.ExecutionType = ExecutionType.QuerySingle;
            var executionContext = await InvokeAsync<TResult>(requestContext);
            return ((SingleResultContext<TResult>)executionContext.Result).Data;
        }

        public async Task<DataSet> GetDataSetAsync(RequestContext requestContext)
        {
            requestContext.ExecutionType = ExecutionType.GetDataSet;
            var executionContext = await InvokeAsync<DataSet>(requestContext);
            return executionContext.Result.GetData() as DataSet;
        }

        public async Task<DataTable> GetDataTableAsync(RequestContext requestContext)
        {
            requestContext.ExecutionType = ExecutionType.GetDataTable;
            var executionContext = await InvokeAsync<DataTable>(requestContext);
            return executionContext.Result.GetData() as DataTable;
        }
    }
}