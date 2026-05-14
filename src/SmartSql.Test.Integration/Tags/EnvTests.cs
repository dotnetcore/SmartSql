using FluentAssertions;
using SmartSql.Configuration;
using Xunit;

namespace SmartSql.Test.Integration.Tags;

public class EnvTests : IntegrationTestBase
{
    public EnvTests(SmartSqlFixture fixture) : base(fixture) { }

    [Fact]
    public void Should_BuildSql_When_EnvMatchesDatabaseProvider()
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

        requestCtx.SqlBuilder.ToString().Trim().Should().Be("Mysql");
    }
}
