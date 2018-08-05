using Microsoft.Extensions.Logging;
using SmartSql.Abstractions;
using SmartSql.Abstractions.Command;
using SmartSql.Abstractions.DbSession;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SmartSql.Command
{
    public class CommandExecuter : ICommandExecuter
    {
        private readonly ILogger<CommandExecuter> _logger;
        private readonly IPreparedCommand _preparedCommand;

        public event OnExecutedHandler OnExecuted;

        public CommandExecuter(
            ILogger<CommandExecuter> logger
            , IPreparedCommand preparedCommand)
        {
            _logger = logger;
            _preparedCommand = preparedCommand;
        }
        #region Sync
        public int ExecuteNonQuery(IDbConnectionSession dbSession, RequestContext context)
        {
            return ExecuteWarp((dbCommand) =>
            {
                return dbCommand.ExecuteNonQuery();
            }, dbSession, context);
        }

        public IDataReaderWrapper ExecuteReader(IDbConnectionSession dbSession, RequestContext context)
        {
            return ExecuteWarp((dbCommand) =>
            {
                var dataReader = dbCommand.ExecuteReader();
                return new DataReaderWrapper(dataReader);
            }, dbSession, context);
        }

        public object ExecuteScalar(IDbConnectionSession dbSession, RequestContext context)
        {
            return ExecuteWarp((dbCommand) =>
             {
                 return dbCommand.ExecuteScalar();
             }, dbSession, context);
        }
        private T ExecuteWarp<T>(Func<IDbCommand, T> excute, IDbConnectionSession dbSession, RequestContext context)
        {
            var dbCommand = _preparedCommand.Prepare(dbSession, context);
            dbSession.OpenConnection();
            T result = excute(dbCommand);
            OnExecuted?.Invoke(this, new OnExecutedEventArgs
            {
                DbSession = dbSession,
                RequestContext = context
            });
            return result;
        }
        #endregion
        #region Async
        public Task<int> ExecuteNonQueryAsync(IDbConnectionSession dbSession, RequestContext context)
        {
            return ExecuteWarpAsync((dbCommand) =>
            {
                return dbCommand.ExecuteNonQueryAsync();
            }, dbSession, context);
        }

        public Task<int> ExecuteNonQueryAsync(IDbConnectionSession dbSession, RequestContext context, CancellationToken cancellationToken)
        {
            return ExecuteWarpAsync((dbCommand) =>
            {
                return dbCommand.ExecuteNonQueryAsync(cancellationToken);
            }, dbSession, context);
        }

        public Task<object> ExecuteScalarAsync(IDbConnectionSession dbSession, RequestContext context)
        {
            return ExecuteWarpAsync((dbCommand) =>
            {
                return dbCommand.ExecuteScalarAsync();
            }, dbSession, context);
        }

        public Task<object> ExecuteScalarAsync(IDbConnectionSession dbSession, RequestContext context, CancellationToken cancellationToken)
        {
            return ExecuteWarpAsync((dbCommand) =>
            {
                return dbCommand.ExecuteScalarAsync(cancellationToken);
            }, dbSession, context);
        }
        public Task<IDataReaderWrapper> ExecuteReaderAsync(IDbConnectionSession dbSession, RequestContext context)
        {
            return ExecuteWarpAsync<IDataReaderWrapper>(async (dbCommand) =>
            {
                var dataReader = await dbCommand.ExecuteReaderAsync();
                return new DataReaderWrapper(dataReader);
            }, dbSession, context);
        }

        public Task<IDataReaderWrapper> ExecuteReaderAsync(IDbConnectionSession dbSession, RequestContext context, CancellationToken cancellationToken)
        {
            return ExecuteWarpAsync<IDataReaderWrapper>(async (dbCommand) =>
            {
                var dataReader = await dbCommand.ExecuteReaderAsync(cancellationToken);
                return new DataReaderWrapper(dataReader);
            }
            , dbSession, context);
        }

        private async Task<T> ExecuteWarpAsync<T>(Func<DbCommand, Task<T>> excute, IDbConnectionSession dbSession, RequestContext context)
        {
            var dbCommand = _preparedCommand.Prepare(dbSession, context);
            await dbSession.OpenConnectionAsync();
            var dbCommandAsync = dbCommand as DbCommand;
            T result = await excute(dbCommandAsync);
            OnExecuted?.Invoke(this, new OnExecutedEventArgs
            {
                DbSession = dbSession,
                RequestContext = context
            });
            return result;
        }
        #endregion
    }
}
