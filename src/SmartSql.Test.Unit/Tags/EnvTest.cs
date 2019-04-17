using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace SmartSql.Test.Unit.Tags
{
   public class EnvTest : AbstractXmlConfigBuilderTest
    {
        [Fact]
        public void Env_Test()
        {
            var msg = DbSession.ExecuteScalar<String>(new RequestContext
            {
                Scope = nameof(EnvTest),
                SqlId = "GetEntity",
                Request = new { FLong = 11 }
            });
          Assert.True(true);
        }

    }
}
