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
        public void QueryByLruCache()
        {
            var list = SqlMapper.Query<AllPrimitive>(new RequestContext
            {
                Scope = nameof(AllPrimitive),
                SqlId = "QueryByLruCache",
                Request = new {Taken = 8}
            });
            var cachedList = SqlMapper.Query<AllPrimitive>(new RequestContext
            {
                Scope = nameof(AllPrimitive),
                SqlId = "QueryByLruCache",
                Request = new {Taken = 8}
            });
            Assert.Equal(list.GetHashCode(), cachedList.GetHashCode());
        }

        [Fact]
        public void QueryByLruCacheFromRequest()
        {
            var list = SqlMapper.Query<AllPrimitive>(new RequestContext
            {
                Scope = nameof(AllPrimitive),
                SqlId = "QueryByLruCacheFromRequest",
                Request = new {Request = new {Taken = 8}}
            });
            var cachedList = SqlMapper.Query<AllPrimitive>(new RequestContext
            {
                Scope = nameof(AllPrimitive),
                SqlId = "QueryByLruCacheFromRequest",
                Request = new {Request = new {Taken = 8}}
            });
            Assert.Equal(list.GetHashCode(), cachedList.GetHashCode());
        }
    }
}