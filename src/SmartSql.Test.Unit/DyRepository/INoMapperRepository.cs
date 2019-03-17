using SmartSql.DyRepository.Annotations;
using SmartSql.Test.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmartSql.Test.Unit.DyRepository
{
    public interface INoMapperRepository
    {
        [Statement(Sql = "Select NewId();")]
        Guid GetGuidFromDb();
        [Statement(Sql = "Select Top 1 T.* From T_AllPrimitive T With(NoLock)")]
        AllPrimitive GetAllPrimitive();
    }
}
