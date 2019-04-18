using System;
using System.Collections.Generic;
using System.Text;
using SmartSql.Test.Entities;
using Xunit;

namespace SmartSql.Test.Unit.FlexibleConvert
{
    public class FlexibleBooleanTest : FlexibleTest
    {
        protected ISqlMapper SqlMapper => BuildSqlMapper(this.GetType().FullName);

        [Fact]
        public void Test()
        {
            var entity = SqlMapper.QuerySingle<FlexibleBoolean>(new RequestContext
            {
                RealSql = @"Select 
Convert(bit,1) As Boolean,
Convert(tinyint,1) As Byte,
Convert(char,'true') As Char,
Convert(smallint,1) As Int16,
Convert(int,1) As Int32,
Convert(bigint,1) As Int64,
Convert(decimal,1) As Decimal,
'true' As String"
            });
            Assert.True(entity.Boolean);
        }
    }
}
