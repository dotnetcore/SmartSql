using SmartSql.DbSession;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmartSql.Diagnostics
{
    public class DbSessionBeginTransactionBeforeEventData : DbSessionEventData
    {
        public DbSessionBeginTransactionBeforeEventData(Guid operationId, string operation) : base(operationId, operation)
        {

        }
    }
}
