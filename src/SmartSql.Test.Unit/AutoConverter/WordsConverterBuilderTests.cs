using System;
using System.Collections.Generic;
using FluentAssertions;
using SmartSql.AutoConverter;
using SmartSql.Exceptions;
using Xunit;

namespace SmartSql.Test.Unit.AutoConverter;

public class WordsConverterBuilderTests
{
    private readonly WordsConverterBuilder _builder = new();

    [Fact]
    public void Should_BuildCamelConverter_When_NameIsCamel()
    {
        var result = _builder.Build("Camel", null);

        result.Should().BeOfType<CamelCaseConverter>();
        result.Should().NotBeNull();
    }

    [Fact]
    public void Should_BuildCamelConverter_When_NameIsLowercaseCamel()
    {
        var result = _builder.Build("camel", null);

        result.Should().BeOfType<CamelCaseConverter>();
    }

    [Fact]
    public void Should_BuildDelimiterConverter_When_NameIsDelimiter()
    {
        var result = _builder.Build("Delimiter", new Dictionary<string, object>());

        result.Should().BeOfType<DelimiterConverter>();
    }

    [Fact]
    public void Should_BuildPascalConverter_When_NameIsPascal()
    {
        var result = _builder.Build("Pascal", null);

        result.Should().BeOfType<PascalCaseConverter>();
    }

    [Fact]
    public void Should_BuildPascalSingularConverter_When_NameIsPascalSingular()
    {
        var result = _builder.Build("PascalSingular", null);

        result.Should().BeOfType<PascalCaseSingularConverter>();
    }

    [Fact]
    public void Should_BuildStrikeThroughConverter_When_NameIsStrikeThrough()
    {
        var result = _builder.Build("StrikeThrough", null);

        result.Should().BeOfType<StrikeThroughConverter>();
    }

    [Fact]
    public void Should_BuildNoneConverter_When_NameIsNone()
    {
        var result = _builder.Build("None", null);

        result.Should().BeOfType<NoneConverter>();
    }

    [Fact]
    public void Should_Throw_When_NameIsUnknown()
    {
        var act = () => _builder.Build("UnknownConverter", null);

        act.Should().Throw<SmartSqlException>()
            .WithMessage("*UnknownConverter*can not found*");
    }

    [Fact]
    public void Should_InitializeConverter_When_BuildCalled()
    {
        var mockProps = new Dictionary<string, object> { { "Prefix", "tbl_" } };

        var result = _builder.Build("Delimiter", mockProps);

        result.Should().BeOfType<DelimiterConverter>();
        // Verify initialization occurred by checking conversion behavior
        var output = result.Convert(new[] { "User", "Name" });
        output.Should().StartWith("tbl_");
    }

    [Fact]
    public void Should_BuildConverterCaseInsensitively_When_NameIsMixedCase()
    {
        var result = _builder.Build("CAMEL", null);

        result.Should().BeOfType<CamelCaseConverter>();
    }

    [Fact]
    public void Should_BuildAllConverterTypes_When_ValidNamesProvided()
    {
        var names = new[] { "CAMEL", "DELIMITER", "PASCAL", "PASCALSINGULAR", "STRIKETHROUGH", "NONE" };

        foreach (var name in names)
        {
            var result = _builder.Build(name, null);
            result.Should().NotBeNull($"because {name} should build successfully");
        }
    }
}
