using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SmartSql.Abstractions
{
    /// <summary>
    /// SQL 命令执行器
    /// </summary>
    public interface ICommandExecuter
    {
        int ExecuteNonQuery();
        IDataReader ExecuteReader();
        IDataReader ExecuteReader(CommandBehavior behavior);
        object ExecuteScalar();
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
