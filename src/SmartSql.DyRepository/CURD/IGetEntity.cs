using SmartSql.DyRepository.Annotations;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SmartSql.DyRepository
{
    public interface IGetEntity<out TEntity, in TPrimary>
    {
        TEntity GetEntity(object reqParams);
        [Statement(Id = "GetEntity")]
        TEntity GetById([Param("Id")]TPrimary id);
    }
    public interface IGetEntityAsync<TEntity, in TPrimary>
    {
        Task<TEntity> GetEntityAsync(object reqParams);
        [Statement(Id = "GetEntity")]
        Task<TEntity> GetByIdAsync([Param("Id")]TPrimary id);
    }
}
