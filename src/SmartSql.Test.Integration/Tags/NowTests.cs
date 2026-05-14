using FluentAssertions;
using Xunit;

namespace SmartSql.Test.Integration.Tags;

public class NowTests : IntegrationTestBase
{
    public NowTests(SmartSqlFixture fixture) : base(fixture) { }

    [Fact]
    public void Should_AppendNowTimeParameter_When_Resolved()
    {
        var requestCtx = new RequestContext
        {
            Scope = "TagTest",
            SqlId = "Now"
        };
        requestCtx.SetupParameters();

        var statement = SmartSqlConfig.GetStatement(requestCtx.FullSqlId);
        statement.BuildSql(requestCtx);

        requestCtx.Parameters.ContainsKey("NowTime").Should().BeTrue();
        requestCtx.SqlBuilder.ToString().Trim().Should().Be("?NowTime");
    }
}
