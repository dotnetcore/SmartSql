using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace SmartSql.Diagnostics
{
    public class EventData
    {
        public EventData(Guid operationId, string operation)
        {
            OperationId = operationId;
            Operation = operation;
        }
        public Guid OperationId { get; }

        public string Operation { get; }
    }
}
