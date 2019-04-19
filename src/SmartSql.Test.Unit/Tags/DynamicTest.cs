using SmartSql.Test.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace SmartSql.Test.Unit.Tags
{
    public class DynamicTest : AbstractXmlConfigBuilderTest
    {
        [Fact]
        public void Dynamic_Test()
        {
            var user = SqlMapper.QuerySingle<User>(new RequestContext
            {
                Scope = nameof(DynamicTest),
                SqlId = "GetUser",
                Request = new { UserName = "SmartSql" }
            });
            Assert.True(true);
        }

    }
}
