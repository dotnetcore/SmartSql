using System;
using System.Collections.Generic;
using System.Text;

namespace SmartSql.Diagnostics
{
    public interface IErrorEventData
    {
        Exception Exception { get; }
    }
}
