using System;
using System.Collections.Generic;
using System.Text;
using SmartSql.Test.Entities;
using Xunit;

namespace SmartSql.Test.Unit.FlexibleConvert
{
    [Collection("GlobalSmartSql")]
    public class FlexibleInt64Test : FlexibleTest
    {
        protected ISqlMapper SqlMapper { get; }

        public FlexibleInt64Test(SmartSqlFixture smartSqlFixture)
        {
            SqlMapper = smartSqlFixture.SqlMapper;
        }
        [Fact]
        public void Test()
        {
            var entity = SqlMapper.QuerySingle<FlexibleInt64>(new RequestContext
            {
                RealSql = SQL
            });
            Assert.Equal(1, entity.Int64);
        }
    }
}
