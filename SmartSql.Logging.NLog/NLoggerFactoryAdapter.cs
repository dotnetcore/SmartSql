using SmartSql.Abstractions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using NLog;
namespace SmartSql.Logging.Impl
{
    public class NLoggerFactoryAdapter : ILoggerFactoryAdapter
    {
        public ILog GetLogger(Type type)
        {
            var nlogger = NLog.LogManager.GetLogger("", type);
            return new NLogger(nlogger);
        }

        public ILog GetLogger(string name)
        {
            var nlogger = NLog.LogManager.GetLogger(name);
            return new NLogger(nlogger);
        }
    }
}
