using System;
using System.Collections.Generic;
using System.Text;

namespace SmartSql.Abstractions.Logging
{
    public interface ILog
    {
        void Debug(String message);
        void Debug(String message, Exception exception);
        void Error(String message);
        void Error(String message, Exception exception);
        void Info(String message);
        void Info(String message, Exception exception);
        void Warn(String message);
        void Warn(String message, Exception exception);
        void Fatal(String message);
        void Fatal(String message, Exception exception);
    }
}
