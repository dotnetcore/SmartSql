using Xunit;

namespace SmartSql.Test.Integration.Tags
{
    public class NowTest : IntegrationTestBase
    {
        public NowTest(SmartSqlFixture fixture) : base(fixture) { }

        [Fact]
        public void AppendNowTime()
        {
            var requestCtx = new RequestContext
            {
                Scope = "TagTest",
                SqlId = "Now"
            };
            requestCtx.SetupParameters();

            var statement = SmartSqlConfig.GetStatement(requestCtx.FullSqlId);
            statement.BuildSql(requestCtx);

            Assert.True(requestCtx.Parameters.ContainsKey("NowTime"));
            Assert.Equal("?NowTime", requestCtx.SqlBuilder.ToString().Trim());
        }
    }
}
