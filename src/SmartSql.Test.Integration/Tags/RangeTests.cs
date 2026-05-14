using FluentAssertions;
using System;
using SmartSql.Configuration.Tags;
using Xunit;

namespace SmartSql.Test.Integration.Tags;

public class RangeTests : IntegrationTestBase
{
    public RangeTests(SmartSqlFixture fixture) : base(fixture) { }

    [Fact]
    public void Should_BuildSql_When_PropertyIsInRange()
    {
        var requestCtx = new RequestContext
        {
            Scope = "TagTest",
            SqlId = "Range",
            Request = new { Property = 0 }
        };
        requestCtx.SetupParameters();

        var statement = SmartSqlConfig.GetStatement(requestCtx.FullSqlId);
        statement.BuildSql(requestCtx);

        requestCtx.SqlBuilder.ToString().Trim().Should().Be("Property Between 0 And 10");
    }

    [Fact]
    public void Should_ReturnEmpty_When_PropertyIsOutOfRange()
    {
        var requestCtx = new RequestContext
        {
            Scope = "TagTest",
            SqlId = "Range",
            Request = new { Property = 11 }
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
            SqlId = "RangeRequired"
        };
        requestCtx.SetupParameters();

        var statement = SmartSqlConfig.GetStatement(requestCtx.FullSqlId);
        Action act = () => statement.BuildSql(requestCtx);

        act.Should().Throw<TagRequiredFailException>();
    }
}
