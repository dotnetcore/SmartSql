using System;
using System.Collections.Generic;
using System.Text;
using SmartSql.Test.Entities;
using Xunit;

namespace SmartSql.Test.Unit.FlexibleConvert
{
    [Collection("GlobalSmartSql")]
    public class FlexibleBooleanTest : FlexibleTest
    {
        protected ISqlMapper SqlMapper { get; }

        public FlexibleBooleanTest(SmartSqlFixture smartSqlFixture)
        {
            SqlMapper = smartSqlFixture.SqlMapper;
        }
        // TODO
        [Fact(Skip = "TODO")]
        public void Test()
        {
            var entity = SqlMapper.QuerySingle<FlexibleBoolean>(new RequestContext
            {
                RealSql = @"Select 
                convert(1,bool) As Boolean,
                convert(1,tinyint) As Byte,
                convert('true',char) As Char,
                convert(1,smallint) As Int16,
                convert(1,int) As Int32,
                convert(1,bigint) As Int64,
                convert(1,decimal) As Decimal,
                'true' As String"
            });
            Assert.True(entity.Boolean);
        }
    }
}
