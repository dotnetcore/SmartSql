using SmartSql.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SmartSql.Command
{
    /// <summary>
    /// SQL 命令执行器
    /// </summary>
    public interface ICommandExecuter
    {
        int ExecuteNonQuery(ExecutionContext executionContext);
        DataReaderWrapper ExecuteReader(ExecutionContext executionContext);
        object ExecuteScalar(ExecutionContext executionContext);
        DataTable GetDateTable(ExecutionContext executionContext);
        DataSet GetDateSet(ExecutionContext executionContext);
        #region Async
        Task<int> ExecuteNonQueryAsync(ExecutionContext executionContext);
        Task<int> ExecuteNonQueryAsync(ExecutionContext executionContext, CancellationToken cancellationToken);
        Task<DataReaderWrapper> ExecuteReaderAsync(ExecutionContext executionContext);
        Task<DataReaderWrapper> ExecuteReaderAsync(ExecutionContext executionContext, CancellationToken cancellationToken);
        Task<object> ExecuteScalarAsync(ExecutionContext executionContext);
        Task<object> ExecuteScalarAsync(ExecutionContext executionContext, CancellationToken cancellationToken);
        Task<DataTable> GetDateTableAsync(ExecutionContext executionContext);
        Task<DataSet> GetDateSetAsync(ExecutionContext executionContext);
        #endregion
    }
}
