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

        [Fact]
        public void QueryByRedisCache()
        {
            var list = SqlMapper.Query<AllPrimitive>(new RequestContext
            {
                Scope = nameof(AllPrimitive),
                SqlId = "QueryByRedisCache",
                Request = new {Taken = 8}
            });
            var cachedList = SqlMapper.Query<AllPrimitive>(new RequestContext
            {
                Scope = nameof(AllPrimitive),
                SqlId = "QueryByRedisCache",
                Request = new {Taken = 8}
            });
            Assert.Equal(list.Count(), cachedList.Count());
        }

        [Fact]
        public void QueryByRedisCacheWithKey()
        {
            var list = SqlMapper.Query<AllPrimitive>(new RequestContext
            {
                Scope = nameof(AllPrimitive),
                SqlId = "QueryByRedisCache",
                Request = new {Taken = 8},
                CacheKey = new CacheKey("QueryByRedisCacheWithKey", typeof(IList<AllPrimitive>))
            });
            var cachedList = SqlMapper.Query<AllPrimitive>(new RequestContext
            {
                Scope = nameof(AllPrimitive),
                SqlId = "QueryByRedisCache",
                Request = new {Taken = 8},
                CacheKey = new CacheKey("QueryByRedisCacheWithKey", typeof(IList<AllPrimitive>))
            });
            Assert.Equal(list.Count(), cachedList.Count());
        }
    }
}