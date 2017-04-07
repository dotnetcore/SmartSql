using System;
using System.Collections.Generic;
using System.Text;

namespace SmartSql.DataAccess.Abstractions
{
    /// <summary>
    /// 数据查询接口
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public interface IQueryDataAccess<TEntity> : IIsExist, IGetEntity<TEntity>, IGetList, IGetTop, IGetListByPage, IGetRecord
         where TEntity : class
    {
    }

    /// <summary>
    /// 判断是否存在
    /// </summary>
    public interface IIsExist
    {
        bool IsExist(object paramObj);
    }
    /// <summary>
    /// 获取实体
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public interface IGetEntity<TEntity> where TEntity : class
    {
        TEntity GetEntity(object paramObj);
        TEntity GetEntity<TPrimary>(TPrimary Id);
    }
    /// <summary>
    /// 获取列表
    /// </summary>
    public interface IGetList
    {
        IList<TResponse> GetList<TResponse>(object paramObj);
    }
    /// <summary>
    /// 获取N个数据列表
    /// </summary>
    public interface IGetTop
    {
        IList<TResponse> GetTop<TResponse>(int topNum, object paramObj);
    }
    /// <summary>
    /// 分页
    /// </summary>
    public interface IGetListByPage
    {
        IList<TResponse> GetListByPage<TResponse>(object paramObj);
    }
    /// <summary>
    /// 获取记录数
    /// </summary>
    public interface IGetRecord
    {
        int GetRecord(object paramObj);
    }
}
