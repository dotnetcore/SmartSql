using System;
using System.Collections.Generic;
using System.Text;

namespace SmartSql.DataAccess.Abstractions
{
    /// <summary>
    /// DataAccess功能接口
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IDataAccess<TEntity> : IInsert<TEntity>, IDelete, IUpdate<TEntity>, ITransaction
        , IQueryDataAccess<TEntity>
        where TEntity : class
    {
    }
    /// <summary>
    /// 插入
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public interface IInsert<TEntity> where TEntity : class
    {
        /// <summary>
        /// 插入数据
        /// </summary>
        /// <typeparam name="TPrimary">主键类型</typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        TPrimary Insert<TPrimary>(TEntity entity);
    }
    /// <summary>
    /// 删除
    /// </summary>
    public interface IDelete
    {
        /// <summary>
        /// 主键类型
        /// </summary>
        /// <typeparam name="TPrimary"></typeparam>
        /// <param name="Id"></param>
        /// <returns></returns>
        int Delete<TPrimary>(TPrimary Id);
        int DeleteList(string Ids);
    }
    /// <summary>
    /// 更新
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public interface IUpdate<TEntity> where TEntity : class
    {
        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        int Update(TEntity entity);
    }

    /// <summary>
    /// 事务接口
    /// </summary>
    public interface ITransaction
    {
        /// <summary>
        /// 开启事务
        /// </summary>
        /// <returns></returns>
        void BeginTransaction();
        /// <summary>
        /// 提交事务
        /// </summary>
        void CommitTransaction();
        /// <summary>
        /// 回滚事务
        /// </summary>
        void RollBackTransaction();
    }
}
