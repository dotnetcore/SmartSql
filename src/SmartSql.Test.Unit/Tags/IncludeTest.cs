using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace SmartSql.Test.Unit.Tags
{
  public  class IncludeTest:AbstractXmlConfigBuilderTest
    {
        [Fact]
        public void Include_Test()
        {
            var msg = DbSession.ExecuteScalar<String>(new RequestContext
            {
                Scope = nameof(IncludeTest),
                SqlId = "Query",
                Request = new { FLong = 1 }
            });
            Assert.True(true);
        }
    }
}
