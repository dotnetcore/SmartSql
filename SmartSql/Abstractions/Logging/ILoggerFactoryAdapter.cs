using System;
using System.Collections.Generic;
using System.Text;

namespace SmartSql.Abstractions.Logging
{
    public interface ILoggerFactoryAdapter
    {
        ILog GetLogger(Type type);
        ILog GetLogger(string name);
    }
}
