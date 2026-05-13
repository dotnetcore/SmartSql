using System;
using FluentAssertions;
using SmartSql.Reflection;
using SmartSql.Test.Unit.TestEntities;
using Xunit;

namespace SmartSql.Test.Unit.Reflection;

public class TypeExtensionsTests
{
    [Fact]
    public void Should_ReturnNull_When_ReferenceType()
    {
        var result = typeof(string).Default();

        result.Should().BeNull();
    }

    [Fact]
    public void Should_ReturnZero_When_IntType()
    {
        var result = typeof(int).Default();

        result.Should().Be(0);
    }

    [Fact]
    public void Should_ReturnNull_When_NullableInt()
    {
        var result = typeof(int?).Default();

        result.Should().BeNull();
    }

    [Fact]
    public void Should_ReturnDefaultDateTime_When_DateTimeType()
    {
        var result = typeof(DateTime).Default();

        result.Should().Be(default(DateTime));
    }

    [Fact]
    public void Should_ReturnDefaultGuid_When_GuidType()
    {
        var result = typeof(Guid).Default();

        result.Should().Be(default(Guid));
    }

    [Fact]
    public void Should_ReturnEnumDefault_When_EnumType()
    {
        var result = typeof(UserStatus).Default();

        result.Should().Be(default(UserStatus));
    }
}
