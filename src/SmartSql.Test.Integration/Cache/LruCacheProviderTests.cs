using FluentAssertions;
using SmartSql.Test.Entities;
using Xunit;

namespace SmartSql.Test.Integration.Cache
{
    public class LruCacheProviderTest : IntegrationTestBase
    {
        public LruCacheProviderTest(SmartSqlFixture fixture) : base(fixture) { }

        [Fact]
        public void GetByCache()
        {
            var expected = SqlMapper.QuerySingle<CachedEntity>(new RequestContext
            {
                Scope = "LruCache",
                SqlId = "GetByCache"
            });
            var actual = SqlMapper.QuerySingle<CachedEntity>(new RequestContext
            {
                Scope = "LruCache",
                SqlId = "GetByCache"
            });
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void GetByCacheFromRequest()
        {
            var list = SqlMapper.Query<CachedEntity>(new RequestContext
            {
                Scope = "LruCache",
                SqlId = "GetByCacheFromRequest",
                Request = new { CacheKey = "CacheKey" }
            });
            var cachedList = SqlMapper.Query<CachedEntity>(new RequestContext
            {
                Scope = "LruCache",
                SqlId = "GetByCacheFromRequest",
                Request = new { CacheKey = "CacheKey" }
            });
            Assert.Equal(list, cachedList);
        }
    }
}
