using SmartSql.Test.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace SmartSql.Test.Unit.Cache
{
    public class LruCacheProviderTest : AbstractXmlConfigBuilderTest
    {
        protected ISqlMapper SqlMapper { get; }

        public LruCacheProviderTest()
        {
            SqlMapper = BuildSqlMapper(this.GetType().Name);
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
