using SmartSql.Test.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using SmartSql.Cache;
using Xunit;

namespace SmartSql.Test.Unit.Cache
{
    [Collection("GlobalSmartSql")]
    public class FifoCacheProviderTest
    {
        protected ISqlMapper SqlMapper { get; }

        public FifoCacheProviderTest(SmartSqlFixture smartSqlFixture)
        {
            SqlMapper = smartSqlFixture.SqlMapper;
        }

        [Fact]
        public void QueryByFifoCache()
        {
            var list = SqlMapper.Query<AllPrimitive>(new RequestContext
            {
                Scope = nameof(AllPrimitive),
                SqlId = "QueryByFifoCache",
                Request = new {Taken = 8},
            });
            var cachedList = SqlMapper.Query<AllPrimitive>(new RequestContext
            {
                Scope = nameof(AllPrimitive),
                SqlId = "QueryByFifoCache",
                Request = new {Taken = 8}
            });
            Assert.Equal(list.GetHashCode(), cachedList.GetHashCode());
        }
    }
}