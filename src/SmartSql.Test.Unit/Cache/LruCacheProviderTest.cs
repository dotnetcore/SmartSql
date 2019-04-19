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
                SqlId = "QueryByLruCache"
            });
            var cachedList = SqlMapper.Query<AllPrimitive>(new RequestContext
            {
                Scope = nameof(AllPrimitive),
                SqlId = "QueryByLruCache"
            });
            Assert.Equal(list.GetHashCode(), cachedList.GetHashCode());
        }
    }
}
