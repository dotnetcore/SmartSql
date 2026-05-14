using System;
using FluentAssertions;
using SmartSql.Configuration.Tags;
using Xunit;

namespace SmartSql.Test.Integration.Tags;

public class WhereTests : IntegrationTestBase
{
    public WhereTests(SmartSqlFixture fixture) : base(fixture) { }

    [Fact]
    public void Should_BuildSql_When_RequestHasProperty()
    {
        var requestCtx = new RequestContext
        {
            Scope = "TagTest",
            SqlId = "Where",
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
    public void Should_ReturnEmpty_When_RequestIsEmpty()
    {
        var requestCtx = new RequestContext
        {
            Scope = "TagTest",
            SqlId = "Where"
        };
        requestCtx.SetupParameters();

        var statement = SmartSqlConfig.GetStatement(requestCtx.FullSqlId);
        statement.BuildSql(requestCtx);

        requestCtx.SqlBuilder.ToString().Trim().Should().BeEmpty();
    }

    [Fact]
    public void Should_BuildSql_When_MinMatchRequestHasProperty()
    {
        var requestCtx = new RequestContext
        {
            Scope = "TagTest",
            SqlId = "WhereMin",
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
    public void Should_Throw_When_MinMatchRequestIsEmpty()
    {
        var requestCtx = new RequestContext
        {
            Scope = "TagTest",
            SqlId = "WhereMin"
        };
        requestCtx.SetupParameters();

        var statement = SmartSqlConfig.GetStatement(requestCtx.FullSqlId);
        Action act = () => statement.BuildSql(requestCtx);

        act.Should().Throw<TagMinMatchedFailException>();
    }
}
