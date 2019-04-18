using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SmartSql.DyRepository
{
    public interface IInsert<in TEntity>
    {
        int Insert(TEntity entity);
    }
    public interface IInsertAsync<in TEntity>
    {
        Task<int> InsertAsync(TEntity entity);
    }
}
