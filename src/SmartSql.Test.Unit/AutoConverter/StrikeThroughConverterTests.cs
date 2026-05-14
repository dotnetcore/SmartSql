using System.Collections.Generic;
using FluentAssertions;
using SmartSql.AutoConverter;
using Xunit;

namespace SmartSql.Test.Unit.AutoConverter;

public class StrikeThroughConverterTests
{
    private readonly StrikeThroughConverter _converter = new StrikeThroughConverter();

    [Fact]
    public void Should_ConvertToStrikeThrough_When_MultipleWords()
    {
        var words = new List<string> { "user", "name" };

        var result = _converter.Convert(words);

        result.Should().Be("user-name");
    }

    [Fact]
    public void Should_ConvertToLowercase_When_WordsAreCamelCase()
    {
        var words = new List<string> { "User", "Name" };

        var result = _converter.Convert(words);

        result.Should().Be("user-name");
    }

    [Fact]
    public void Should_ReturnSingleWord_When_SingleWord()
    {
        var words = new List<string> { "name" };

        var result = _converter.Convert(words);

        result.Should().Be("name");
    }

    [Fact]
    public void Should_HandleMultipleHyphens_When_ManyWords()
    {
        var words = new List<string> { "user", "first", "name" };

        var result = _converter.Convert(words);

        result.Should().Be("user-first-name");
    }

    [Fact]
    public void Should_ReturnName_When_Accessed()
    {
        _converter.Name.Should().Be("StrikeThrough");
    }

    [Fact]
    public void Should_NotBeInitialized_When_NotInitialized()
    {
        _converter.Initialized.Should().BeFalse();
    }

    [Fact]
    public void Should_BeInitialized_When_InitializeCalled()
    {
        _converter.Initialize(new Dictionary<string, object>());

        _converter.Initialized.Should().BeTrue();
    }
}
