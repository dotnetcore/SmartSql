using System;
using FluentAssertions;
using SmartSql.Exceptions;
using Xunit;

namespace SmartSql.Test.Unit.Exceptions;

public class SmartSqlExceptionTests
{
    [Fact]
    public void Should_HaveDefaultMessage_When_ParameterlessConstructorUsed()
    {
        var ex = new SmartSqlException();

        ex.Message.Should().Be("SmartSql throw an exception.");
    }

    [Fact]
    public void Should_WrapInnerException_When_ExceptionConstructorUsed()
    {
        var inner = new InvalidOperationException("inner error");

        var ex = new SmartSqlException(inner);

        ex.Message.Should().Be("SmartSql throw an exception.");
        ex.InnerException.Should().Be(inner);
    }

    [Fact]
    public void Should_SetMessage_When_MessageConstructorUsed()
    {
        var ex = new SmartSqlException("custom message");

        ex.Message.Should().Be("custom message");
        ex.InnerException.Should().BeNull();
    }

    [Fact]
    public void Should_SetMessageAndInner_When_MessageAndExceptionConstructorUsed()
    {
        var inner = new InvalidOperationException("inner");

        var ex = new SmartSqlException("custom", inner);

        ex.Message.Should().Be("custom");
        ex.InnerException.Should().Be(inner);
    }

    [Fact]
    public void Should_BeException_When_Created()
    {
        var ex = new SmartSqlException();

        ex.Should().BeAssignableTo<Exception>();
    }
}
