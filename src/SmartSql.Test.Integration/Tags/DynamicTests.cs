using FluentAssertions;
using System;
using Xunit;

namespace SmartSql.Test.Integration.Tags;

public class DynamicTests : IntegrationTestBase
{
    public DynamicTests(SmartSqlFixture fixture) : base(fixture) { }

    [Fact]
    public void Should_BuildSql_When_PropertyIsNotEmpty()
    {
        var requestCtx = new RequestContext
        {
            Scope = "TagTest",
            SqlId = "Dynamic",
            Request = new { Property = "Property" }
        };
        requestCtx.SetupParameters();

        var statement = SmartSqlConfig.GetStatement(requestCtx.FullSqlId);
        statement.BuildSql(requestCtx);

        var sql = requestCtx.SqlBuilder.ToString().Trim();
        sql.Should().StartWith("Where");
        sql.Should().Contain("T.Property=?Property");
    }

    [Fact]
    public void Should_BuildEmptySql_When_PropertyIsEmpty()
    {
        var requestCtx = new RequestContext
        {
            Scope = "TagTest",
            SqlId = "Dynamic",
            Request = new { Property = "" }
        };
        requestCtx.SetupParameters();

        var statement = SmartSqlConfig.GetStatement(requestCtx.FullSqlId);
        statement.BuildSql(requestCtx);

        requestCtx.SqlBuilder.ToString().Trim().Should().BeEmpty();
    }
}
