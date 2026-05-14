using FluentAssertions;
using System;
using SmartSql.Configuration.Tags;
using Xunit;

namespace SmartSql.Test.Integration.Tags;

public class IncludeTests : IntegrationTestBase
{
    public IncludeTests(SmartSqlFixture fixture) : base(fixture) { }

    [Fact]
    public void Should_BuildSql_When_PropertyIsNotEmpty()
    {
        var requestCtx = new RequestContext
        {
            Scope = "TagTest",
            SqlId = "Include",
            Request = new { Property = "Property" }
        };
        requestCtx.SetupParameters();

        var statement = SmartSqlConfig.GetStatement(requestCtx.FullSqlId);
        statement.BuildSql(requestCtx);

        requestCtx.SqlBuilder.ToString().Trim().Should().Be(@"Where
                Property=?Property");
    }

    [Fact]
    public void Should_BuildEmptySql_When_PropertyIsEmpty()
    {
        var requestCtx = new RequestContext
        {
            Scope = "TagTest",
            SqlId = "Include",
            Request = new { Property = "" }
        };
        requestCtx.SetupParameters();

        var statement = SmartSqlConfig.GetStatement(requestCtx.FullSqlId);
        statement.BuildSql(requestCtx);

        requestCtx.SqlBuilder.ToString().Trim().Should().BeEmpty();
    }

    [Fact]
    public void Should_ThrowTagRequiredFailException_When_RequiredIncludeIsEmpty()
    {
        var requestCtx = new RequestContext
        {
            Scope = "TagTest",
            SqlId = "IncludeRequired",
            Request = new { Property = "" }
        };
        requestCtx.SetupParameters();

        var statement = SmartSqlConfig.GetStatement(requestCtx.FullSqlId);
        Action act = () => { statement.BuildSql(requestCtx); };

        act.Should().Throw<TagRequiredFailException>();
    }
}
