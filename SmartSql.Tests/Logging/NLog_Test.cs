using System;
using System.Collections.Generic;
using System.Text;
using NLog;
using Xunit;
using System.Diagnostics;

namespace SmartSql.Tests.Logging
{
    
    public class NLog_Test
    {
        [Fact]
        public void Log()
        {
            var logger = LogManager.GetLogger("Log", this.GetType());
            logger.Info("hello");
            
        }
    }
}
