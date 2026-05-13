using System.Collections.Generic;
using FluentAssertions;
using SmartSql.AutoConverter;
using Xunit;

namespace SmartSql.Test.Unit.AutoConverter;

public class DelimiterConverterTests
{
    private DelimiterConverter CreateConverter(
        string delimiter = null,
        string prefix = null,
        DelimiterConverter.ConvertMode? mode = null)
    {
        var converter = new DelimiterConverter();
        var parameters = new Dictionary<string, object>();
        if (delimiter != null) parameters["Delimiter"] = delimiter;
        if (prefix != null) parameters["Prefix"] = prefix;
        if (mode != null) parameters["Mode"] = mode.Value;
        converter.Initialize(parameters);
        return converter;
    }

    [Fact]
    public void Should_JoinWithUnderscore_When_ModeNone()
    {
        var converter = CreateConverter();

        var result = converter.Convert(new List<string> { "user", "name" });

        result.Should().Be("user_name");
    }

    [Fact]
    public void Should_JoinWithUnderscoreLower_When_ModeAllLower()
    {
        var converter = CreateConverter(mode: DelimiterConverter.ConvertMode.AllLower);

        var result = converter.Convert(new List<string> { "User", "Name" });

        result.Should().Be("user_name");
    }

    [Fact]
    public void Should_JoinWithUnderscoreUpper_When_ModeAllUpper()
    {
        var converter = CreateConverter(mode: DelimiterConverter.ConvertMode.AllUpper);

        var result = converter.Convert(new List<string> { "User", "Name" });

        result.Should().Be("USER_NAME");
    }

    [Fact]
    public void Should_JoinWithCapitalized_When_ModeFirstUpper()
    {
        var converter = CreateConverter(mode: DelimiterConverter.ConvertMode.FirstUpper);

        var result = converter.Convert(new List<string> { "user", "name" });

        result.Should().Be("User_Name");
    }

    [Fact]
    public void Should_AddPrefix_When_PrefixSet()
    {
        var converter = CreateConverter(prefix: "t_");

        var result = converter.Convert(new List<string> { "user", "name" });

        result.Should().Be("t_user_name");
    }

    [Fact]
    public void Should_InitializeFromParameters_When_Called()
    {
        var converter = new DelimiterConverter();
        var parameters = new Dictionary<string, object>
        {
            ["Delimiter"] = "-",
            ["Prefix"] = "app-",
            ["Mode"] = DelimiterConverter.ConvertMode.AllLower
        };

        converter.Initialize(parameters);

        converter.Initialized.Should().BeTrue();
        var result = converter.Convert(new List<string> { "User", "Name" });
        result.Should().Be("app-user-name");
    }
}
