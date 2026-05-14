using System.Linq;
using FluentAssertions;
using SmartSql.Test.Entities;
using SmartSql.Test.Integration.Fixtures;
using Xunit;

namespace SmartSql.Test.Integration.MySql;

[Collection(MySqlFixture.CollectionName)]
public class MySqlRedisCacheTests : IntegrationTestBase
{
    public MySqlRedisCacheTests(MySqlFixture fixture) : base(fixture) { }

    [Fact(Skip = "Requires Redis container")]
    public void Should_ReturnSameCount_When_QueryTwice()
    {
        var first = SqlMapper.Query<AllPrimitive>(new RequestContext
        {
            Scope = "RedisCache", SqlId = "GetByCache", Request = new { Taken = 8 }
        });
        var second = SqlMapper.Query<AllPrimitive>(new RequestContext
        {
            Scope = "RedisCache", SqlId = "GetByCache", Request = new { Taken = 8 }
        });
        second.Should().HaveCount(first.Count);
    }

    [Fact(Skip = "Requires Redis container")]
    public void Should_ReturnSameCount_When_QueryWithCustomCacheKey()
    {
        var first = SqlMapper.Query<AllPrimitive>(new RequestContext
        {
            Scope = "RedisCache", SqlId = "GetByCache",
            Request = new { Taken = 8 }, CacheKeyTemplate = "QueryByRedisCacheWithKey"
        });
        var second = SqlMapper.Query<AllPrimitive>(new RequestContext
        {
            Scope = "RedisCache", SqlId = "GetByCache",
            Request = new { Taken = 8 }, CacheKeyTemplate = "QueryByRedisCacheWithKey"
        });
        second.Should().HaveCount(first.Count());
    }

    [Fact(Skip = "Requires Redis container")]
    public void Should_ReturnResults_When_QueryWithKeyParam()
    {
        var list = SqlMapper.Query<AllPrimitive>(new RequestContext
        {
            Scope = "RedisCache", SqlId = "GetByCache",
            Request = new { Taken = 8, UserId = 1 },
            CacheKeyTemplate = "QueryByRedisCacheWithKeyParam:uid:$UserId:taken:$Taken"
        });
        list.Should().NotBeNull();
    }
}
