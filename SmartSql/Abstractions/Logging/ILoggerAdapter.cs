using System;
using System.Collections.Generic;
using System.Text;

namespace SmartSql.Abstractions.Logging
{
    public interface ILoggerAdapter
    {
        ILog GetLogger(Type type);
        ILog GetLogger(string name);
    }
}
