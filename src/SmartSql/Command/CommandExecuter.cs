using SmartSql.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SmartSql.Diagnostics;

namespace SmartSql.Command
{
    public class DbCommandCreatedEventArgs : EventArgs
    {
        public DbCommand DbCommand { get; set; }
    }

    public delegate void DbCommandCreatedEventHandler(object sender, DbCommandCreatedEventArgs eventArgs);

    public class CommandExecuter : ICommandExecuter
    {
        private readonly ILogger<CommandExecuter> _logger;

        public event DbCommandCreatedEventHandler DbCommandCreated;

        public CommandExecuter(ILogger<CommandExecuter> logger)
        {
            _logger = logger;
        }

        private static readonly DiagnosticListener _diagnosticListener = SmartSqlDiagnosticListenerExtensions.Instance;

        private DbCommand CreateCmd(ExecutionContext executionContext)
        {
            var dbSession = executionContext.DbSession;
            var dbCmd = dbSession.Connection.CreateCommand();
            dbCmd.CommandType = executionContext.Request.CommandType;
            dbCmd.Transaction = dbSession.Transaction;
            dbCmd.CommandText = executionContext.Request.RealSql;

            if (executionContext.Request.CommandTimeout.HasValue)
            {
                dbCmd.CommandTimeout = executionContext.Request.CommandTimeout.Value;
            }

            foreach (var dbParam in executionContext.Request.Parameters.DbParameters.Values)
            {
                dbCmd.Parameters.Add(dbParam);
            }

            DbCommandCreated?.Invoke(this, new DbCommandCreatedEventArgs
            {
                DbCommand = dbCmd
            });

            return dbCmd;
        }

        private TResult ExecuteWrap<TResult>(Func<TResult> executeImpl, ExecutionContext executionContext
            , [CallerMemberName] string operation = "")
        {
            Stopwatch stopwatch = null;
            var operationId = Guid.Empty;
            try
            {
                if (_logger.IsEnabled(LogLevel.Debug))
                {
                    stopwatch = Stopwatch.StartNew();
                }

                operationId = _diagnosticListener.WriteCommandExecuterExecuteBefore(executionContext, operation);
                var result = executeImpl();
                _diagnosticListener.WriteCommandExecuterExecuteAfter(operationId, executionContext, operation);
                return result;
            }
            catch (Exception ex)
            {
                _diagnosticListener.WriteCommandExecuterExecuteError(operationId, executionContext, ex, operation);
                throw;
            }
            finally
            {
                if (_logger.IsEnabled(LogLevel.Debug))
                {
                    _logger.LogDebug(
                        $"Operation:{operation} Statement.Id:{executionContext.Request.FullSqlId} Execute Taken:{stopwatch?.ElapsedMilliseconds}.");
                }
            }
        }

        public DataTable GetDateTable(ExecutionContext executionContext)
        {
            return ExecuteWrap(() =>
            {
                DataTable dataTable = new DataTable();
                executionContext.DbSession.Open();
                DbCommand dbCmd = CreateCmd(executionContext);
                var dataAdapter = executionContext.SmartSqlConfig.Database.DbProvider.Factory.CreateDataAdapter();
                dataAdapter.SelectCommand = dbCmd;
                dataAdapter.Fill(dataTable);
                return dataTable;
            }, executionContext);
        }

        public DataSet GetDateSet(ExecutionContext executionContext)
        {
            return ExecuteWrap(() =>
            {
                DataSet dataSet = new DataSet();
                executionContext.DbSession.Open();
                DbCommand dbCmd = CreateCmd(executionContext);
                var dataAdapter = executionContext.SmartSqlConfig.Database.DbProvider.Factory.CreateDataAdapter();
                dataAdapter.SelectCommand = dbCmd;
                dataAdapter.Fill(dataSet);
                return dataSet;
            }, executionContext);
        }

        public int ExecuteNonQuery(ExecutionContext executionContext)
        {
            return ExecuteWrap(() =>
            {
                executionContext.DbSession.Open();
                DbCommand dbCmd = CreateCmd(executionContext);
                return dbCmd.ExecuteNonQuery();
            }, executionContext);
        }

        public DataReaderWrapper ExecuteReader(ExecutionContext executionContext)
        {
            return ExecuteWrap(() =>
            {
                executionContext.DbSession.Open();
                DbCommand dbCmd = CreateCmd(executionContext);
                var dbReader = dbCmd.ExecuteReader();
                return new DataReaderWrapper(dbReader);
            }, executionContext);
        }

        public object ExecuteScalar(ExecutionContext executionContext)
        {
            return ExecuteWrap(() =>
            {
                executionContext.DbSession.Open();
                DbCommand dbCmd = CreateCmd(executionContext);
                return dbCmd.ExecuteScalar();
            }, executionContext);
        }

        private async Task<TResult> ExecuteWrapAsync<TResult>(Func<Task<TResult>> executeImplAsync,
            ExecutionContext executionContext
            , [CallerMemberName] string operation = "")
        {
            Stopwatch stopwatch = null;
            var operationId = Guid.Empty;
            try
            {
                if (_logger.IsEnabled(LogLevel.Debug))
                {
                    stopwatch = Stopwatch.StartNew();
                }

                operationId = _diagnosticListener.WriteCommandExecuterExecuteBefore(executionContext, operation);
                var result = await executeImplAsync();
                _diagnosticListener.WriteCommandExecuterExecuteAfter(operationId, executionContext, operation);
                return result;
            }
            catch (Exception ex)
            {
                _diagnosticListener.WriteCommandExecuterExecuteError(operationId, executionContext, ex, operation);
                throw;
            }
            finally
            {
                if (_logger.IsEnabled(LogLevel.Debug))
                {
                    _logger.LogDebug(
                        $"Operation:{operation} Statement.Id:{executionContext.Request.FullSqlId} Execute Taken:{stopwatch?.ElapsedMilliseconds}.");
                }
            }
        }

        public async Task<object> ExecuteScalarAsync(ExecutionContext executionContext)
        {
            return await ExecuteWrapAsync(async () =>
            {
                await executionContext.DbSession.OpenAsync();
                DbCommand dbCmd = CreateCmd(executionContext);
                return await dbCmd.ExecuteScalarAsync();
            }, executionContext);
        }

        public async Task<object> ExecuteScalarAsync(ExecutionContext executionContext,
            CancellationToken cancellationToken)
        {
            return await ExecuteWrapAsync(async () =>
            {
                await executionContext.DbSession.OpenAsync(cancellationToken);
                DbCommand dbCmd = CreateCmd(executionContext);
                return await dbCmd.ExecuteScalarAsync(cancellationToken);
            }, executionContext);
        }

        public async Task<DataTable> GetDateTableAsync(ExecutionContext executionContext)
        {
            return await ExecuteWrapAsync(async () =>
            {
                DataTable dataTable = new DataTable();
                await executionContext.DbSession.OpenAsync();
                DbCommand dbCmd = CreateCmd(executionContext);
                var dataAdapter = executionContext.SmartSqlConfig.Database.DbProvider.Factory.CreateDataAdapter();
                dataAdapter.SelectCommand = dbCmd;
                dataAdapter.Fill(dataTable);
                return dataTable;
            }, executionContext);
        }

        public async Task<DataSet> GetDateSetAsync(ExecutionContext executionContext)
        {
            return await ExecuteWrapAsync(async () =>
            {
                DataSet dataSet = new DataSet();
                await executionContext.DbSession.OpenAsync();
                DbCommand dbCmd = CreateCmd(executionContext);
                var dataAdapter = executionContext.SmartSqlConfig.Database.DbProvider.Factory.CreateDataAdapter();
                dataAdapter.SelectCommand = dbCmd;
                dataAdapter.Fill(dataSet);
                return dataSet;
            }, executionContext);
        }

        public async Task<DataReaderWrapper> ExecuteReaderAsync(ExecutionContext executionContext)
        {
            return await ExecuteWrapAsync(async () =>
            {
                await executionContext.DbSession.OpenAsync();
                DbCommand dbCmd = CreateCmd(executionContext);
                var dbReader = await dbCmd.ExecuteReaderAsync();
                return new DataReaderWrapper(dbReader);
            }, executionContext);
        }

        public async Task<DataReaderWrapper> ExecuteReaderAsync(ExecutionContext executionContext,
            CancellationToken cancellationToken)
        {
            return await ExecuteWrapAsync(async () =>
            {
                await executionContext.DbSession.OpenAsync(cancellationToken);
                DbCommand dbCmd = CreateCmd(executionContext);
                var dbReader = await dbCmd.ExecuteReaderAsync(cancellationToken);
                return new DataReaderWrapper(dbReader);
            }, executionContext);
        }

        public async Task<int> ExecuteNonQueryAsync(ExecutionContext executionContext)
        {
            return await ExecuteWrapAsync(async () =>
            {
                await executionContext.DbSession.OpenAsync();
                DbCommand dbCmd = CreateCmd(executionContext);
                return await dbCmd.ExecuteNonQueryAsync();
            }, executionContext);
        }

        public async Task<int> ExecuteNonQueryAsync(ExecutionContext executionContext,
            CancellationToken cancellationToken)
        {
            return await ExecuteWrapAsync(async () =>
            {
                await executionContext.DbSession.OpenAsync(cancellationToken);
                DbCommand dbCmd = CreateCmd(executionContext);
                return await dbCmd.ExecuteNonQueryAsync(cancellationToken);
            }, executionContext);
        }
    }
}