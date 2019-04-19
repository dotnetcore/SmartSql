using System;
using System.Collections.Generic;
using System.Text;
using SmartSql.Test.Entities;
using Xunit;

namespace SmartSql.Test.Unit.FlexibleConvert
{
    [Collection("GlobalSmartSql")]
    public class FlexibleByteTest : FlexibleTest
    {
        protected ISqlMapper SqlMapper { get; }

        public FlexibleByteTest(SmartSqlFixture smartSqlFixture)
        {
            SqlMapper = smartSqlFixture.SqlMapper;
        }
        [Fact]
        public void Test()
        {
            var entity = SqlMapper.QuerySingle<FlexibleByte>(new RequestContext
            {
                RealSql = SQL
            });
            Assert.Equal(1, entity.Byte);
        }
    }
}
