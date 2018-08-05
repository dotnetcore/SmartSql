using SmartSql.Abstractions.DbSession;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SmartSql.Abstractions.Command
{
    public class OnExecutedEventArgs : EventArgs
    {
        public RequestContext RequestContext { get; set; }
        public IDbConnectionSession DbSession { get; set; }
    }
    public delegate void OnExecutedHandler(object sender, OnExecutedEventArgs eventArgs);

    /// <summary>
    /// SQL 命令执行器
    /// </summary>
    public interface ICommandExecuter : ICommandExecuterAsync
    {
        event OnExecutedHandler OnExecuted;
        int ExecuteNonQuery(IDbConnectionSession dbSession, RequestContext context);
        IDataReaderWrapper ExecuteReader(IDbConnectionSession dbSession, RequestContext context);
        object ExecuteScalar(IDbConnectionSession dbSession, RequestContext context);
    }

    public interface ICommandExecuterAsync
    {
        Task<int> ExecuteNonQueryAsync(IDbConnectionSession dbSession, RequestContext context);
        Task<int> ExecuteNonQueryAsync(IDbConnectionSession dbSession, RequestContext context, CancellationToken cancellationToken);
        Task<IDataReaderWrapper> ExecuteReaderAsync(IDbConnectionSession dbSession, RequestContext context);
        Task<IDataReaderWrapper> ExecuteReaderAsync(IDbConnectionSession dbSession, RequestContext context, CancellationToken cancellationToken);
        Task<object> ExecuteScalarAsync(IDbConnectionSession dbSession, RequestContext context);
        Task<object> ExecuteScalarAsync(IDbConnectionSession dbSession, RequestContext context, CancellationToken cancellationToken);
    }
}
