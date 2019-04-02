using System;
using System.Collections.Generic;
using System.Text;

namespace SmartSql.Diagnostics
{
    public class CommandExecuterEventData : EventData
    {
        public CommandExecuterEventData(Guid operationId, string operation) : base(operationId, operation)
        {

        }
        public ExecutionContext ExecutionContext { get; set; }
    }
}
