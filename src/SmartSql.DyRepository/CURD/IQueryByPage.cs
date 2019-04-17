using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SmartSql.DyRepository
{
    public interface IQueryByPage<TEntity>
    {
        IEnumerable<TEntity> QueryByPage(object reqParams);
    }
    public interface IQueryByPageAsync<TEntity>
    {
        Task<IEnumerable<TEntity>> QueryByPageAsync(object reqParams);
    }
}
