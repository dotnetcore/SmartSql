using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using SmartSql.DbSession;

namespace SmartSql.Tests.DbSession
{
    public class DbConnectionSessionStore_Test
    {
        [Fact]
        public void CreateInstance()
        {
            DbConnectionSessionStore sesstionStore = new DbConnectionSessionStore("dbStore");
            
            Assert.NotNull(sesstionStore);
        }
    }
}
