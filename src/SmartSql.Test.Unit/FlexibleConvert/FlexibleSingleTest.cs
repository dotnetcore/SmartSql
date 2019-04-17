using System;
using System.Collections.Generic;
using System.Text;
using SmartSql.Test.Entities;
using Xunit;

namespace SmartSql.Test.Unit.FlexibleConvert
{
    public class FlexibleSingleTest : FlexibleTest
    {
        [Fact]
        public void Test()
        {
            var entity = DbSession.QuerySingle<FlexibleSingle>(new RequestContext
            {
                RealSql = SQL
            });
            Assert.Equal(1, entity.Single);
        }
    }
}
