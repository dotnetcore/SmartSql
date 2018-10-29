using System;
using System.Threading.Tasks;

namespace SmartSql.Batch
{
    public interface IBatchInsert : IDisposable
    {
        DbTable Table { get; set; }
        void Insert();
        Task InsertAsync();
    }
}
