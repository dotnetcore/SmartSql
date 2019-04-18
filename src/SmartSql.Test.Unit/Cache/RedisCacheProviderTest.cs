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
        protected ISqlMapper SqlMapper { get; }
        public RedisCacheProviderTest()
        {
            SqlMapper = BuildSqlMapper(this.GetType().FullName);
        }
        //[Fact]
        public void QueryByRedisCache()
        {
            var list = SqlMapper.Query<AllPrimitive>(new RequestContext
            {
                Scope = nameof(AllPrimitive),
                SqlId = "QueryByRedisCache"
            });
            var cachedList = SqlMapper.Query<AllPrimitive>(new RequestContext
            {
                Scope = nameof(AllPrimitive),
                SqlId = "QueryByRedisCache"
            });
            Assert.Equal(list.Count(), cachedList.Count());
        }
    }
}
