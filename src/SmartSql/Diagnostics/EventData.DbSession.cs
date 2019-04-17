using SmartSql.DbSession;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmartSql.Diagnostics
{
    public class DbSessionEventData : EventData
    {
        public DbSessionEventData(Guid operationId, string operation) : base(operationId, operation)
        {

        }

        public IDbSession DbSession { get; set; }
    }
}
