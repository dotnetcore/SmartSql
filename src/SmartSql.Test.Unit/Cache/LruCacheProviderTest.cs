using SmartSql.Test.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace SmartSql.Test.Unit.Cache
{
    [Collection("GlobalSmartSql")]
    public class LruCacheProviderTest
    {
        protected ISqlMapper SqlMapper { get; }

        public LruCacheProviderTest(SmartSqlFixture smartSqlFixture)
        {
            SqlMapper = smartSqlFixture.SqlMapper;
        }

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