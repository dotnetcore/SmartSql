using System;
using System.Collections.Generic;
using System.Data.Common;
using FluentAssertions;
using Moq;
using SmartSql.Configuration;
using SmartSql.Data;
using SmartSql.Deserializer;
using SmartSql.Exceptions;
using SmartSql.TypeHandlers;
using Xunit;

namespace SmartSql.Test.Unit.Deserializer;

public class ValueTupleDeserializerTests
{
    private readonly SmartSqlConfig _config = new();

    private ValueTupleDeserializer CreateSut()
    {
        var deserializerFactory = new DeserializerFactory();
        deserializerFactory.Add(new ValueTypeDeserializer());

        return new ValueTupleDeserializer(deserializerFactory);
    }

    private ExecutionContext CreateContext<T>(Mock<DbDataReader> mockReader)
    {
        var wrapper = new DataReaderWrapper(mockReader.Object);
        return new ExecutionContext
        {
            SmartSqlConfig = _config,
            DataReaderWrapper = wrapper,
            Request = new RequestContext(),
            Result = new SingleResultContext<T>()
        };
    }

    [Fact]
    public void CanDeserialize_Should_ReturnTrue_When_ResultTypeIsValueTuple()
    {
        var sut = CreateSut();
        var context = CreateContext<ValueTuple<int, string>>(new Mock<DbDataReader>());

        sut.CanDeserialize(context, typeof(ValueTuple<int, string>)).Should().BeTrue();
    }

    [Fact]
    public void CanDeserialize_Should_ReturnTrue_When_ResultTypeIsSimpleValueTuple()
    {
        var sut = CreateSut();
        var context = CreateContext<ValueTuple<int>>(new Mock<DbDataReader>());

        sut.CanDeserialize(context, typeof(ValueTuple<int>)).Should().BeTrue();
    }

    [Fact]
    public void CanDeserialize_Should_ReturnFalse_When_ResultTypeIsNotValueTuple()
    {
        var sut = CreateSut();
        var context = CreateContext<int>(new Mock<DbDataReader>());

        sut.CanDeserialize(context, typeof(int)).Should().BeFalse();
    }

    [Fact]
    public void CanDeserialize_Should_ReturnFalse_When_ResultTypeIsClass()
    {
        var sut = CreateSut();
        var context = CreateContext<string>(new Mock<DbDataReader>());

        sut.CanDeserialize(context, typeof(string)).Should().BeFalse();
    }

    [Fact]
    public void ToSingle_Should_ReturnValueTuple_When_DataReaderHasMultipleResultSets()
    {
        var sut = CreateSut();
        var mockReader = new Mock<DbDataReader>();

        // First result set: int value
        mockReader.Setup(r => r.HasRows).Returns(true);
        var readSequence = mockReader.SetupSequence(r => r.Read());
        readSequence.Returns(true);
        readSequence.Returns(true);

        mockReader.SetupSequence(r => r.GetValue(0))
            .Returns(42)
            .Returns("hello");

        mockReader.Setup(r => r.NextResult()).Returns(true);

        var context = CreateContext<ValueTuple<int, string>>(mockReader);

        var result = sut.ToSingle<ValueTuple<int, string>>(context);

        result.Item1.Should().Be(42);
        result.Item2.Should().Be("hello");
    }

    [Fact]
    public void ToSingle_Should_HandleSingleElementTuple_When_DataReaderHasOneResultSet()
    {
        var sut = CreateSut();
        var mockReader = new Mock<DbDataReader>();
        mockReader.Setup(r => r.HasRows).Returns(true);
        mockReader.Setup(r => r.Read()).Returns(true);
        mockReader.Setup(r => r.GetValue(0)).Returns(99);

        var context = CreateContext<ValueTuple<int>>(mockReader);

        var result = sut.ToSingle<ValueTuple<int>>(context);

        result.Item1.Should().Be(99);
    }

    [Fact]
    public void ToList_Should_Throw_When_Called()
    {
        var sut = CreateSut();
        var context = CreateContext<ValueTuple<int, string>>(new Mock<DbDataReader>());

        var act = () => sut.ToList<ValueTuple<int, string>>(context);

        act.Should().Throw<SmartSqlException>();
    }
}
