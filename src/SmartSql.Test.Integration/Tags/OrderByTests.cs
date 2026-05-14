using FluentAssertions;
using System;
using System.Collections.Generic;
using Xunit;

namespace SmartSql.Test.Integration.Tags;

public class OrderByTests : IntegrationTestBase
{
    public OrderByTests(SmartSqlFixture fixture) : base(fixture) { }

    [Fact]
    public void Should_BuildSql_When_OrderByIsSingle()
    {
        var requestCtx = new RequestContext
        {
            Scope = "TagTest",
            SqlId = "OrderBy",
            Request = new
            {
                OrderBy = new KeyValuePair<String, String>("Id", "Desc")
            }
        };
        requestCtx.SetupParameters();

        var statement = SmartSqlConfig.GetStatement(requestCtx.FullSqlId);
        statement.BuildSql(requestCtx);

        requestCtx.SqlBuilder.ToString().Trim().Should().Be("Order By Id Desc");
    }

    [Fact]
    public void Should_BuildEmptySql_When_OrderByIsEmpty()
    {
        var requestCtx = new RequestContext
        {
            Scope = "TagTest",
            SqlId = "OrderBy"
        };
        requestCtx.SetupParameters();

        var statement = SmartSqlConfig.GetStatement(requestCtx.FullSqlId);
        statement.BuildSql(requestCtx);

        requestCtx.SqlBuilder.ToString().Trim().Should().BeEmpty();
    }

    [Fact]
    public void Should_BuildSql_When_OrderByIsMulti()
    {
        var requestCtx = new RequestContext
        {
            Scope = "TagTest",
            SqlId = "OrderBy",
            Request = new
            {
                OrderBy = new Dictionary<string, string>
                {
                    { "Id", "Desc" },
                    { "Name", "Asc" },
                }
            }
        };
        requestCtx.SetupParameters();

        var statement = SmartSqlConfig.GetStatement(requestCtx.FullSqlId);
        statement.BuildSql(requestCtx);

        requestCtx.SqlBuilder.ToString().Trim().Should().Be("Order By Id Desc,Name Asc");
    }
}
