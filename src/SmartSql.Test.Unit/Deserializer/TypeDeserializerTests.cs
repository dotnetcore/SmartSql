using System;
using System.Collections.Generic;
using System.Data.Common;
using FluentAssertions;
using Moq;
using SmartSql.Configuration;
using SmartSql.Data;
using SmartSql.Deserializer;
using SmartSql.TypeHandlers;
using Xunit;

namespace SmartSql.Test.Unit.Deserializer;

public class TypeDeserializerTests
{
    private readonly SmartSqlConfig _config = new();

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
    public void Deserialize_Should_CallToSingle_When_ResultTypeIsSingle()
    {
        var mockReader = new Mock<DbDataReader>();
        mockReader.Setup(r => r.HasRows).Returns(true);
        mockReader.Setup(r => r.Read()).Returns(true);
        mockReader.Setup(r => r.GetValue(0)).Returns(42);

        var context = CreateContext<int>(mockReader);
        var mockDeserializer = new Mock<IDataReaderDeserializer>();
        mockDeserializer.Setup(d => d.ToSingle<int>(context)).Returns(42);

        var result = TypeDeserializer.Deserialize(typeof(int), mockDeserializer.Object, context);

        result.Should().Be(42);
        mockDeserializer.Verify(d => d.ToSingle<int>(context), Times.Once);
    }

    [Fact]
    public void Deserialize_Should_CallToList_When_ResultTypeIsIEnumerable()
    {
        var mockReader = new Mock<DbDataReader>();
        mockReader.Setup(r => r.HasRows).Returns(true);

        var context = CreateContext<List<int>>(mockReader);
        var expectedList = new List<int> { 1, 2, 3 };

        var mockDeserializer = new Mock<IDataReaderDeserializer>();
        mockDeserializer.Setup(d => d.ToList<int>(context)).Returns(expectedList);

        var result = TypeDeserializer.Deserialize(typeof(List<int>), mockDeserializer.Object, context);

        result.Should().BeEquivalentTo(expectedList);
        mockDeserializer.Verify(d => d.ToList<int>(context), Times.Once);
    }

    [Fact]
    public void Deserialize_Should_CallToSingle_When_ResultTypeIsString()
    {
        var mockReader = new Mock<DbDataReader>();
        mockReader.Setup(r => r.HasRows).Returns(true);
        mockReader.Setup(r => r.Read()).Returns(true);
        mockReader.Setup(r => r.GetValue(0)).Returns("hello");

        var context = CreateContext<string>(mockReader);
        var mockDeserializer = new Mock<IDataReaderDeserializer>();
        mockDeserializer.Setup(d => d.ToSingle<string>(context)).Returns("hello");

        var result = TypeDeserializer.Deserialize(typeof(string), mockDeserializer.Object, context);

        result.Should().Be("hello");
    }

    [Fact]
    public void Deserialize_Should_BoxResult_When_ResultTypeIsValueType()
    {
        var mockReader = new Mock<DbDataReader>();
        var context = CreateContext<int>(mockReader);
        var mockDeserializer = new Mock<IDataReaderDeserializer>();
        mockDeserializer.Setup(d => d.ToSingle<int>(context)).Returns(42);

        var result = TypeDeserializer.Deserialize(typeof(int), mockDeserializer.Object, context);

        result.Should().BeOfType<int>();
        result.Should().Be(42);
    }
}
