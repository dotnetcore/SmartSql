using FluentAssertions;
using System;
using SmartSql.Configuration.Tags;
using Xunit;

namespace SmartSql.Test.Integration.Tags;

public class SetTests : IntegrationTestBase
{
    public SetTests(SmartSqlFixture fixture) : base(fixture) { }

    [Fact]
    public void Should_BuildSql_When_RequestHasProperties()
    {
        var requestCtx = new RequestContext
        {
            Scope = "TagTest",
            SqlId = "Set",
            Request = new { Property1 = "SmartSql", Property2 = 1, Id = 1 }
        };
        requestCtx.SetupParameters();

        var statement = SmartSqlConfig.GetStatement(requestCtx.FullSqlId);
        statement.BuildSql(requestCtx);

        requestCtx.SqlBuilder.ToString().Trim().Should().Be(@"Set
                    Property1=?Property1
                 ,
                    Property2=?Property2");
    }

    [Fact]
    public void Should_Throw_When_RequestIsEmpty()
    {
        var requestCtx = new RequestContext
        {
            Scope = "TagTest",
            SqlId = "Set"
        };
        requestCtx.SetupParameters();

        var statement = SmartSqlConfig.GetStatement(requestCtx.FullSqlId);
        Action act = () => statement.BuildSql(requestCtx);

        act.Should().Throw<TagMinMatchedFailException>();
    }
}
