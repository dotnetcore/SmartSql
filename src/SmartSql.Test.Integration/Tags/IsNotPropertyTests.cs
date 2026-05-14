using FluentAssertions;
using System;
using Xunit;

namespace SmartSql.Test.Integration.Tags;

public class IsNotPropertyTests : IntegrationTestBase
{
    public IsNotPropertyTests(SmartSqlFixture fixture) : base(fixture) { }

    [Fact]
    public void Should_BuildSql_When_PropertyIsNotPresent()
    {
        var requestCtx = new RequestContext
        {
            Scope = "TagTest",
            SqlId = "IsNotProperty"
        };
        requestCtx.SetupParameters();

        var statement = SmartSqlConfig.GetStatement(requestCtx.FullSqlId);
        statement.BuildSql(requestCtx);

        requestCtx.SqlBuilder.ToString().Trim().Should().Be("IsNotProperty");
    }

    [Fact]
    public void Should_BuildEmptySql_When_PropertyIsPresent()
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

        requestCtx.SqlBuilder.ToString().Trim().Should().BeEmpty();
    }
}
