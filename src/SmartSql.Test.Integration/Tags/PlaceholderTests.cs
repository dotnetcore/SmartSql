using FluentAssertions;
using Xunit;

namespace SmartSql.Test.Integration.Tags;

public class PlaceholderTests : IntegrationTestBase
{
    public PlaceholderTests(SmartSqlFixture fixture) : base(fixture) { }

    [Fact]
    public void Should_BuildSql_When_PlaceholderIsSet()
    {
        var requestCtx = new RequestContext
        {
            Scope = "TagTest",
            SqlId = "Placeholder",
            Request = new { Placeholder = "Placeholder" }
        };
        requestCtx.SetupParameters();

        var statement = SmartSqlConfig.GetStatement(requestCtx.FullSqlId);
        statement.BuildSql(requestCtx);

        requestCtx.SqlBuilder.ToString().Trim().Should().Be("Placeholder");
    }

    [Fact]
    public void Should_BuildSql_When_NestPlaceholderIsSet()
    {
        var requestCtx = new RequestContext
        {
            Scope = "TagTest",
            SqlId = "NestPlaceholder",
            Request = new { Nest = new { Placeholder = "Placeholder" } }
        };
        requestCtx.SetupParameters();

        var statement = SmartSqlConfig.GetStatement(requestCtx.FullSqlId);
        statement.BuildSql(requestCtx);

        requestCtx.SqlBuilder.ToString().Trim().Should().Be("Placeholder");
    }
}
