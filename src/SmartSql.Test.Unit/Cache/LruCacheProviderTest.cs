using SmartSql.Test.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace SmartSql.Test.Unit.Cache
{
    public class LruCacheProviderTest : AbstractXmlConfigBuilderTest
    {
        [Fact]
        public void QueryByLruCache()
        {
            var list = DbSession.Query<AllPrimitive>(new RequestContext
            {
                Scope = nameof(AllPrimitive),
                SqlId = "QueryByLruCache"
            });
            var cachedList = DbSession.Query<AllPrimitive>(new RequestContext
            {
                Scope = nameof(AllPrimitive),
                SqlId = "QueryByLruCache"
            });
            Assert.Equal(list, cachedList);
        }
    }
}
