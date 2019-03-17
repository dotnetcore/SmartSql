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
        int Execute(RequestContext requestContext);
        /// <summary>
        /// IDbCommand.ExecuteScalar
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="requestContext"></param>
        /// <returns></returns>
        T ExecuteScalar<T>(RequestContext requestContext);
        /// <summary>
        /// 查询返回List
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="requestContext"></param>
        /// <returns></returns>
        IEnumerable<T> Query<T>(RequestContext requestContext);
        /// <summary>
        /// 查询返回单个实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="requestContext"></param>
        /// <returns></returns>
        T QuerySingle<T>(RequestContext requestContext);

        DataSet GetDataSet(RequestContext requestContext);
        DataTable GetDataTable(RequestContext requestContext);
        #region Async
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
        /// <typeparam name="T"></typeparam>
        /// <param name="requestContext"></param>
        /// <returns></returns>
        Task<TResult> QuerySingleAsync<TResult>(RequestContext requestContext);
        Task<DataSet> GetDataSetAsync(RequestContext requestContext);
        Task<DataTable> GetDataTableAsync(RequestContext requestContext);
        #endregion

    }
}
