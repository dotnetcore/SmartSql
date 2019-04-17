using SmartSql.DbSession;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmartSql.Diagnostics
{
    public class DbSessionOpenBeforeEventData : DbSessionEventData
    {
        public DbSessionOpenBeforeEventData(Guid operationId, string operation) : base(operationId, operation)
        {

        }

    }
}
