using SmartSql.DbSession;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmartSql.Diagnostics
{
    public class DbSessionRollbackBeforeEventData : DbSessionEventData
    {
        public DbSessionRollbackBeforeEventData(Guid operationId, string operation) : base(operationId, operation)
        {

        }
    }
}
