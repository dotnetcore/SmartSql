using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace SmartSql.DyRepository
{
    public interface IRepository
    {
        ISqlMapper SqlMapper { get; }
    }
    /// <summary>
    /// 泛型仓储接口
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TPrimary"></typeparam>
    public interface IRepository<TEntity, TPrimary> : IRepository
        , IInsert<TEntity>, IUpdate<TEntity>, IDelete<TPrimary>,
        IGetEntity<TEntity, TPrimary>,
        IGetRecord, IQueryByPage<TEntity>,
        IQuery<TEntity>, IIsExist
    {

    }
    /// <summary>
    /// 异步泛型仓储接口
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TPrimary"></typeparam>
    public interface IRepositoryAsync<TEntity, TPrimary> : IRepository
      , IInsertAsync<TEntity>, IUpdateAsync<TEntity>, IDeleteAsync<TPrimary>,
        IGetEntityAsync<TEntity, TPrimary>,
        IGetRecordAsync, IQueryByPageAsync<TEntity>,
        IQueryAsync<TEntity>, IIsExistAsync
    {

    }
}
