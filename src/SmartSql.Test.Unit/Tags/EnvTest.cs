using System;
using System.Collections.Generic;
using System.Text;
using SmartSql.Configuration;
using Xunit;

namespace SmartSql.Test.Unit.Tags
{
    [Collection("GlobalSmartSql")]
    public class EnvTest
    {
        SmartSqlConfig SmartSqlConfig { get; }

        public EnvTest(SmartSqlFixture smartSqlFixture)
        {
            SmartSqlConfig = smartSqlFixture.SqlMapper.SmartSqlConfig;
        }

        [Fact]
        public void BuildSql()
        {
            var requestCtx = new RequestContext
            {
                Scope = "TagTest",
                SqlId = "Env",
                Request = new { Property = "Property" }
            };

            var executionContext = new ExecutionContext
            {
                Request = requestCtx,
                SmartSqlConfig = SmartSqlConfig
            };
            requestCtx.ExecutionContext = executionContext;
            requestCtx.SetupParameters();

            var statement = SmartSqlConfig.GetStatement(requestCtx.FullSqlId);
            statement.BuildSql(requestCtx);

            Assert.Equal("Mysql", requestCtx.SqlBuilder.ToString().Trim());
        }
    }
}