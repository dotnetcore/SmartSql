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

    public interface IDbSession : ITransaction, IDisposable
    {
        event DbSessionEventHandler Opened;
        event DbSessionEventHandler TransactionBegan;
        event DbSessionEventHandler Committed;
        event DbSessionEventHandler Rollbacked;
        event DbSessionEventHandler Disposed;
        Guid Id { get; }
        DbTransaction Transaction { get; }
        DbConnection Connection { get; }
        AbstractDataSource DataSource { get; }
        SmartSqlConfig SmartSqlConfig { get; }
        void SetDataSource(AbstractDataSource dataSource);
        void Open();
        Task OpenAsync();
        Task OpenAsync(CancellationToken cancellationToken);
        ExecutionContext Invoke<TResult>(RequestContext requestContext);
        /// <summary>
        /// IDbCommand.ExecuteNonQuery
        /// </summary>
        /// <param name="requestContext"></param>
        /// <returns></returns>
        int Execute(RequestContext requestContext);
        /// <summary>
        /// IDbCommand.ExecuteScalar
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="requestContext"></param>
        /// <returns></returns>
        TResult ExecuteScalar<TResult>(RequestContext requestContext);
        /// <summary>
        /// 查询返回List
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="requestContext"></param>
        /// <returns></returns>
        IEnumerable<TResult> Query<TResult>(RequestContext requestContext);
        /// <summary>
        /// 查询返回单个实体
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="requestContext"></param>
        /// <returns></returns>
        TResult QuerySingle<TResult>(RequestContext requestContext);
        DataSet GetDataSet(RequestContext requestContext);
        DataTable GetDataTable(RequestContext requestContext);
        #region Async
        Task<ExecutionContext> InvokeAsync<TResult>(RequestContext requestContext);
        /// <summary>
        /// IDbCommand.ExecuteNonQuery
        /// </summary>
        /// <param name="requestContext"></param>
        /// <returns></returns>
        Task<int> ExecuteAsync(RequestContext requestContext);
        /// <summary>
        /// IDbCommand.ExecuteScalar
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="requestContext"></param>
        /// <returns></returns>
        Task<TResult> ExecuteScalarAsync<TResult>(RequestContext requestContext);
        /// <summary>
        /// 查询返回List
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="requestContext"></param>
        /// <returns></returns>
        Task<IEnumerable<TResult>> QueryAsync<TResult>(RequestContext requestContext);

        /// <summary>
        /// 查询返回单个实体
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="requestContext"></param>
        /// <returns></returns>
        Task<TResult> QuerySingleAsync<TResult>(RequestContext requestContext);
        Task<DataSet> GetDataSetAsync(RequestContext requestContext);
        Task<DataTable> GetDataTableAsync(RequestContext requestContext);
        #endregion
    }
}
