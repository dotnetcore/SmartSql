using SmartSql.Test.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using System.Linq;

namespace SmartSql.Test.Unit.Cache
{
    public class RedisCacheProviderTest : AbstractXmlConfigBuilderTest
    {
        [Fact]
        public void QueryByRedisCache()
        {
            var list = DbSession.Query<AllPrimitive>(new RequestContext
            {
                Scope = nameof(AllPrimitive),
                SqlId = "QueryByRedisCache"
            });
            var cachedList = DbSession.Query<AllPrimitive>(new RequestContext
            {
                Scope = nameof(AllPrimitive),
                SqlId = "QueryByRedisCache"
            });
            Assert.Equal(list.Count(), cachedList.Count());
        }
    }
}
