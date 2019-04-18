using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace SmartSql.Test.Unit.Tags
{
    public class EnvTest : AbstractXmlConfigBuilderTest
    {
        protected ISqlMapper SqlMapper => BuildSqlMapper(this.GetType().FullName);

        [Fact]
        public void Env_Test()
        {
            var msg = SqlMapper.ExecuteScalar<String>(new RequestContext
            {
                Scope = nameof(EnvTest),
                SqlId = "GetEntity",
                Request = new { UserName = "SmartSql" }
            });
            Assert.True(true);
        }

    }
}
