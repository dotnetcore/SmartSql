using SmartSql.DyRepository;
using SmartSql.UTests.Entity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

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

        [Statement(Sql = "Select Top(@taken) T.* From T_Entity T With(NoLock);")]
        IEnumerable<T_Entity> QueryBySql(int taken);

        Task DeleteAsync(long Id);
        Task<int> UpdateAsync(T_Entity entity);
        Task<IEnumerable<T_Entity>> QueryAsync(int Taken);
        Task<T_Entity> GetEntityAsync(long Id);
        [Statement(Id = "MultiQuery")]
        Task<DataTable> QueryDataTableAsync();
        [Statement(Id = "MultiQuery")]
        Task<DataSet> QueryDataSetAsync();
        [Statement(Sql = "Select Top(@taken) T.* From T_Entity T With(NoLock);")]
        Task<IEnumerable<T_Entity>> QueryBySqlAsync(int taken);
    }
}
