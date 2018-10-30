using System;
using System.Threading.Tasks;
using System.Collections.Generic;
namespace SmartSql.Batch
{
    public interface IBatchInsert : IDisposable
    {
        IDictionary<String, ColumnMapping> ColumnMappings { get; }
        void AddColumnMapping(ColumnMapping columnMapping);
        void ClearColumnMapping();
        DbTable Table { get; set; }
        void Insert();
        Task InsertAsync();
    }
}
