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
    public interface IReadDataAccess<TEntity> : IIsExist, IGetEntity<TEntity>, IGetList<TEntity>, IGetListByPage<TEntity>, IGetRecord
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
        TEntity GetEntity(object id, DataSourceChoice sourceChoice = DataSourceChoice.Read);
    }
    /// <summary>
    /// 获取列表
    /// </summary>
    public interface IGetList<TEntity> where TEntity : class
    {
        IEnumerable<TEntity> GetList(object paramObj, DataSourceChoice sourceChoice = DataSourceChoice.Read);
    }
    /// <summary>
    /// 分页
    /// </summary>
    public interface IGetListByPage<TEntity> where TEntity : class
    {
        IEnumerable<TEntity> GetListByPage(object paramObj, DataSourceChoice sourceChoice = DataSourceChoice.Read);
    }
    /// <summary>
    /// 获取记录数
    /// </summary>
    public interface IGetRecord
    {
        int GetRecord(object paramObj, DataSourceChoice sourceChoice = DataSourceChoice.Read);
    }
}
