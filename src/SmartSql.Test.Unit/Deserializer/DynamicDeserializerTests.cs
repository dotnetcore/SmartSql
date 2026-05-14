using System;
using System.Collections.Generic;
using System.Data.Common;
using FluentAssertions;
using Moq;
using SmartSql.Configuration;
using SmartSql.Data;
using SmartSql.Deserializer;
using SmartSql.Reflection.TypeConstants;
using Xunit;

namespace SmartSql.Test.Unit.Deserializer;

public class DynamicDeserializerTests
{
    private readonly DynamicDeserializer _sut = new();
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
    public void CanDeserialize_Should_ReturnTrue_When_ResultTypeIsObject()
    {
        var context = CreateContext<object>(new Mock<DbDataReader>());

        _sut.CanDeserialize(context, CommonType.Object).Should().BeTrue();
    }

    [Fact]
    public void CanDeserialize_Should_ReturnTrue_When_ResultTypeIsDictionaryStringObject()
    {
        var context = CreateContext<Dictionary<string, object>>(new Mock<DbDataReader>());

        _sut.CanDeserialize(context, CommonType.DictionaryStringObject).Should().BeTrue();
    }

    [Fact]
    public void CanDeserialize_Should_ReturnTrue_When_ResultTypeIsDynamicRow()
    {
        var context = CreateContext<DynamicRow>(new Mock<DbDataReader>());

        _sut.CanDeserialize(context, DataType.DynamicRow).Should().BeTrue();
    }

    [Fact]
    public void CanDeserialize_Should_ReturnFalse_When_ResultTypeIsValueType()
    {
        var context = CreateContext<int>(new Mock<DbDataReader>());

        _sut.CanDeserialize(context, typeof(int)).Should().BeFalse();
    }

    [Fact]
    public void CanDeserialize_Should_ReturnFalse_When_ResultTypeIsString()
    {
        var context = CreateContext<string>(new Mock<DbDataReader>());

        _sut.CanDeserialize(context, typeof(string)).Should().BeFalse();
    }

    [Fact]
    public void ToSingle_Should_ReturnDynamicRow_When_DataReaderHasRows()
    {
        var mockReader = new Mock<DbDataReader>();
        mockReader.Setup(r => r.HasRows).Returns(true);
        mockReader.Setup(r => r.Read()).Returns(true);
        mockReader.Setup(r => r.FieldCount).Returns(2);
        mockReader.Setup(r => r.GetName(0)).Returns("Id");
        mockReader.Setup(r => r.GetName(1)).Returns("Name");
        mockReader.Setup(r => r.GetValues(It.IsAny<object[]>()))
            .Callback<object[]>(values =>
            {
                values[0] = 1L;
                values[1] = "Test";
            });

        var context = CreateContext<DynamicRow>(mockReader);

        var result = _sut.ToSingle<DynamicRow>(context);

        result.Should().NotBeNull();
        result["Id"].Should().Be(1L);
        result["Name"].Should().Be("Test");
    }

    [Fact]
    public void ToSingle_Should_ReturnDefault_When_NoRows()
    {
        var mockReader = new Mock<DbDataReader>();
        mockReader.Setup(r => r.HasRows).Returns(false);

        var context = CreateContext<DynamicRow>(mockReader);

        var result = _sut.ToSingle<DynamicRow>(context);

        result.Should().BeNull();
    }

    [Fact]
    public void ToSingle_Should_ReturnAsObject_When_ResultTypeIsObject()
    {
        var mockReader = new Mock<DbDataReader>();
        mockReader.Setup(r => r.HasRows).Returns(true);
        mockReader.Setup(r => r.Read()).Returns(true);
        mockReader.Setup(r => r.FieldCount).Returns(1);
        mockReader.Setup(r => r.GetName(0)).Returns("Value");
        mockReader.Setup(r => r.GetValues(It.IsAny<object[]>()))
            .Callback<object[]>(values => { values[0] = 42; });

        var context = CreateContext<object>(mockReader);

        var result = _sut.ToSingle<object>(context);

        result.Should().NotBeNull();
        result.Should().BeOfType<DynamicRow>();
    }

    [Fact]
    public void ToList_Should_ReturnAllRows_When_DataReaderHasMultipleRows()
    {
        var mockReader = new Mock<DbDataReader>();
        mockReader.Setup(r => r.HasRows).Returns(true);
        var readSequence = mockReader.SetupSequence(r => r.Read());
        readSequence.Returns(true);
        readSequence.Returns(true);
        readSequence.Returns(false);

        mockReader.Setup(r => r.FieldCount).Returns(2);
        mockReader.Setup(r => r.GetName(0)).Returns("Id");
        mockReader.Setup(r => r.GetName(1)).Returns("Name");

        var valueCallCount = 0;
        mockReader.Setup(r => r.GetValues(It.IsAny<object[]>()))
            .Callback<object[]>(values =>
            {
                valueCallCount++;
                values[0] = valueCallCount;
                values[1] = $"Name{valueCallCount}";
            });

        var context = CreateContext<DynamicRow>(mockReader);

        var result = _sut.ToList<DynamicRow>(context);

        result.Should().HaveCount(2);
        result[0]["Id"].Should().Be(1);
        result[1]["Id"].Should().Be(2);
    }

    [Fact]
    public void ToList_Should_ReturnEmptyList_When_NoRows()
    {
        var mockReader = new Mock<DbDataReader>();
        mockReader.Setup(r => r.HasRows).Returns(false);

        var context = CreateContext<DynamicRow>(mockReader);

        var result = _sut.ToList<DynamicRow>(context);

        result.Should().BeEmpty();
    }

    [Fact]
    public void ToSingle_Should_HandleNullColumnValues()
    {
        var mockReader = new Mock<DbDataReader>();
        mockReader.Setup(r => r.HasRows).Returns(true);
        mockReader.Setup(r => r.Read()).Returns(true);
        mockReader.Setup(r => r.FieldCount).Returns(2);
        mockReader.Setup(r => r.GetName(0)).Returns("Id");
        mockReader.Setup(r => r.GetName(1)).Returns("NullableName");
        mockReader.Setup(r => r.GetValues(It.IsAny<object[]>()))
            .Callback<object[]>(values =>
            {
                values[0] = 1L;
                values[1] = DBNull.Value;
            });

        var context = CreateContext<DynamicRow>(mockReader);

        var result = _sut.ToSingle<DynamicRow>(context);

        result.Should().NotBeNull();
        result["NullableName"].Should().Be(DBNull.Value);
    }
}
