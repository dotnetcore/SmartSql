using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SmartSql.DyRepository
{
    public interface IQuery<TEntity>
    {
        IList<TEntity> Query(object reqParams);
    }



    public interface IQueryAsync<TEntity>
    {
        Task<IList<TEntity>> QueryAsync(object reqParams);
    }
}
