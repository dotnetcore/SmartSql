using System;
using System.Collections.Generic;
using System.Text;
using SmartSql.Test.Entities;
using Xunit;

namespace SmartSql.Test.Unit.FlexibleConvert
{
    [Collection("GlobalSmartSql")]
    public class FlexibleStringTest : FlexibleTest
    {
        protected ISqlMapper SqlMapper { get; }

        public FlexibleStringTest(SmartSqlFixture smartSqlFixture)
        {
            SqlMapper = smartSqlFixture.SqlMapper;
        }
        [Fact]
        public void Test()
        {
            var entity = SqlMapper.QuerySingle<FlexibleString>(new RequestContext
            {
                RealSql = SQL
            });
            Assert.Equal("1", entity.String);
        }
    }
}
