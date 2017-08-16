using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions.Internal;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmartSql.Abstractions.Logging
{
    public class NullLogger : ILogger
    {
        public static NullLogger Instance { get; } = new NullLogger();

        private NullLogger()
        {
        }

        /// <inheritdoc />
        public IDisposable BeginScope<TState>(TState state)
        {
            return NullScope.Instance;
        }

        /// <inheritdoc />
        public bool IsEnabled(LogLevel logLevel)
        {
            return false;
        }

        /// <inheritdoc />
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
        }
    }
}
