using SmartSql.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmartSql.DataAccess.Abstractions
{
    /// <summary>
    /// 数据查询接口
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public interface IReadDataAccess<TEntity> : IIsExist, IGetEntity<TEntity>, IGetList, IGetListByPage, IGetRecord
         where TEntity : class
    {
    }

    /// <summary>
    /// 判断是否存在
    /// </summary>
    public interface IIsExist
    {
        bool IsExist(object paramObj, DataSourceChoice sourceChoice = DataSourceChoice.Read);
    }
    /// <summary>
    /// 获取实体
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public interface IGetEntity<TEntity> where TEntity : class
    {
        TEntity GetEntity(object paramObj, DataSourceChoice sourceChoice = DataSourceChoice.Read);
        TEntity GetEntity<TPrimary>(TPrimary Id, DataSourceChoice sourceChoice = DataSourceChoice.Read);
    }
    /// <summary>
    /// 获取列表
    /// </summary>
    public interface IGetList
    {
        IEnumerable<TResponse> GetList<TResponse>(object paramObj, DataSourceChoice sourceChoice = DataSourceChoice.Read);
    }
    /// <summary>
    /// 分页
    /// </summary>
    public interface IGetListByPage
    {
        IEnumerable<TResponse> GetListByPage<TResponse>(object paramObj,  DataSourceChoice sourceChoice = DataSourceChoice.Read);
    }
    /// <summary>
    /// 获取记录数
    /// </summary>
    public interface IGetRecord
    {
        int GetRecord(object paramObj, DataSourceChoice sourceChoice = DataSourceChoice.Read);
    }
}
