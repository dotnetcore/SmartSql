using System;
using System.Collections.Generic;
using System.Text;

namespace SmartSql.Abstractions.Logging.None
{
    public class NoneLoggerAdapter : ILoggerAdapter
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
