using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace SmartSql.Test.Unit.Tags
{
  public  class IncludeTest:AbstractXmlConfigBuilderTest
    {
        protected ISqlMapper SqlMapper => BuildSqlMapper(this.GetType().FullName);

        [Fact]
        public void Include_Test()
        {
            var msg = SqlMapper.ExecuteScalar<String>(new RequestContext
            {
                Scope = nameof(IncludeTest),
                SqlId = "Query",
                Request = new { UserName = "SmartSql" }
            });
            Assert.True(true);
        }
    }
}
