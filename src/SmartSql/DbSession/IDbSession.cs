using SmartSql.Configuration;
using SmartSql.DataSource;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SmartSql.DbSession
{
    public class DbSessionEventArgs : EventArgs
    {
        public static readonly DbSessionEventArgs None = new DbSessionEventArgs();
    }
    public delegate void DbSessionEventHandler(object sender, DbSessionEventArgs eventArgs);
    public class DbSessionInvokedEventArgs : EventArgs
    {
        public ExecutionContext ExecutionContext { get; set; }
    }
    public delegate void DbSessionInvokedEventHandler(object sender, DbSessionInvokedEventArgs eventArgs);

    public interface IDbSession : ITransaction, IDisposable
    {
        event DbSessionEventHandler Opened;
        event DbSessionEventHandler TransactionBegan;
        event DbSessionEventHandler Committed;
        event DbSessionEventHandler Rollbacked;
        event DbSessionEventHandler Disposed;
        event DbSessionInvokedEventHandler Invoked;
        
        Guid Id { get; }
        DbTransaction Transaction { get; }
        DbConnection Connection { get; }
        AbstractDataSource DataSource { get; }
        SmartSqlConfig SmartSqlConfig { get; }
        void SetDataSource(AbstractDataSource dataSource);
        void Open();
        Task OpenAsync();
        Task OpenAsync(CancellationToken cancellationToken);
        ExecutionContext Invoke<TResult>(AbstractRequestContext requestContext);
        /// <summary>
        /// IDbCommand.ExecuteNonQuery
        /// </summary>
        /// <param name="requestContext"></param>
        /// <returns></returns>
        int Execute(AbstractRequestContext requestContext);
        /// <summary>
        /// IDbCommand.ExecuteScalar
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="requestContext"></param>
        /// <returns></returns>
        TResult ExecuteScalar<TResult>(AbstractRequestContext requestContext);
        /// <summary>
        /// 查询返回List
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="requestContext"></param>
        /// <returns></returns>
        IList<TResult> Query<TResult>(AbstractRequestContext requestContext);
        /// <summary>
        /// 查询返回单个实体
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="requestContext"></param>
        /// <returns></returns>
        TResult QuerySingle<TResult>(AbstractRequestContext requestContext);
        DataSet GetDataSet(AbstractRequestContext requestContext);
        DataTable GetDataTable(AbstractRequestContext requestContext);
        #region Async
        Task<ExecutionContext> InvokeAsync<TResult>(AbstractRequestContext requestContext);
        /// <summary>
        /// IDbCommand.ExecuteNonQuery
        /// </summary>
        /// <param name="requestContext"></param>
        /// <returns></returns>
        Task<int> ExecuteAsync(AbstractRequestContext requestContext);
        /// <summary>
        /// IDbCommand.ExecuteScalar
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="requestContext"></param>
        /// <returns></returns>
        Task<TResult> ExecuteScalarAsync<TResult>(AbstractRequestContext requestContext);
        /// <summary>
        /// 查询返回List
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="requestContext"></param>
        /// <returns></returns>
        Task<IList<TResult>> QueryAsync<TResult>(AbstractRequestContext requestContext);

        /// <summary>
        /// 查询返回单个实体
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="requestContext"></param>
        /// <returns></returns>
        Task<TResult> QuerySingleAsync<TResult>(AbstractRequestContext requestContext);
        Task<DataSet> GetDataSetAsync(AbstractRequestContext requestContext);
        Task<DataTable> GetDataTableAsync(AbstractRequestContext requestContext);
        #endregion
    }
}
