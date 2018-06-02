using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmartSql.Logging
{
    public class NoneLoggerFactory : ILoggerFactory
    {
        public static readonly NoneLoggerFactory Instance = new NoneLoggerFactory();

        public ILogger CreateLogger(string name)
        {
            return NoneLogger.Instance;
        }

        public void AddProvider(ILoggerProvider provider)
        {
        }

        public void Dispose()
        {
        }
    }
}
