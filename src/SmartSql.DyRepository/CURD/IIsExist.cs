using SmartSql.DyRepository.Annotations;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SmartSql.DyRepository
{
    public interface IIsExist
    {
        [Statement(Execute = ExecuteBehavior.ExecuteScalar)]
        bool IsExist(object reqParams);
    }
    public interface IIsExistAsync
    {
        [Statement(Execute = ExecuteBehavior.ExecuteScalar)]
        Task<bool> IsExistAsync(object reqParams);
    }
}
