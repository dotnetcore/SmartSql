using System;
using System.Collections.Generic;
using System.Text;
using SmartSql.DyRepository.Annotations;
using SmartSql.Test.Entities;

namespace SmartSql.Test.Repositories
{
    public interface IAllPrimitiveRepository
    {
        [Statement(Sql = "SELECT Top (@Taken) T.* From T_AllPrimitive T With(NoLock)")]
        IEnumerable<AllPrimitive> Query([Param("Taken")]int taken);
        long Insert(AllPrimitive entity);

        (IEnumerable<AllPrimitive>, int) GetByPage_ValueTuple(int PageSize = 10, int PageIndex = 1);
    }
}
