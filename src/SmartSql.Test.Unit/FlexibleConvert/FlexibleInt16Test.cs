using System;
using System.Collections.Generic;
using System.Text;
using SmartSql.Test.Entities;
using Xunit;

namespace SmartSql.Test.Unit.FlexibleConvert
{
    public class FlexibleInt16Test : FlexibleTest
    {
        protected ISqlMapper SqlMapper => BuildSqlMapper(this.GetType().FullName);

        [Fact]
        public void Test()
        {
            var entity = SqlMapper.QuerySingle<FlexibleInt16>(new RequestContext
            {
                RealSql = SQL
            });
            Assert.Equal(1, entity.Int16);
        }
    }
}
