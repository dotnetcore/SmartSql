using SmartSql.DyRepository;
using SmartSql.UTests.Entity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace SmartSql.UTests.DyRepository
{
    public interface IEntityRepository : IRepository<T_Entity>
    {
        IEnumerable<long> QueryId();
        IEnumerable<EntityStatus> QueryStatus();
        IEnumerable<EntityStatus?> QueryNullStatus();

        int Delete(long Id);
        int Update(T_Entity entity);
        IEnumerable<T_Entity> Query(String FString);
        T_Entity GetEntity(long Id);
        [Statement(Id = "MultiQuery")]
        DataTable QueryDataTable();
        [Statement(Id = "MultiQuery")]
        DataSet QueryDataSet();

        [Statement(Sql = "Select Top(@Taken) T.* From T_Entity T With(NoLock);")]
        IEnumerable<T_Entity> QueryBySql(int Taken);
    }
}
