using SmartSql.Test.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace SmartSql.Test.Unit.Tags
{
    public class SetTest : AbstractXmlConfigBuilderTest
    {
        [Fact]
        public void Set_Test()
        {
            var user = SqlMapper.QuerySingle<User>(new RequestContext
            {
                Scope = nameof(SetTest),
                SqlId = "UpdateUser",
                Request = new { UserName = "SmartSql",Status=1 }
            });
            Assert.True(true);
        }

    }
}
