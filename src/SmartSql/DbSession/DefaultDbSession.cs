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
        private readonly ILogger<DefaultDbSession> _logger;

        public event DbSessionEventHandler Opened;
        public event DbSessionEventHandler TransactionBegan;
        public event DbSessionEventHandler Committed;
        public event DbSessionEventHandler Rollbacked;
        public event DbSessionEventHandler Disposed;
        public event DbSessionInvokedEventHandler Invoked;

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
            Connection = DataSource.CreateConnection();
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
                throw new SmartSqlException($"OpenConnection Unable to open connection to {DataSource.Name}.", ex);
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
                throw new SmartSqlException($"OpenConnection Unable to open connection to {DataSource.Name}.", ex);
            }
        }

        public DbTransaction BeginTransaction()
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

                Open();
                Transaction = Connection.BeginTransaction();
                TransactionBegan?.Invoke(this, DbSessionEventArgs.None);

                #endregion

                _diagnosticListener.WriteDbSessionBeginTransactionAfter(operationId, this);
                return Transaction;
            }
            catch (Exception ex)
            {
                _diagnosticListener.WriteDbSessionBeginTransactionError(operationId, this, ex);
                throw;
            }
        }

        public DbTransaction BeginTransaction(IsolationLevel isolationLevel)
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

                Open();
                Transaction = Connection.BeginTransaction(isolationLevel);
                TransactionBegan?.Invoke(this, DbSessionEventArgs.None);

                #endregion

                _diagnosticListener.WriteDbSessionBeginTransactionAfter(operationId, this);
                return Transaction;
            }
            catch (Exception ex)
            {
                _diagnosticListener.WriteDbSessionBeginTransactionError(operationId, this, ex);
                throw;
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
                throw;
            }
            finally
            {
                ReleaseTransaction();
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
                    if (_logger.IsEnabled(LogLevel.Warning))
                    {
                        _logger.LogWarning("Before RollbackTransaction,Please BeginTransaction first!");
                    }

                    _diagnosticListener.WriteDbSessionRollbackAfter(operationId, this);
                    return;
                }

                Transaction.Rollback();
                Rollbacked?.Invoke(this, DbSessionEventArgs.None);

                #endregion

                _diagnosticListener.WriteDbSessionRollbackAfter(operationId, this);
            }
            catch (Exception ex)
            {
                _diagnosticListener.WriteDbSessionRollbackError(operationId, this, ex);
                throw;
            }
            finally
            {
                ReleaseTransaction();
            }
        }

        private void ReleaseTransaction()
        {
            Transaction.Dispose();
            Transaction = null;
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

                if (Transaction != null)
                {
                    RollbackTransaction();
                }

                if (Connection != null)
                {
                    Connection.Close();
                    Connection.Dispose();
                    Connection = null;
                }

                Disposed?.Invoke(this, DbSessionEventArgs.None);

                #endregion

                _diagnosticListener.WriteDbSessionDisposeAfter(operationId, this);
            }
            catch (Exception ex)
            {
                _diagnosticListener.WriteDbSessionDisposeError(operationId, this, ex);
                throw;
            }
        }

        public ExecutionContext Invoke<TResult>(AbstractRequestContext requestContext)
        {
            Stopwatch stopwatch = null;
            var operationId = Guid.Empty;
            var executionContext = new ExecutionContext
            {
                Request = requestContext,
                SmartSqlConfig = SmartSqlConfig,
                DbSession = this,
            };
            try
            {
                if (_logger.IsEnabled(LogLevel.Debug))
                {
                    stopwatch = Stopwatch.StartNew();
                }

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
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                requestContext.ExecutionContext = executionContext;
                Pipeline.Invoke<TResult>(executionContext);
                Invoked?.Invoke(this, new DbSessionInvokedEventArgs {ExecutionContext = executionContext});

                #endregion

                _diagnosticListener.WriteDbSessionInvokeAfter(operationId, executionContext);
                return executionContext;
            }
            catch (Exception ex)
            {
                _diagnosticListener.WriteDbSessionInvokeError(operationId, executionContext, ex);
                throw;
            }
            finally
            {
                if (_logger.IsEnabled(LogLevel.Debug))
                {
                    _logger.LogDebug(
                        $"Statement.Id:{requestContext.FullSqlId} Invoke Taken:{stopwatch?.ElapsedMilliseconds}.");
                }
            }
        }

        public async Task<ExecutionContext> InvokeAsync<TResult>(AbstractRequestContext requestContext)
        {
            Stopwatch stopwatch = null;
            Guid operationId = Guid.Empty;
            var executionContext = new ExecutionContext
            {
                Request = requestContext,
                SmartSqlConfig = SmartSqlConfig,
                DbSession = this,
            };
            try
            {
                if (_logger.IsEnabled(LogLevel.Debug))
                {
                    stopwatch = Stopwatch.StartNew();
                }

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
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                requestContext.ExecutionContext = executionContext;
                await Pipeline.InvokeAsync<TResult>(executionContext);
                Invoked?.Invoke(this, new DbSessionInvokedEventArgs {ExecutionContext = executionContext});

                #endregion

                _diagnosticListener.WriteDbSessionInvokeAfter(operationId, executionContext);
                return executionContext;
            }
            catch (Exception ex)
            {
                _diagnosticListener.WriteDbSessionInvokeError(operationId, executionContext, ex);
                throw;
            }
            finally
            {
                if (_logger.IsEnabled(LogLevel.Debug))
                {
                    _logger.LogDebug(
                        $"Statement.Id:{requestContext.FullSqlId} Invoke Taken:{stopwatch?.ElapsedMilliseconds}.");
                }
            }
        }

        public int Execute(AbstractRequestContext requestContext)
        {
            requestContext.ExecutionType = ExecutionType.Execute;
            var executionContext = Invoke<int>(requestContext);
            return ((SingleResultContext<int>) executionContext.Result).Data;
        }

        public TResult ExecuteScalar<TResult>(AbstractRequestContext requestContext)
        {
            requestContext.ExecutionType = ExecutionType.ExecuteScalar;
            var executionContext = Invoke<TResult>(requestContext);
            return ((SingleResultContext<TResult>) executionContext.Result).Data;
        }

        public TResult QuerySingle<TResult>(AbstractRequestContext requestContext)
        {
            requestContext.ExecutionType = ExecutionType.QuerySingle;
            var executionContext = Invoke<TResult>(requestContext);
            return ((SingleResultContext<TResult>) executionContext.Result).Data;
        }

        public DataSet GetDataSet(AbstractRequestContext requestContext)
        {
            requestContext.ExecutionType = ExecutionType.GetDataSet;
            var executionContext = Invoke<DataSet>(requestContext);
            return executionContext.Result.GetData() as DataSet;
        }

        public DataTable GetDataTable(AbstractRequestContext requestContext)
        {
            requestContext.ExecutionType = ExecutionType.GetDataTable;
            var executionContext = Invoke<DataTable>(requestContext);
            return executionContext.Result.GetData() as DataTable;
        }

        public IList<TResult> Query<TResult>(AbstractRequestContext requestContext)
        {
            requestContext.ExecutionType = ExecutionType.Query;
            var executionContext = Invoke<TResult>(requestContext);
            return executionContext.Result.GetData() as IList<TResult>;
        }

        public async Task<int> ExecuteAsync(AbstractRequestContext requestContext)
        {
            requestContext.ExecutionType = ExecutionType.Execute;
            var executionContext = await InvokeAsync<int>(requestContext);
            return ((SingleResultContext<int>) executionContext.Result).Data;
        }

        public async Task<TResult> ExecuteScalarAsync<TResult>(AbstractRequestContext requestContext)
        {
            requestContext.ExecutionType = ExecutionType.ExecuteScalar;
            var executionContext = await InvokeAsync<TResult>(requestContext);
            return ((SingleResultContext<TResult>) executionContext.Result).Data;
        }

        public async Task<IList<TResult>> QueryAsync<TResult>(AbstractRequestContext requestContext)
        {
            requestContext.ExecutionType = ExecutionType.Query;
            var executionContext = await InvokeAsync<TResult>(requestContext);
            return executionContext.Result.GetData() as IList<TResult>;
        }

        public async Task<TResult> QuerySingleAsync<TResult>(AbstractRequestContext requestContext)
        {
            requestContext.ExecutionType = ExecutionType.QuerySingle;
            var executionContext = await InvokeAsync<TResult>(requestContext);
            return ((SingleResultContext<TResult>) executionContext.Result).Data;
        }

        public async Task<DataSet> GetDataSetAsync(AbstractRequestContext requestContext)
        {
            requestContext.ExecutionType = ExecutionType.GetDataSet;
            var executionContext = await InvokeAsync<DataSet>(requestContext);
            return executionContext.Result.GetData() as DataSet;
        }

        public async Task<DataTable> GetDataTableAsync(AbstractRequestContext requestContext)
        {
            requestContext.ExecutionType = ExecutionType.GetDataTable;
            var executionContext = await InvokeAsync<DataTable>(requestContext);
            return executionContext.Result.GetData() as DataTable;
        }
    }
}