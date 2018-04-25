using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SmartSql.Abstractions.Command
{
    /// <summary>
    /// SQL 命令执行器
    /// </summary>
    public interface ICommandExecuter
    {
        int ExecuteNonQuery(RequestContext context);
        IDataReader ExecuteReader(RequestContext context);
        IDataReader ExecuteReader(RequestContext context, CommandBehavior behavior);
        object ExecuteScalar(RequestContext context);
    }

    public interface ICommandExecuterAsync
    {
        Task<int> ExecuteNonQueryAsync();
        Task<int> ExecuteNonQueryAsync(CancellationToken cancellationToken);

        Task<DbDataReader> ExecuteReaderAsync();
        Task<DbDataReader> ExecuteReaderAsync(CommandBehavior behavior);
        Task<DbDataReader> ExecuteReaderAsync(CommandBehavior behavior, CancellationToken cancellationToken);
        Task<DbDataReader> ExecuteReaderAsync(CancellationToken cancellationToken);

        Task<object> ExecuteScalarAsync();
        Task<object> ExecuteScalarAsync(CancellationToken cancellationToken);

    }
}
