using FluentAssertions;
using Xunit;

namespace SmartSql.Test.Integration.Cache;

public class LruCacheProviderTests : IntegrationTestBase
{
    public LruCacheProviderTests(SmartSqlFixture fixture) : base(fixture) { }

    [Fact]
    public void Should_ReturnCachedEntity_When_QueryTwice()
    {
        var first = SqlMapper.QuerySingle<CachedEntity>(new RequestContext
        {
            Scope = "LruCache", SqlId = "GetByCache"
        });
        var second = SqlMapper.QuerySingle<CachedEntity>(new RequestContext
        {
            Scope = "LruCache", SqlId = "GetByCache"
        });
        second.Should().Be(first);
    }

    [Fact]
    public void Should_ReturnCachedList_When_QueryByCacheFromRequest()
    {
        var first = SqlMapper.Query<CachedEntity>(new RequestContext
        {
            Scope = "LruCache", SqlId = "GetByCacheFromRequest",
            Request = new { CacheKey = "CacheKey" }
        });
        var second = SqlMapper.Query<CachedEntity>(new RequestContext
        {
            Scope = "LruCache", SqlId = "GetByCacheFromRequest",
            Request = new { CacheKey = "CacheKey" }
        });
        second.Should().Equal(first);
    }
}
