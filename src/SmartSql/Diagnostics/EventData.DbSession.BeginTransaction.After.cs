using SmartSql.DbSession;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmartSql.Diagnostics
{
    public class DbSessionBeginTransactionAfterEventData : DbSessionEventData
    {
        public DbSessionBeginTransactionAfterEventData(Guid operationId, string operation) : base(operationId, operation)
        {

        }
    }
}
