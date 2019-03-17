using SmartSql.DbSession;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmartSql.Diagnostics
{
    public class DbSessionDisposeBeforeEventData : DbSessionEventData
    {
        public DbSessionDisposeBeforeEventData(Guid operationId, string operation) : base(operationId, operation)
        {

        }
    }
}
