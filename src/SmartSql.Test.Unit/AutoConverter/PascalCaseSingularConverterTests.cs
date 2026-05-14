using System.Collections.Generic;
using FluentAssertions;
using SmartSql.AutoConverter;
using Xunit;

namespace SmartSql.Test.Unit.AutoConverter;

public class PascalCaseSingularConverterTests
{
    private readonly PascalCaseSingularConverter _converter = new PascalCaseSingularConverter();

    [Fact]
    public void Should_ConvertToSingularPascalCase_When_PluralWords()
    {
        var words = new List<string> { "user", "names" };

        var result = _converter.Convert(words);

        result.Should().Be("UserName");
    }

    [Fact]
    public void Should_ConvertToSingularPascalCase_When_SinglePluralWord()
    {
        var words = new List<string> { "users" };

        var result = _converter.Convert(words);

        result.Should().Be("User");
    }

    [Fact]
    public void Should_ReturnPascalCase_When_WordAlreadySingular()
    {
        var words = new List<string> { "user", "name" };

        var result = _converter.Convert(words);

        result.Should().Be("UserName");
    }

    [Fact]
    public void Should_ReturnName_When_Accessed()
    {
        _converter.Name.Should().Be("PascalSingular");
    }

    [Fact]
    public void Should_BeInitialized_When_UnderlyingConverterAlwaysInitialized()
    {
        _converter.Initialized.Should().BeTrue();

        _converter.Initialize(new Dictionary<string, object>());

        _converter.Initialized.Should().BeTrue();
    }

    [Fact]
    public void Should_Initialize_When_NullParametersPassed()
    {
        _converter.Initialize(null);

        _converter.Initialized.Should().BeTrue();
    }
}
