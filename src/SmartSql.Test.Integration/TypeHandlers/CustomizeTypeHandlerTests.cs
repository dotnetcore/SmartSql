using System;
using FluentAssertions;
using Xunit;

namespace SmartSql.Test.Integration.TypeHandlers;

public class CustomizeTypeHandlerTests : IntegrationTestBase
{
    public CustomizeTypeHandlerTests(SmartSqlFixture fixture) : base(fixture) { }

    [Fact]
    public void Should_ReturnMatchingString_When_QueryingByAnsiString()
    {
        var reqParams = new
        {
            AnsiString = "AnsiString"
        };
        var actual = SqlMapper.QuerySingle<String>(new RequestContext
        {
            Scope = "CustomizeTypeHandlerTest",
            SqlId = "QueryByAnsiString",
            Request = reqParams
        });
        actual.Should().Be(reqParams.AnsiString);
    }

    [Fact]
    public void Should_ReturnMatchingString_When_QueryingByAnsiStringFixedLength()
    {
        var reqParams = new
        {
            AnsiStringFixedLength = "AnsiStringFixedLength"
        };
        var actual = SqlMapper.QuerySingle<String>(new RequestContext
        {
            Scope = "CustomizeTypeHandlerTest",
            SqlId = "QueryByAnsiStringFixedLength",
            Request = reqParams
        });
        actual.Should().Be(reqParams.AnsiStringFixedLength);
    }
}
