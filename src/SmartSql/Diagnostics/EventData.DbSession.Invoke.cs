using SmartSql.DbSession;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmartSql.Diagnostics
{
    public class DbSessionInvokeEventData : DbSessionEventData
    {
        public DbSessionInvokeEventData(Guid operationId, string operation) : base(operationId, operation)
        {

        }

        public ExecutionContext ExecutionContext { get; set; }
    }
}
