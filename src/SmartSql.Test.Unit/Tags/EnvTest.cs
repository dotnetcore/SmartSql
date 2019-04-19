using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace SmartSql.Test.Unit.Tags
{
    [Collection("GlobalSmartSql")]
    public class EnvTest 
    {
        protected ISqlMapper SqlMapper { get; }

        public EnvTest(SmartSqlFixture smartSqlFixture)
        {
            SqlMapper = smartSqlFixture.SqlMapper;
        }
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
