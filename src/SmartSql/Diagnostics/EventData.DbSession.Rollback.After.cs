using SmartSql.DbSession;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmartSql.Diagnostics
{
    public class DbSessionRollbackAfterEventData : DbSessionEventData
    {
        public DbSessionRollbackAfterEventData(Guid operationId, string operation) : base(operationId, operation)
        {

        }
    }
}
