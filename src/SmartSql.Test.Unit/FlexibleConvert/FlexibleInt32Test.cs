using System;
using System.Collections.Generic;
using System.Text;
using SmartSql.Test.Entities;
using Xunit;

namespace SmartSql.Test.Unit.FlexibleConvert
{
    [Collection("GlobalSmartSql")]
    public class FlexibleInt32Test : FlexibleTest
    {
        protected ISqlMapper SqlMapper { get; }

        public FlexibleInt32Test(SmartSqlFixture smartSqlFixture)
        {
            SqlMapper = smartSqlFixture.SqlMapper;
        }
        [Fact]
        public void Test()
        {
            var entity = SqlMapper.QuerySingle<FlexibleInt32>(new RequestContext
            {
                RealSql = SQL
            });
            Assert.Equal(1, entity.Int32);
        }
    }
}
