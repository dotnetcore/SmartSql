using FluentAssertions;
using System;
using SmartSql.Configuration.Tags;
using Xunit;

namespace SmartSql.Test.Integration.Tags;

public class IsLessThanTests : IntegrationTestBase
{
    public IsLessThanTests(SmartSqlFixture fixture) : base(fixture) { }

    [Fact]
    public void Should_BuildSql_When_PropertyIsLessThanCompareValue()
    {
        var requestCtx = new RequestContext
        {
            Scope = "TagTest",
            SqlId = "IsLessThan",
            Request = new { Property = 9 }
        };
        requestCtx.SetupParameters();

        var statement = SmartSqlConfig.GetStatement(requestCtx.FullSqlId);
        statement.BuildSql(requestCtx);

        requestCtx.SqlBuilder.ToString().Trim().Should().Be("Property IsLessThan 10");
    }

    [Fact]
    public void Should_BuildEmptySql_When_PropertyIsNotLessThanCompareValue()
    {
        var requestCtx = new RequestContext
        {
            Scope = "TagTest",
            SqlId = "IsLessThan",
            Request = new { Property = 10 }
        };
        requestCtx.SetupParameters();

        var statement = SmartSqlConfig.GetStatement(requestCtx.FullSqlId);
        statement.BuildSql(requestCtx);

        requestCtx.SqlBuilder.ToString().Trim().Should().BeEmpty();
    }

    [Fact]
    public void Should_ThrowTagRequiredFailException_When_RequiredPropertyIsEmpty()
    {
        var requestCtx = new RequestContext
        {
            Scope = "TagTest",
            SqlId = "IsLessThanRequired"
        };
        requestCtx.SetupParameters();

        var statement = SmartSqlConfig.GetStatement(requestCtx.FullSqlId);
        Action act = () => { statement.BuildSql(requestCtx); };

        act.Should().Throw<TagRequiredFailException>();
    }
}
