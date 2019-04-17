using SmartSql.DbSession;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmartSql.Diagnostics
{
    public class DbSessionInvokeBeforeEventData : DbSessionInvokeEventData
    {
        public DbSessionInvokeBeforeEventData(Guid operationId, string operation) : base(operationId, operation)
        {

        }
    }
}
