using SmartSql.DbSession;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmartSql.Diagnostics
{
    public class DbSessionInvokeAfterEventData : DbSessionInvokeEventData
    {
        public DbSessionInvokeAfterEventData(Guid operationId, string operation) : base(operationId, operation)
        {

        }
    }
}
