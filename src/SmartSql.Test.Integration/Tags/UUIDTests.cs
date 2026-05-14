using FluentAssertions;
using Xunit;

namespace SmartSql.Test.Integration.Tags;

public class UUIDTests : IntegrationTestBase
{
    public UUIDTests(SmartSqlFixture fixture) : base(fixture) { }

    [Fact]
    public void Should_GenerateUUID_When_Resolved()
    {
        var requestCtx = new RequestContext
        {
            Scope = "TagTest",
            SqlId = "UUID"
        };
        requestCtx.SetupParameters();

        var statement = SmartSqlConfig.GetStatement(requestCtx.FullSqlId);
        statement.BuildSql(requestCtx);

        requestCtx.Parameters.ContainsKey("UUID").Should().BeTrue();
        requestCtx.Parameters["UUID"].Value.ToString().Should().Contain("-");
        requestCtx.SqlBuilder.ToString().Trim().Should().Be("?UUID");
    }

    [Fact]
    public void Should_GenerateUUIDWithoutDashes_When_ResolvedToN()
    {
        var requestCtx = new RequestContext
        {
            Scope = "TagTest",
            SqlId = "UUIDToN"
        };
        requestCtx.SetupParameters();

        var statement = SmartSqlConfig.GetStatement(requestCtx.FullSqlId);
        statement.BuildSql(requestCtx);

        requestCtx.Parameters.ContainsKey("UUID").Should().BeTrue();
        requestCtx.Parameters["UUID"].Value.ToString().Should().NotContain("-");
        requestCtx.SqlBuilder.ToString().Trim().Should().Be("?UUID");
    }
}
