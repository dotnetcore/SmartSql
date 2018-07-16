using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SmartSql.DyRepository
{
    public interface IRepository
    {
    }
    /// <summary>
    /// 泛型仓储接口
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TPrimary"></typeparam>
    public interface IRepository<TEntity, TPrimary> : IRepository
    {
        int Insert(TEntity entity);
        int Delete(object reqParams);
        [Statement(Id = "Delete")]
        int DeleteById([Param("Id")]TPrimary id);
        int Update(TEntity entity);
        [Statement(Id = "Update")]
        int DyUpdate(object dyObj);
        IEnumerable<TEntity> Query(object reqParams);
        IEnumerable<TEntity> QueryByPage(object reqParams);
        [Statement(Execute = ExecuteBehavior.ExecuteScalar)]
        int GetRecord(object reqParams);
        TEntity GetEntity(object reqParams);
        [Statement(Id = "GetEntity")]
        TEntity GetById([Param("Id")]TPrimary id);
        [Statement(Execute = ExecuteBehavior.ExecuteScalar)]
        bool IsExist(object reqParams);
    }
    /// <summary>
    /// 异步泛型仓储接口
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TPrimary"></typeparam>
    public interface IRepositoryAsync<TEntity, TPrimary> : IRepository
    {
        Task<int> InsertAsync(TEntity entity);
        Task<int> DeleteAsync(object reqParams);
        [Statement(Id = "Delete")]
        Task<int> DeleteByIdAsync([Param("Id")]TPrimary id);
        Task<int> UpdateAsync(TEntity entity);
        [Statement(Id = "Update")]
        Task<int> DyUpdateAsync(object dyObj);
        Task<IEnumerable<TEntity>> QueryAsync(object reqParams);
        Task<IEnumerable<TEntity>> QueryByPageAsync(object reqParams);
        [Statement(Execute = ExecuteBehavior.ExecuteScalar)]
        Task<int> GetRecordAsync(object reqParams);
        Task<TEntity> GetEntityAsync(object reqParams);
        [Statement(Id = "GetEntity")]
        Task<TEntity> GetByIdAsync([Param("Id")]TPrimary id);
        [Statement(Execute = ExecuteBehavior.ExecuteScalar)]
        Task<bool> IsExistAsync(object reqParams);
    }
}
