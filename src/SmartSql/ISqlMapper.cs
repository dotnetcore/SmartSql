using SmartSql.Configuration;
using SmartSql.Data;
using SmartSql.DbSession;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace SmartSql
{
    public interface ISqlMapper : ITransaction, IDisposable
    {
        SmartSqlConfig SmartSqlConfig { get; }
        IDbSessionStore SessionStore { get; }
        /// <summary>
        /// IDbCommand.ExecuteNonQuery
        /// </summary>
        /// <param name="requestContext"></param>
        /// <returns></returns>
        int Execute(AbstractRequestContext requestContext);
        /// <summary>
        /// IDbCommand.ExecuteScalar
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="requestContext"></param>
        /// <returns></returns>
        T ExecuteScalar<T>(AbstractRequestContext requestContext);
        /// <summary>
        /// 查询返回List
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="requestContext"></param>
        /// <returns></returns>
        IList<T> Query<T>(AbstractRequestContext requestContext);
        /// <summary>
        /// 查询返回单个实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="requestContext"></param>
        /// <returns></returns>
        T QuerySingle<T>(AbstractRequestContext requestContext);

        DataSet GetDataSet(AbstractRequestContext requestContext);
        DataTable GetDataTable(AbstractRequestContext requestContext);
        #region Async
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
