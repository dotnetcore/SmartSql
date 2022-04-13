using SmartSql.Test.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using System.Linq;
using SmartSql.Cache;

namespace SmartSql.Test.Unit.Cache
{
    [Collection("GlobalSmartSql")]
    public class RedisCacheProviderTest
    {
        protected ISqlMapper SqlMapper { get; }

        public RedisCacheProviderTest(SmartSqlFixture smartSqlFixture)
        {
            SqlMapper = smartSqlFixture.SqlMapper;
        }

        [Fact(Skip = "Missing unit test runtime environment")]
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

        [Fact(Skip = "Missing unit test runtime environment")]
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

        [Fact(Skip = "Missing unit test runtime environment")]
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