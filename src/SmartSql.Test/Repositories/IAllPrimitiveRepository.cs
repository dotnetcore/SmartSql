using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using SmartSql.AOP;
using SmartSql.DyRepository.Annotations;
using SmartSql.Test.Entities;

namespace SmartSql.Test.Repositories
{
    public interface IAllPrimitiveRepository
    {
        [Statement(Id = "QueryByTaken", Sql = "SELECT Top (@Taken) T.* From T_AllPrimitive T With(NoLock)")]
        IList<AllPrimitive> Query([Param("Taken")] int taken);

        long Insert(AllPrimitive entity);

        (IList<AllPrimitive>, int) GetByPage_ValueTuple(int PageSize = 10, int PageIndex = 1);

        [UseTransaction]
        [Statement(Id = "Insert")]
        long InsertByAnnotationTransaction(AllPrimitive entity);

        [Transaction]
        [Statement(Id = "Insert")]
        long InsertByAnnotationAOPTransaction(AllPrimitive entity);

        [Statement(Id = "QueryDictionary", Sql = "SELECT Top (@Taken) T.* From T_AllPrimitive T With(NoLock)")]
        IList<IDictionary<String, Object>> QueryDictionary([Param("Taken")] int taken);
    }
}