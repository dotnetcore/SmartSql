using FluentAssertions;
using SmartSql.Test.Entities;
using Xunit;

namespace SmartSql.Test.Integration.Cache
{
    public class FifoCacheProviderTest : IntegrationTestBase
    {
        public FifoCacheProviderTest(SmartSqlFixture fixture) : base(fixture) { }

        [Fact]
        public void GetByCache()
        {
            var expected = SqlMapper.QuerySingle<CachedEntity>(new RequestContext
            {
                Scope = "FifoCache",
                SqlId = "GetByCache"
            });
            var actual = SqlMapper.QuerySingle<CachedEntity>(new RequestContext
            {
                Scope = "FifoCache",
                SqlId = "GetByCache"
            });
            Assert.Equal(expected, actual);
        }
    }
}
