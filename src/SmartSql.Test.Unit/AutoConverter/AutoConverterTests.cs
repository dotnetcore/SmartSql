using System.Collections.Generic;
using FluentAssertions;
using SmartSql.AutoConverter;
using Xunit;

namespace SmartSql.Test.Unit.AutoConverter;

public class AutoConverterTests
{
    [Fact]
    public void Should_ReturnName_When_Accessed()
    {
        var converter = new SmartSql.AutoConverter.AutoConverter(
            "TestConverter",
            new NoneTokenizer(),
            new PascalCaseConverter());

        converter.Name.Should().Be("TestConverter");
    }

    [Fact]
    public void Should_ConvertUsingTokenizerAndConverter_When_PascalCaseUsed()
    {
        var tokenizer = new DefaultTokenizer();
        tokenizer.Initialize(new Dictionary<string, object>
        {
            ["Delimiter"] = "_",
            ["UppercaseSplit"] = false
        });
        var converter = new SmartSql.AutoConverter.AutoConverter(
            "Pascal",
            tokenizer,
            new PascalCaseConverter());

        var result = converter.Convert("user_name");

        result.Should().Be("UserName");
    }

    [Fact]
    public void Should_ConvertUsingTokenizerAndConverter_When_StrikeThroughUsed()
    {
        var converter = new SmartSql.AutoConverter.AutoConverter(
            "Strike",
            new DefaultTokenizer(),
            new StrikeThroughConverter());

        var result = converter.Convert("UserName");

        result.Should().Be("user-name");
    }

    [Fact]
    public void Should_PassThrough_When_NoneTokenizerUsed()
    {
        var converter = new SmartSql.AutoConverter.AutoConverter(
            "Test",
            new NoneTokenizer(),
            new PascalCaseConverter());

        var result = converter.Convert("hello");

        result.Should().Be("Hello");
    }
}
