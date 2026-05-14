using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using SmartSql.AutoConverter;
using Xunit;

namespace SmartSql.Test.Unit.AutoConverter;

public class DefaultTokenizerTests
{
    private DefaultTokenizer CreateTokenizer(
        string ignorePrefix = null,
        string delimiter = null,
        bool? uppercaseSplit = null)
    {
        var tokenizer = new DefaultTokenizer();
        var parameters = new Dictionary<string, object>();
        if (ignorePrefix != null) parameters["IgnorePrefix"] = ignorePrefix;
        if (delimiter != null) parameters["Delimiter"] = delimiter;
        if (uppercaseSplit != null) parameters["UppercaseSplit"] = uppercaseSplit.Value;
        tokenizer.Initialize(parameters);
        return tokenizer;
    }

    [Fact]
    public void Should_SplitOnUppercase_When_UppercaseSplitEnabled()
    {
        var tokenizer = CreateTokenizer();

        var result = tokenizer.Segment("UserName").ToList();

        result.Should().BeEquivalentTo("User", "Name");
    }

    [Fact]
    public void Should_NotSplitOnUppercase_When_UppercaseSplitDisabled()
    {
        var tokenizer = CreateTokenizer(uppercaseSplit: false);

        var result = tokenizer.Segment("UserName").ToList();

        result.Should().ContainSingle().Which.Should().Be("UserName");
    }

    [Fact]
    public void Should_RemovePrefix_When_IgnorePrefixSet()
    {
        var tokenizer = CreateTokenizer(ignorePrefix: "T_");

        var result = tokenizer.Segment("T_User").ToList();

        result.Should().ContainSingle().Which.Should().Be("User");
    }

    [Fact]
    public void Should_SplitByDelimiter_When_DelimiterSet()
    {
        var tokenizer = CreateTokenizer(delimiter: "_", uppercaseSplit: false);

        var result = tokenizer.Segment("user_name").ToList();

        result.Should().BeEquivalentTo("user", "name");
    }

    [Fact]
    public void Should_ReturnSingleWord_When_NoSplittingNeeded()
    {
        var tokenizer = CreateTokenizer();

        var result = tokenizer.Segment("name").ToList();

        result.Should().ContainSingle().Which.Should().Be("name");
    }

    [Fact]
    public void Should_InitializeFromParameters_When_Called()
    {
        var tokenizer = new DefaultTokenizer();
        var parameters = new Dictionary<string, object>
        {
            ["IgnorePrefix"] = "T_",
            ["Delimiter"] = "_",
            ["UppercaseSplit"] = true
        };

        tokenizer.Initialize(parameters);

        tokenizer.Initialized.Should().BeTrue();
        tokenizer.IgnorePrefix.Should().Be("T_");
        tokenizer.Delimiter.Should().Be("_");
        tokenizer.UppercaseSplit.Should().BeTrue();
    }
}
