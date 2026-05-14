using FluentAssertions;
using Xunit;

namespace SmartSql.Test.Integration.Cache;

public class FifoCacheProviderTests : IntegrationTestBase
{
    public FifoCacheProviderTests(SmartSqlFixture fixture) : base(fixture) { }

    [Fact]
    public void Should_ReturnCachedEntity_When_QueryTwice()
    {
        var first = SqlMapper.QuerySingle<CachedEntity>(new RequestContext
        {
            Scope = "FifoCache", SqlId = "GetByCache"
        });
        var second = SqlMapper.QuerySingle<CachedEntity>(new RequestContext
        {
            Scope = "FifoCache", SqlId = "GetByCache"
        });
        second.Should().Be(first);
    }
}
