using System;
using System.Data.Common;
using System.Linq;
using FluentAssertions;
using Moq;
using SmartSql.Configuration;
using SmartSql.Data;
using SmartSql.Deserializer;
using SmartSql.TypeHandlers;
using Xunit;

namespace SmartSql.Test.Unit.Deserializer;

public class ValueTypeDeserializerTests
{
    private readonly ValueTypeDeserializer _sut = new();
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
    public void CanDeserialize_Should_ReturnTrue_When_ResultTypeIsValueType()
    {
        var context = CreateContext<int>(new Mock<DbDataReader>());

        _sut.CanDeserialize(context, typeof(int)).Should().BeTrue();
    }

    [Fact]
    public void CanDeserialize_Should_ReturnTrue_When_ResultTypeIsString()
    {
        var context = CreateContext<string>(new Mock<DbDataReader>());

        _sut.CanDeserialize(context, typeof(string)).Should().BeTrue();
    }

    [Fact]
    public void CanDeserialize_Should_ReturnTrue_When_ResultTypeIsLong()
    {
        var context = CreateContext<long>(new Mock<DbDataReader>());

        _sut.CanDeserialize(context, typeof(long)).Should().BeTrue();
    }

    [Fact]
    public void CanDeserialize_Should_ReturnTrue_When_ResultTypeIsBool()
    {
        var context = CreateContext<bool>(new Mock<DbDataReader>());

        _sut.CanDeserialize(context, typeof(bool)).Should().BeTrue();
    }

    [Fact]
    public void CanDeserialize_Should_ReturnTrue_When_ResultTypeIsDateTime()
    {
        var context = CreateContext<DateTime>(new Mock<DbDataReader>());

        _sut.CanDeserialize(context, typeof(DateTime)).Should().BeTrue();
    }

    [Fact]
    public void CanDeserialize_Should_ReturnFalse_When_ResultTypeIsReferenceType()
    {
        var context = CreateContext<object>(new Mock<DbDataReader>());

        _sut.CanDeserialize(context, typeof(object)).Should().BeFalse();
    }

    [Fact]
    public void ToSingle_Should_ReturnValue_When_DataReaderHasInt32()
    {
        var mockReader = new Mock<DbDataReader>();
        mockReader.Setup(r => r.HasRows).Returns(true);
        mockReader.Setup(r => r.Read()).Returns(true);
        mockReader.Setup(r => r.GetValue(0)).Returns(42);

        var context = CreateContext<int>(mockReader);

        var result = _sut.ToSingle<int>(context);

        result.Should().Be(42);
    }

    [Fact]
    public void ToSingle_Should_ReturnValue_When_DataReaderHasInt64()
    {
        var mockReader = new Mock<DbDataReader>();
        mockReader.Setup(r => r.HasRows).Returns(true);
        mockReader.Setup(r => r.Read()).Returns(true);
        mockReader.Setup(r => r.GetValue(0)).Returns(99L);

        var context = CreateContext<long>(mockReader);

        var result = _sut.ToSingle<long>(context);

        result.Should().Be(99L);
    }

    [Fact]
    public void ToSingle_Should_ReturnValue_When_DataReaderHasString()
    {
        var mockReader = new Mock<DbDataReader>();
        mockReader.Setup(r => r.HasRows).Returns(true);
        mockReader.Setup(r => r.Read()).Returns(true);
        mockReader.Setup(r => r.GetValue(0)).Returns("hello");

        var context = CreateContext<string>(mockReader);

        var result = _sut.ToSingle<string>(context);

        result.Should().Be("hello");
    }

    [Fact]
    public void ToSingle_Should_ReturnDefault_When_NoRows()
    {
        var mockReader = new Mock<DbDataReader>();
        mockReader.Setup(r => r.HasRows).Returns(false);

        var context = CreateContext<int>(mockReader);

        var result = _sut.ToSingle<int>(context);

        result.Should().Be(default(int));
    }

    [Fact]
    public void ToSingle_Should_ReturnDefault_When_NoRowsForString()
    {
        var mockReader = new Mock<DbDataReader>();
        mockReader.Setup(r => r.HasRows).Returns(false);

        var context = CreateContext<string>(mockReader);

        var result = _sut.ToSingle<string>(context);

        result.Should().BeNull();
    }

    [Fact]
    public void ToList_Should_ReturnAllValues_When_DataReaderHasMultipleRows()
    {
        var mockReader = new Mock<DbDataReader>();
        mockReader.Setup(r => r.HasRows).Returns(true);
        var readSequence = mockReader.SetupSequence(r => r.Read());
        readSequence.Returns(true);
        readSequence.Returns(true);
        readSequence.Returns(false);

        var valueSequence = mockReader.SetupSequence(r => r.GetValue(0));
        valueSequence.Returns(10);
        valueSequence.Returns(20);

        var context = CreateContext<int>(mockReader);

        var result = _sut.ToList<int>(context);

        result.Should().HaveCount(2);
        result[0].Should().Be(10);
        result[1].Should().Be(20);
    }

    [Fact]
    public void ToList_Should_ReturnEmptyList_When_NoRows()
    {
        var mockReader = new Mock<DbDataReader>();
        mockReader.Setup(r => r.HasRows).Returns(false);

        var context = CreateContext<int>(mockReader);

        var result = _sut.ToList<int>(context);

        result.Should().BeEmpty();
    }

    [Fact]
    public void ToList_Should_ReturnStrings_When_DataReaderHasStrings()
    {
        var mockReader = new Mock<DbDataReader>();
        mockReader.Setup(r => r.HasRows).Returns(true);
        var readSequence = mockReader.SetupSequence(r => r.Read());
        readSequence.Returns(true);
        readSequence.Returns(false);

        mockReader.Setup(r => r.GetValue(0)).Returns("test-string");

        var context = CreateContext<string>(mockReader);

        var result = _sut.ToList<string>(context);

        result.Should().HaveCount(1);
        result[0].Should().Be("test-string");
    }
}
