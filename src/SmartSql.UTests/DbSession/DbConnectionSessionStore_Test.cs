using SmartSql.DbSession;
using System;
using Xunit;
using SmartSql.Abstractions.DbSession;

namespace SmartSql.UTests.DbSession
{
    public class DbConnectionSessionStore_Test : TestBase, IDisposable
    {
        IDbConnectionSessionStore _sessionStore;
        public DbConnectionSessionStore_Test()
        {
            _sessionStore = new DbConnectionSessionStore(LoggerFactory, DbProviderFactory);
        }

        public void Dispose()
        {
            _sessionStore.Dispose();
        }

        [Fact]
        public void GetOrAddDbSession()
        {
            var dbSession = _sessionStore.GetOrAddDbSession(DataSource);
            Assert.NotNull(dbSession);
        }

    }
}
