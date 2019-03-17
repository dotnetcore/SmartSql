using SmartSql.Test.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace SmartSql.Test.Unit.Cache
{
    public class FifoCacheProviderTest : AbstractXmlConfigBuilderTest
    {
        [Fact]
        public void QueryByFifoCache()
        {
            var list = DbSession.Query<AllPrimitive>(new RequestContext
            {
                Scope = nameof(AllPrimitive),
                SqlId = "QueryByFifoCache"
            });
            var cachedList = DbSession.Query<AllPrimitive>(new RequestContext
            {
                Scope = nameof(AllPrimitive),
                SqlId = "QueryByFifoCache"
            });
            Assert.Equal(list, cachedList);
        }
    }
}
