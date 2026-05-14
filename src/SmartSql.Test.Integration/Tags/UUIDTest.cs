using Xunit;

namespace SmartSql.Test.Integration.Tags
{
    public class UUIDTest : IntegrationTestBase
    {
        public UUIDTest(SmartSqlFixture fixture) : base(fixture) { }

        [Fact]
        public void UUID()
        {
            var requestCtx = new RequestContext
            {
                Scope = "TagTest",
                SqlId = "UUID"
            };
            requestCtx.SetupParameters();

            var statement = SmartSqlConfig.GetStatement(requestCtx.FullSqlId);
            statement.BuildSql(requestCtx);

            Assert.True(requestCtx.Parameters.ContainsKey("UUID"));
            Assert.True(requestCtx.Parameters["UUID"].Value.ToString().Contains("-"));
            Assert.Equal("?UUID", requestCtx.SqlBuilder.ToString().Trim());
        }

        [Fact]
        public void UUIDToN()
        {
            var requestCtx = new RequestContext
            {
                Scope = "TagTest",
                SqlId = "UUIDToN"
            };
            requestCtx.SetupParameters();

            var statement = SmartSqlConfig.GetStatement(requestCtx.FullSqlId);
            statement.BuildSql(requestCtx);

            Assert.True(requestCtx.Parameters.ContainsKey("UUID"));
            Assert.False(requestCtx.Parameters["UUID"].Value.ToString().Contains("-"));
            Assert.Equal("?UUID", requestCtx.SqlBuilder.ToString().Trim());
        }
    }
}
