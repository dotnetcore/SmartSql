using FluentAssertions;
using SmartSql.Utils;
using Xunit;

namespace SmartSql.Test.Unit.Utils;

public class NamingUtilTests
{
    [Fact]
    public void Should_LowerFirstChar_When_CamelCase()
    {
        var result = NamingUtil.CamelCase("HelloWorld");

        result.Should().Be("helloWorld");
    }

    [Fact]
    public void Should_UpperFirstChar_When_PascalCase()
    {
        var result = NamingUtil.PascalCase("helloWorld");

        result.Should().Be("HelloWorld");
    }

    [Fact]
    public void Should_ConvertToSingular_When_EndsWithIes()
    {
        var result = NamingUtil.ToSingular("stories");

        result.Should().Be("story");
    }

    [Fact]
    public void Should_ConvertToSingular_When_EndsWithVowelYS()
    {
        var result = NamingUtil.ToSingular("days");

        result.Should().Be("day");
    }

    [Fact]
    public void Should_ConvertToSingular_When_EndsWithEs()
    {
        var result = NamingUtil.ToSingular("boxes");

        result.Should().Be("box");
    }

    [Fact]
    public void Should_ConvertToSingular_When_EndsWithS()
    {
        var result = NamingUtil.ToSingular("users");

        result.Should().Be("user");
    }

    [Fact]
    public void Should_ConvertToPlural_When_EndsWithConsonantY()
    {
        var result = NamingUtil.ToPlural("story");

        result.Should().Be("stories");
    }

    [Fact]
    public void Should_ConvertToPlural_When_EndsWithSxZh()
    {
        var result = NamingUtil.ToPlural("box");

        result.Should().Be("boxes");
    }

    [Fact]
    public void Should_ConvertToPlural_When_Regular()
    {
        var result = NamingUtil.ToPlural("user");

        result.Should().Be("users");
    }

    [Fact]
    public void Should_NotChange_When_SingularFormDoesNotMatch()
    {
        var result = NamingUtil.ToSingular("fish");

        result.Should().Be("fish");
    }
}
