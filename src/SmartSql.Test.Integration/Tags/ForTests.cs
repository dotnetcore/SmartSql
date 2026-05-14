using FluentAssertions;
using System;
using SmartSql.Configuration;
using Xunit;

namespace SmartSql.Test.Integration.Tags;

public class ForTests : IntegrationTestBase
{
    public ForTests(SmartSqlFixture fixture) : base(fixture) { }

    [Fact]
    public void Should_BuildSql_When_DirectValue()
    {
        var requestCtx = new RequestContext
        {
            Scope = "TagTest",
            SqlId = "ForWhenDirectValue",
            Request = new { Items = new[] { 1, 2 } }
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

        var sql = requestCtx.SqlBuilder.ToString().Trim();
        sql.Should().StartWith("(");
        sql.Should().EndWith(")");
        sql.Should().Contain("?Item_For__0");
        sql.Should().Contain("?Item_For__1");

        requestCtx.Parameters["Item_For__0"].Value.Should().Be(1);
        requestCtx.Parameters["Item_For__1"].Value.Should().Be(2);
    }

    [Fact]
    public void Should_BuildSql_When_NotDirectValue()
    {
        var requestCtx = new RequestContext
        {
            Scope = "TagTest",
            SqlId = "ForWhenNotDirectValue",
            Request = new { Items = new[] { new { Id = 1 }, new { Id = 2 } } }
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

        var sql = requestCtx.SqlBuilder.ToString().Trim();
        sql.Should().StartWith("(");
        sql.Should().EndWith(")");
        sql.Should().Contain("?Item_For__Id_0");
        sql.Should().Contain("?Item_For__Id_1");

        requestCtx.Parameters["Item_For__Id_0"].Value.Should().Be(1);
        requestCtx.Parameters["Item_For__Id_1"].Value.Should().Be(2);
    }

    [Fact]
    public void Should_BuildSql_When_NotDirectValueWithKey()
    {
        var requestCtx = new RequestContext
        {
            Scope = "TagTest",
            SqlId = "ForWhenNotDirectValueWithKey",
            Request = new { Items = new[] { new { Id = 1 }, new { Id = 2 } } }
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

        var sql = requestCtx.SqlBuilder.ToString().Trim();
        sql.Should().StartWith("(");
        sql.Should().EndWith(")");
        sql.Should().Contain("?Item_For__Id_0");
        sql.Should().Contain("?Item_For__Id_1");

        requestCtx.Parameters["Item_For__Id_0"].Value.Should().Be(1);
        requestCtx.Parameters["Item_For__Id_1"].Value.Should().Be(2);
    }

    [Fact]
    public void Should_BuildSql_When_NotDirectNestedValueWithKey()
    {
        var requestCtx = new RequestContext
        {
            Scope = "TagTest",
            SqlId = "ForWhenNotDirectNestValueWithKey",
            Request = new
            {
                Items = new[]
                {
                    new { Info = new { Id = 1 } },
                    new { Info = new { Id = 2 } }
                }
            }
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

        var sql = requestCtx.SqlBuilder.ToString().Trim();
        sql.Should().StartWith("(");
        sql.Should().EndWith(")");
        sql.Should().Contain("?Item_For__Info_Id_0");
        sql.Should().Contain("?Item_For__Info_Id_1");

        requestCtx.Parameters["Item_For__Info_Id_0"].Value.Should().Be(1);
        requestCtx.Parameters["Item_For__Info_Id_1"].Value.Should().Be(2);
    }
}
