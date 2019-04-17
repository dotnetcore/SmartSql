using System;
using System.Collections.Generic;
using System.Text;

namespace SmartSql.Diagnostics
{
    public class CommandExecuterExecuteAfterEventData : CommandExecuterEventData
    {
        public CommandExecuterExecuteAfterEventData(Guid operationId, string operation) : base(operationId, operation)
        {

        }
    }
}
