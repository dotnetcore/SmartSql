using SmartSql.Abstractions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using NLog;
namespace SmartSql.Logging.Impl
{
    public class NLogger : ILog
    {
        public Logger Logger { get; }
        public NLogger(Logger logger)
        {
            Logger = logger;
        }
        public void Debug(String message)
        {
            Logger.Debug(message);
        }

        public void Debug(String message, Exception exception)
        {
            Logger.Debug(exception, message);
        }

        public void Error(String message)
        {
            Logger.Error(message);
        }

        public void Error(String message, Exception exception)
        {
            Logger.Error(exception, message);
        }

        public void Fatal(String message)
        {
            Logger.Fatal(message);
        }

        public void Fatal(String message, Exception exception)
        {
            Logger.Fatal(exception, message);
        }

        public void Info(String message)
        {
            Logger.Info(message);
        }

        public void Info(String message, Exception exception)
        {
            Logger.Info(exception, message);
        }

        public void Warn(String message)
        {
            Logger.Warn(message);
        }

        public void Warn(String message, Exception exception)
        {
            Logger.Warn(exception, message);
        }
    }
}
