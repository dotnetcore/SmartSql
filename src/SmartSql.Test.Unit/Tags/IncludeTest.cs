using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace SmartSql.Test.Unit.Tags
{
    [Collection("GlobalSmartSql")]
    public class IncludeTest 
    {
        protected ISqlMapper SqlMapper { get; }

        public IncludeTest(SmartSqlFixture smartSqlFixture)
        {
            SqlMapper = smartSqlFixture.SqlMapper;
        }
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
