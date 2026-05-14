using Xunit;

namespace SmartSql.Test.Integration.Tags
{
    public class PlaceholderTest : IntegrationTestBase
    {
        public PlaceholderTest(SmartSqlFixture fixture) : base(fixture) { }

        [Fact]
        public void Placeholder()
        {
            var requestCtx = new RequestContext
            {
                Scope = "TagTest",
                SqlId = nameof(Placeholder),
                Request = new { Placeholder = "Placeholder" }
            };
            requestCtx.SetupParameters();

            var statement = SmartSqlConfig.GetStatement(requestCtx.FullSqlId);
            statement.BuildSql(requestCtx);

            Assert.Equal("Placeholder", requestCtx.SqlBuilder.ToString().Trim());
        }

        [Fact]
        public void NestPlaceholder()
        {
            var requestCtx = new RequestContext
            {
                Scope = "TagTest",
                SqlId = nameof(NestPlaceholder),
                Request = new { Nest = new { Placeholder = "Placeholder" } }
            };
            requestCtx.SetupParameters();

            var statement = SmartSqlConfig.GetStatement(requestCtx.FullSqlId);
            statement.BuildSql(requestCtx);
            Assert.Equal("Placeholder", requestCtx.SqlBuilder.ToString().Trim());
        }
    }
}
