using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SmartSql.DyRepository
{
    public interface IQueryByPage<TEntity>
    {
        IList<TEntity> QueryByPage(object reqParams);
    }
    public interface IQueryByPageAsync<TEntity>
    {
        Task<IList<TEntity>> QueryByPageAsync(object reqParams);
    }
}
