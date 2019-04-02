using System;
using System.Collections.Generic;
using System.Text;

namespace SmartSql.Diagnostics
{
    public class CommandExecuterExecuteBeforeEventData : CommandExecuterEventData
    {
        public CommandExecuterExecuteBeforeEventData(Guid operationId, string operation) : base(operationId, operation)
        {

        }
    }
}
