using System.Collections.Generic;
using FluentAssertions;
using SmartSql.Configuration;
using SmartSql.Exceptions;
using Xunit;

namespace SmartSql.Test.Unit.ConfigBuilder;

public class PropertiesTests
{
    [Fact]
    public void Should_ResolveValue_When_KeyExists()
    {
        var properties = new Properties();
        properties.Import(new Dictionary<string, string> {{"SmartSql", "Great"}});

        var result = properties.GetPropertyValue("${SmartSql}");

        result.Should().Be("Great");
    }

    [Fact]
    public void Should_ResolveValue_When_AppendedToText()
    {
        var properties = new Properties();
        properties.Import(new Dictionary<string, string> {{"SmartSql", "Great"}});

        var result = properties.GetPropertyValue("${SmartSql}-Great");

        result.Should().Be("Great-Great");
    }

    [Fact]
    public void Should_ResolveValue_When_KeyContainsColon()
    {
        var properties = new Properties();
        properties.Import(new Dictionary<string, string> {{"SmartSql:Great", "Yes"}});

        var result = properties.GetPropertyValue("${SmartSql:Great}");

        result.Should().Be("Yes");
    }

    [Fact]
    public void Should_ResolveValue_When_KeyContainsBackQuote()
    {
        var properties = new Properties();
        properties.Import(new Dictionary<string, string> {{"SmartSql`Great", "Yes"}});

        var result = properties.GetPropertyValue("${SmartSql`Great}");

        result.Should().Be("Yes");
    }

    [Fact]
    public void Should_ResolveValue_When_KeyContainsNumber()
    {
        var properties = new Properties();
        properties.Import(new Dictionary<string, string> {{"SmartSql888", "Yes"}});

        var result = properties.GetPropertyValue("${SmartSql888}");

        result.Should().Be("Yes");
    }

    [Fact]
    public void Should_ResolveValue_When_KeyContainsDot()
    {
        var properties = new Properties();
        properties.Import(new Dictionary<string, string> {{"SmartSql.888", "Yes"}});

        var result = properties.GetPropertyValue("${SmartSql.888}");

        result.Should().Be("Yes");
    }

    [Fact]
    public void Should_ResolveValue_When_KeyContainsSpace()
    {
        var properties = new Properties();
        properties.Import(new Dictionary<string, string> {{"SmartSql 888", "Yes"}});

        var result = properties.GetPropertyValue("${SmartSql 888}");

        result.Should().Be("Yes");
    }

    [Fact]
    public void Should_ResolveValue_When_ConcatWithPrefix()
    {
        var properties = new Properties();
        properties.Import(new Dictionary<string, string> {{"SmartSql", "Great"}});

        var result = properties.GetPropertyValue("SmartSql.${SmartSql}");

        result.Should().Be("SmartSql.Great");
    }

    [Fact]
    public void Should_ReturnRawString_When_KeyNotFound()
    {
        var properties = new Properties();

        var result = properties.GetPropertyValue("${Unknown}");

        result.Should().Be("${Unknown}");
    }
}
