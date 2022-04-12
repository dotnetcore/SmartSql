using System;
using SmartSql.Configuration;
using Xunit;

namespace SmartSql.Test.Unit.Tags
{
    [Collection("GlobalSmartSql")]
    public class IsNotPropertyTest 
    {
        SmartSqlConfig SmartSqlConfig { get; }

        public IsNotPropertyTest(SmartSqlFixture smartSqlFixture)
        {
            SmartSqlConfig = smartSqlFixture.SqlMapper.SmartSqlConfig;
        }

        [Fact]
        public void BuildSql()
        {
            var requestCtx = new RequestContext
            {
                Scope = "TagTest",
                SqlId = "IsNotProperty"
            };
            requestCtx.SetupParameters();

            var statement = SmartSqlConfig.GetStatement(requestCtx.FullSqlId);
            statement.BuildSql(requestCtx);

            Assert.Equal("IsNotProperty", requestCtx.SqlBuilder.ToString().Trim());
        }
        
        [Fact]
        public void BuildSqlWhenHasProperty()
        {
            var requestCtx = new RequestContext
            {
                Scope = "TagTest",
                SqlId = "IsNotProperty",
                Request = new { Property = true }
            };
            requestCtx.SetupParameters();

            var statement = SmartSqlConfig.GetStatement(requestCtx.FullSqlId);
            statement.BuildSql(requestCtx);

            Assert.Equal(String.Empty, requestCtx.SqlBuilder.ToString().Trim());
        }
    }
}