using System;
using System.Collections.Generic;
using System.Text;
using SmartSql.Test.Entities;
using Xunit;

namespace SmartSql.Test.Unit.FlexibleConvert
{
    [Collection("GlobalSmartSql")]
    public class FlexibleSingleTest : FlexibleTest
    {
        protected ISqlMapper SqlMapper { get; }

        public FlexibleSingleTest(SmartSqlFixture smartSqlFixture)
        {
            SqlMapper = smartSqlFixture.SqlMapper;
        }
        [Fact]
        public void Test()
        {
            var entity = SqlMapper.QuerySingle<FlexibleSingle>(new RequestContext
            {
                RealSql = SQL
            });
            Assert.Equal(1, entity.Single);
        }
    }
}
