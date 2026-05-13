using System.Collections.Generic;
using FluentAssertions;
using SmartSql.AutoConverter;
using Xunit;

namespace SmartSql.Test.Unit.AutoConverter;

public class CamelCaseConverterTests
{
    private readonly CamelCaseConverter _converter = new CamelCaseConverter();

    [Fact]
    public void Should_ConvertToCamelCase_When_MultipleWords()
    {
        var words = new List<string> { "User", "Name" };

        var result = _converter.Convert(words);

        result.Should().Be("userName");
    }

    [Fact]
    public void Should_ConvertToLower_When_SingleWord()
    {
        var words = new List<string> { "Name" };

        var result = _converter.Convert(words);

        result.Should().Be("name");
    }

    [Fact]
    public void Should_CapitalizeSecondWord_When_AllWordsLowerCase()
    {
        var words = new List<string> { "user", "name" };

        var result = _converter.Convert(words);

        result.Should().Be("userName");
    }
}
