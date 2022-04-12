using SmartSql.Test.Entities;
using System;
using SmartSql.Cache.Default;
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
        public void GetByCache()
        {
            var expected = SqlMapper.QuerySingle<CachedEntity>(new RequestContext
            {
                Scope = "FifoCache",
                SqlId = "GetByCache"
            });
            var actual = SqlMapper.QuerySingle<CachedEntity>(new RequestContext
            {
                Scope = "FifoCache",
                SqlId = "GetByCache"
            });
            Assert.Equal(expected, actual);
        }
    }
}