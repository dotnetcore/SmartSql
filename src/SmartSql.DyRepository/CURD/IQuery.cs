using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SmartSql.DyRepository
{
    public interface IQuery<TEntity>
    {
        IEnumerable<TEntity> Query(object reqParams);
    }



    public interface IQueryAsync<TEntity>
    {
        Task<IEnumerable<TEntity>> QueryAsync(object reqParams);
    }
}
