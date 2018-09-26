using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Collections;
using System.Data.Common;
using System.Threading.Tasks;
using SmartSql.Abstractions.DbSession;
using SmartSql.Abstractions.DataSource;
using SmartSql.Abstractions.Cache;
using SmartSql.Abstractions.Config;
using SmartSql.Configuration;

namespace SmartSql.Abstractions
{
    /// <summary>
    /// SmartSql 映射器
    /// </summary>
    public interface ISmartSqlMapper : ISmartSqlMapperAsync, ISession, ITransaction, IDisposable
    {
        SmartSqlOptions SmartSqlOptions { get; }
        /// <summary>
        /// IDbCommand.ExecuteNonQuery
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        int Execute(RequestContext context);
        /// <summary>
        /// IDbCommand.ExecuteScalar
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context"></param>
        /// <returns></returns>
        T ExecuteScalar<T>(RequestContext context);
        /// <summary>
        /// 查询返回List
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context"></param>
        /// <returns></returns>
        IEnumerable<T> Query<T>(RequestContext context);
        /// <summary>
        /// 查询返回单个实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context"></param>
        /// <returns></returns>
        T QuerySingle<T>(RequestContext context);
        IMultipleResult FillMultiple(RequestContext context, IMultipleResult multipleResult);
        DataTable GetDataTable(RequestContext context);
        DataSet GetDataSet(RequestContext context);
        /// <summary>
        /// 获取多结果集嵌套对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context"></param>
        /// <returns></returns>
        T GetNested<T>(RequestContext context);
    }

    public enum DataSourceChoice
    {
        Unknow,
        Write,
        Read
    }
}
