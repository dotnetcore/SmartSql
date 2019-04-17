using SmartSql.DbSession;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmartSql.Diagnostics
{
    public class DbSessionOpenErrorEventData : DbSessionEventData, IErrorEventData
    {
        public DbSessionOpenErrorEventData(Guid operationId, string operation) : base(operationId, operation)
        {

        }

        public Exception Exception { get; set; }
    }
}
