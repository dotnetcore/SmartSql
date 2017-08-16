using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using SmartSql.DbSession;
using SmartSql.Abstractions.Logging;

namespace SmartSql.Tests.DbSession
{
    public class DbConnectionSessionStore_Test
    {
        [Fact]
        public void CreateInstance()
        {
            DbConnectionSessionStore sesstionStore = new DbConnectionSessionStore(NullLoggerFactory.Instance, "dbStore");

            Assert.NotNull(sesstionStore);
        }
    }
}
