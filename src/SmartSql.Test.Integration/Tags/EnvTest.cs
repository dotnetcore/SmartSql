using SmartSql.Configuration;
using Xunit;

namespace SmartSql.Test.Integration.Tags
{
    public class EnvTest : IntegrationTestBase
    {
        public EnvTest(SmartSqlFixture fixture) : base(fixture) { }

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
