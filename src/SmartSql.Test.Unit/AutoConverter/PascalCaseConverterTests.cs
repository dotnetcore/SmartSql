using System.Collections.Generic;
using FluentAssertions;
using SmartSql.AutoConverter;
using Xunit;

namespace SmartSql.Test.Unit.AutoConverter;

public class PascalCaseConverterTests
{
    private readonly PascalCaseConverter _converter = new PascalCaseConverter();

    [Fact]
    public void Should_ConvertToPascalCase_When_MultipleWords()
    {
        var words = new List<string> { "user", "name" };

        var result = _converter.Convert(words);

        result.Should().Be("UserName");
    }

    [Fact]
    public void Should_ConvertToCapitalized_When_SingleWord()
    {
        var words = new List<string> { "name" };

        var result = _converter.Convert(words);

        result.Should().Be("Name");
    }

    [Fact]
    public void Should_PreserveCasing_When_WordsAlreadyPascal()
    {
        var words = new List<string> { "User", "Name" };

        var result = _converter.Convert(words);

        result.Should().Be("UserName");
    }
}
