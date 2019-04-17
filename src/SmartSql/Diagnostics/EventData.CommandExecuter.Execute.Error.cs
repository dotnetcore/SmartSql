using System;
using System.Collections.Generic;
using System.Text;

namespace SmartSql.Diagnostics
{
    public class CommandExecuterExecuteErrorEventData : CommandExecuterEventData, IErrorEventData
    {
        public CommandExecuterExecuteErrorEventData(Guid operationId, string operation) : base(operationId, operation)
        {

        }

        public Exception Exception { get; set; }
    }
}
