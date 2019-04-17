using SmartSql.DbSession;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmartSql.Diagnostics
{
    public class DbSessionDisposeAfterEventData : DbSessionEventData
    {
        public DbSessionDisposeAfterEventData(Guid operationId, string operation) : base(operationId, operation)
        {

        }
    }
}
