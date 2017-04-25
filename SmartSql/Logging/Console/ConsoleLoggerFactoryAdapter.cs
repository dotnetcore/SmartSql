using SmartSql.Abstractions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmartSql.Logging
{
    public class ConsoleLoggerFactoryAdapter : ILoggerFactoryAdapter
    {
        public ILog GetLogger(Type type)
        {
            return new ConsoleLogger();
        }

        public ILog GetLogger(string name)
        {
            return new ConsoleLogger();
        }
    }
}
