using FluentAssertions;
using System;
using System.Collections.Generic;
using SmartSql.Configuration.Tags;
using Xunit;

namespace SmartSql.Test.Integration.Tags;

public class IsNotEmptyTests : IntegrationTestBase
{
    public IsNotEmptyTests(SmartSqlFixture fixture) : base(fixture) { }

    [Fact]
    public void Should_BuildSql_When_PropertyIsNotEmpty()
    {
        var requestCtx = new RequestContext
        {
            Scope = "TagTest",
            SqlId = "IsNotEmpty",
            Request = new { Property = true }
        };
        requestCtx.SetupParameters();

        var statement = SmartSqlConfig.GetStatement(requestCtx.FullSqlId);
        statement.BuildSql(requestCtx);

        requestCtx.SqlBuilder.ToString().Trim().Should().Be("Property IsNotEmpty");
    }

    [Fact]
    public void Should_ReturnEmpty_When_StringIsEmpty()
    {
        var requestCtx = new RequestContext
        {
            Scope = "TagTest",
            SqlId = "IsNotEmpty",
            Request = new { Property = "" }
        };
        requestCtx.SetupParameters();

        var statement = SmartSqlConfig.GetStatement(requestCtx.FullSqlId);
        statement.BuildSql(requestCtx);

        requestCtx.SqlBuilder.ToString().Trim().Should().BeEmpty();
    }

    [Fact]
    public void Should_ReturnEmpty_When_ListIsEmpty()
    {
        var requestCtx = new RequestContext
        {
            Scope = "TagTest",
            SqlId = "IsNotEmpty",
            Request = new { Property = new List<String>() }
        };
        requestCtx.SetupParameters();

        var statement = SmartSqlConfig.GetStatement(requestCtx.FullSqlId);
        statement.BuildSql(requestCtx);

        requestCtx.SqlBuilder.ToString().Trim().Should().BeEmpty();
    }

    [Fact]
    public void Should_ReturnEmpty_When_PropertyIsNull()
    {
        var requestCtx = new RequestContext
        {
            Scope = "TagTest",
            SqlId = "IsNotEmpty"
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
            SqlId = "IsNotEmptyRequired"
        };
        requestCtx.SetupParameters();

        var statement = SmartSqlConfig.GetStatement(requestCtx.FullSqlId);
        Action act = () => statement.BuildSql(requestCtx);

        act.Should().Throw<TagRequiredFailException>();
    }
}
