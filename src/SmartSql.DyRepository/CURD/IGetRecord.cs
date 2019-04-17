using SmartSql.DyRepository.Annotations;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SmartSql.DyRepository
{
    public interface IGetRecord
    {
        [Statement(Execute = ExecuteBehavior.ExecuteScalar)]
        int GetRecord(object reqParams);
    }
    public interface IGetRecordAsync
    {
        [Statement(Execute = ExecuteBehavior.ExecuteScalar)]
        Task<int> GetRecordAsync(object reqParams);
    }
}
