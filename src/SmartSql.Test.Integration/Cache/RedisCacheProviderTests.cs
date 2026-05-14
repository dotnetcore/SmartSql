using FluentAssertions;
using System.Linq;
using SmartSql.Test.Entities;
using Xunit;

namespace SmartSql.Test.Integration.Cache
{
    public class RedisCacheProviderTest : IntegrationTestBase
    {
        public RedisCacheProviderTest(SmartSqlFixture fixture) : base(fixture) { }

        [Fact]
        public void GetByCache()
        {
            var list = SqlMapper.Query<AllPrimitive>(new RequestContext
            {
                Scope = "RedisCache",
                SqlId = "GetByCache",
                Request = new { Taken = 8 }
            });
            var cachedList = SqlMapper.Query<AllPrimitive>(new RequestContext
            {
                Scope = "RedisCache",
                SqlId = "GetByCache",
                Request = new { Taken = 8 }
            });
            Assert.Equal(list.Count, cachedList.Count);
        }

        [Fact]
        public void QueryByRedisCacheWithKey()
        {
            var list = SqlMapper.Query<AllPrimitive>(new RequestContext
            {
                Scope = "RedisCache",
                SqlId = "GetByCache",
                Request = new { Taken = 8 },
                CacheKeyTemplate = "QueryByRedisCacheWithKey"
            });
            var cachedList = SqlMapper.Query<AllPrimitive>(new RequestContext
            {
                Scope = "RedisCache",
                SqlId = "GetByCache",
                Request = new { Taken = 8 },
                CacheKeyTemplate = "QueryByRedisCacheWithKey"
            });
            Assert.Equal(list.Count(), cachedList.Count());
        }

        [Fact]
        public void QueryByRedisCacheWithKeyParam()
        {
            var list = SqlMapper.Query<AllPrimitive>(new RequestContext
            {
                Scope = "RedisCache",
                SqlId = "GetByCache",
                Request = new { Taken = 8, UserId = 1 },
                CacheKeyTemplate = "QueryByRedisCacheWithKeyParam:uid:$UserId:taken:$Taken"
            });
            Assert.NotNull(list);
        }
    }
}
