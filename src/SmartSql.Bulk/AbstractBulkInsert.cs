using SmartSql.DbSession;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace SmartSql.Bulk
{
    public abstract class AbstractBulkInsert : IBulkInsert
    {
        protected AbstractBulkInsert(IDbSession dbSession)
        {
            DbSession = dbSession;
        }
        public DataTable Table { get; set; }
        public IDbSession DbSession { get; }

        public void Dispose()
        {
            DbSession.Dispose();
        }
        public abstract void Insert();

        public abstract Task InsertAsync();
    }
}
