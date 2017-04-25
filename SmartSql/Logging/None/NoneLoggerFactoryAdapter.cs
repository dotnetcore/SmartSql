using SmartSql.Abstractions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmartSql.Logging.None
{
    public class NoneLoggerFactoryAdapter : ILoggerFactoryAdapter
    {
        public ILog GetLogger(Type type)
        {
            return new NoneLogger();
        }

        public ILog GetLogger(string name)
        {
            return new NoneLogger();
        }
    }
}
