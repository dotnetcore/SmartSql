using System;
using System.Collections.Generic;
using System.Text;
using SmartSql.DyRepository.Annotations;
using SmartSql.Test.Entities;

namespace SmartSql.Test.Unit.DyRepository
{
    public interface IAllPrimitiveRepository
    {
        long Insert(AllPrimitive entity);
        IEnumerable<AllPrimitive> Query([Param("Taken")]int taken);
    }
}
