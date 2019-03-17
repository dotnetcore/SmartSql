using SmartSql.DyRepository.Annotations;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SmartSql.DyRepository
{
    public interface IUpdate<TEntity>
    {
        int Update(TEntity entity);
        [Statement(Id = "Update")]
        int DyUpdate(object dyObj);
    }
    public interface IUpdateAsync<TEntity>
    {
        Task<int> UpdateAsync(TEntity entity);
        [Statement(Id = "Update")]
        Task<int> DyUpdateAsync(object dyObj);
    }
}
