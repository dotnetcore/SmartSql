using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace SmartSql.Bulk
{
    public interface IBulkInsert : IDisposable
    {
        DataTable Table { get; set; }
        void Insert();
        Task InsertAsync();
    }
}
