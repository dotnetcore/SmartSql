using System;
using System.Collections.Generic;
using FluentAssertions;
using SmartSql.AutoConverter;
using SmartSql.Exceptions;
using Xunit;

namespace SmartSql.Test.Unit.AutoConverter;

public class TokenizerBuilderTests
{
    private readonly TokenizerBuilder _builder = new TokenizerBuilder();

    [Fact]
    public void Should_ReturnDefaultTokenizer_When_NameIsDefault()
    {
        var tokenizer = _builder.Build("DEFAULT", null);

        tokenizer.Should().BeOfType<DefaultTokenizer>();
    }

    [Fact]
    public void Should_ReturnDefaultTokenizer_When_NameIsDefaultCaseInsensitive()
    {
        var tokenizer = _builder.Build("default", null);

        tokenizer.Should().BeOfType<DefaultTokenizer>();
    }

    [Fact]
    public void Should_InitializeDefaultTokenizer_When_PropertiesProvided()
    {
        var properties = new Dictionary<string, object>
        {
            ["Delimiter"] = "_"
        };

        var tokenizer = _builder.Build("DEFAULT", properties);

        tokenizer.Should().BeOfType<DefaultTokenizer>();
        ((DefaultTokenizer)tokenizer).Delimiter.Should().Be("_");
    }

    [Fact]
    public void Should_ReturnNoneTokenizer_When_NameIsNone()
    {
        var tokenizer = _builder.Build("NONE", null);

        tokenizer.Should().BeOfType<NoneTokenizer>();
    }

    [Fact]
    public void Should_ReturnNoneTokenizer_When_NameIsNoneCaseInsensitive()
    {
        var tokenizer = _builder.Build("none", null);

        tokenizer.Should().BeOfType<NoneTokenizer>();
    }

    [Fact]
    public void Should_ThrowSmartSqlException_When_NameIsUnknown()
    {
        Action act = () => _builder.Build("Unknown", null);

        act.Should().Throw<SmartSqlException>()
            .WithMessage("*Unknown*can not found*");
    }
}
