using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmartSql.Abstractions.Logging
{
    public class NullLoggerFactory : ILoggerFactory
    {
        public static readonly NullLoggerFactory Instance = new NullLoggerFactory();

        public ILogger CreateLogger(string name)
        {
            return NullLogger.Instance;
        }

        public void AddProvider(ILoggerProvider provider)
        {
        }

        public void Dispose()
        {
        }
    }
}
