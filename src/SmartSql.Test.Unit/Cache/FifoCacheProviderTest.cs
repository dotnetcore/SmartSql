using SmartSql.Test.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace SmartSql.Test.Unit.Cache
{
    public class FifoCacheProviderTest : AbstractXmlConfigBuilderTest
    {
        protected ISqlMapper SqlMapper { get; }

        public FifoCacheProviderTest()
        {
            SqlMapper= BuildSqlMapper(this.GetType().Name);
        }

        [Fact]
        public void QueryByFifoCache()
        {
            var list = SqlMapper.Query<AllPrimitive>(new RequestContext
            {
                Scope = nameof(AllPrimitive),
                SqlId = "QueryByFifoCache"
            });
            var cachedList = SqlMapper.Query<AllPrimitive>(new RequestContext
            {
                Scope = nameof(AllPrimitive),
                SqlId = "QueryByFifoCache"
            });
            Assert.Equal(list.GetHashCode(), cachedList.GetHashCode());
        }
    }
}
