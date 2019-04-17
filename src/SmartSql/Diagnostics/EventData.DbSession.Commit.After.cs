using SmartSql.DbSession;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmartSql.Diagnostics
{
    public class DbSessionCommitAfterEventData : DbSessionEventData
    {
        public DbSessionCommitAfterEventData(Guid operationId, string operation) : base(operationId, operation)
        {

        }
    }
}
