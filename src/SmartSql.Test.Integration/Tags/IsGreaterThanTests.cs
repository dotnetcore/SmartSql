using FluentAssertions;
using System;
using SmartSql.Configuration.Tags;
using Xunit;

namespace SmartSql.Test.Integration.Tags;

public class IsGreaterThanTests : IntegrationTestBase
{
    public IsGreaterThanTests(SmartSqlFixture fixture) : base(fixture) { }

    [Fact]
    public void Should_BuildSql_When_PropertyIsGreaterThan()
    {
        var requestCtx = new RequestContext
        {
            Scope = "TagTest",
            SqlId = "IsGreaterThan",
            Request = new { Property = 11 }
        };
        requestCtx.SetupParameters();

        var statement = SmartSqlConfig.GetStatement(requestCtx.FullSqlId);
        statement.BuildSql(requestCtx);

        requestCtx.SqlBuilder.ToString().Trim().Should().Be("Property IsGreaterThan 10");
    }

    [Fact]
    public void Should_ReturnEmpty_When_PropertyIsNotGreaterThan()
    {
        var requestCtx = new RequestContext
        {
            Scope = "TagTest",
            SqlId = "IsGreaterThan",
            Request = new { Property = 10 }
        };
        requestCtx.SetupParameters();

        var statement = SmartSqlConfig.GetStatement(requestCtx.FullSqlId);
        statement.BuildSql(requestCtx);

        requestCtx.SqlBuilder.ToString().Trim().Should().BeEmpty();
    }

    [Fact]
    public void Should_Throw_When_RequiredPropertyIsNull()
    {
        var requestCtx = new RequestContext
        {
            Scope = "TagTest",
            SqlId = "IsGreaterThanRequired"
        };
        requestCtx.SetupParameters();

        var statement = SmartSqlConfig.GetStatement(requestCtx.FullSqlId);
        Action act = () => statement.BuildSql(requestCtx);

        act.Should().Throw<TagRequiredFailException>();
    }
}
