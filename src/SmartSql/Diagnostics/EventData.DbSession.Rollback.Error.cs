using SmartSql.DbSession;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmartSql.Diagnostics
{
    public class DbSessionRollbackErrorEventData : DbSessionEventData, IErrorEventData
    {
        public DbSessionRollbackErrorEventData(Guid operationId, string operation) : base(operationId, operation)
        {

        }

        public Exception Exception { get; set; }
    }
}
