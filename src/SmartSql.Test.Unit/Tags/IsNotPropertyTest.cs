using System;
using Xunit;

namespace SmartSql.Test.Unit.Tags
{
    [Collection("GlobalSmartSql")]
    public class IsNotPropertyTest 
    {
        protected ISqlMapper SqlMapper { get; }

        public IsNotPropertyTest(SmartSqlFixture smartSqlFixture)
        {
            SqlMapper = smartSqlFixture.SqlMapper;
        }
        // TODO
        [Fact(Skip = "TODO")]
        public void IsNotProperty_Test()
        {
            var msg = SqlMapper.ExecuteScalar<String>(new RequestContext
            {
                Scope = nameof(IsNotPropertyTest),
                SqlId = "IsNotProperty_Test"
            });
            Assert.Equal("IsNotProperty IsDelete",msg);
        }
    }
}