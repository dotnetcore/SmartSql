using SmartSql.DbSession;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmartSql.Diagnostics
{
    public class DbSessionCommitBeforeEventData : DbSessionEventData
    {
        public DbSessionCommitBeforeEventData(Guid operationId, string operation) : base(operationId, operation)
        {

        }
    }
}
