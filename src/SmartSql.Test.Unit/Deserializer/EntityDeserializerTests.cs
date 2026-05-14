using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Reflection;
using FluentAssertions;
using Moq;
using SmartSql.AutoConverter;
using SmartSql.Configuration;
using SmartSql.Data;
using SmartSql.Deserializer;
using SmartSql.Test.Unit.TestEntities;
using SmartSql.TypeHandlers;
using Xunit;

namespace SmartSql.Test.Unit.Deserializer;

public class EntityDeserializerTests
{
    private readonly EntityDeserializer _sut = new();
    private readonly SmartSqlConfig _config = new();

    private static void SetAutoConverter(AbstractRequestContext request, IAutoConverter converter)
    {
        var prop = typeof(AbstractRequestContext).GetProperty("AutoConverter",
            BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        prop.SetValue(request, converter);
    }

    private ExecutionContext CreateContext<T>(Mock<DbDataReader> mockReader)
    {
        var wrapper = new DataReaderWrapper(mockReader.Object);
        var request = new RequestContext();
        SetAutoConverter(request, NoneAutoConverter.INSTANCE);
        return new ExecutionContext
        {
            SmartSqlConfig = _config,
            DataReaderWrapper = wrapper,
            Request = request,
            Result = new SingleResultContext<T>()
        };
    }

    [Fact]
    public void CanDeserialize_Should_ReturnTrue_When_AnyType()
    {
        var context = CreateContext<User>(new Mock<DbDataReader>());

        _sut.CanDeserialize(context, typeof(User)).Should().BeTrue();
    }

    [Fact]
    public void CanDeserialize_Should_ReturnTrue_When_ResultTypeIsValueType()
    {
        var context = CreateContext<int>(new Mock<DbDataReader>());

        // EntityDeserializer returns true for everything (it is the fallback)
        _sut.CanDeserialize(context, typeof(int)).Should().BeTrue();
    }

    [Fact]
    public void ToSingle_Should_ReturnDefault_When_NoRows()
    {
        var mockReader = new Mock<DbDataReader>();
        mockReader.Setup(r => r.HasRows).Returns(false);

        var context = CreateContext<User>(mockReader);

        var result = _sut.ToSingle<User>(context);

        result.Should().BeNull();
    }

    [Fact]
    public void ToList_Should_ReturnEmptyList_When_NoRows()
    {
        var mockReader = new Mock<DbDataReader>();
        mockReader.Setup(r => r.HasRows).Returns(false);

        var context = CreateContext<User>(mockReader);

        var result = _sut.ToList<User>(context);

        result.Should().BeEmpty();
    }

    [Fact]
    public void ToSingle_Should_ReturnEntity_When_DataReaderHasMappedColumns()
    {
        var mockReader = new Mock<DbDataReader>();
        mockReader.Setup(r => r.HasRows).Returns(true);
        mockReader.Setup(r => r.Read()).Returns(true);
        mockReader.Setup(r => r.FieldCount).Returns(2);
        mockReader.Setup(r => r.GetName(0)).Returns("id");
        mockReader.Setup(r => r.GetName(1)).Returns("user_name");
        mockReader.Setup(r => r.GetFieldType(0)).Returns(typeof(long));
        mockReader.Setup(r => r.GetFieldType(1)).Returns(typeof(string));
        mockReader.Setup(r => r.IsDBNull(It.IsAny<int>())).Returns(false);
        mockReader.Setup(r => r.GetInt64(0)).Returns(1L);
        mockReader.Setup(r => r.GetValue(0)).Returns(1L);
        mockReader.Setup(r => r.GetString(1)).Returns("TestUser");
        mockReader.Setup(r => r.GetValue(1)).Returns("TestUser");

        var context = CreateContext<User>(mockReader);

        var result = _sut.ToSingle<User>(context);

        result.Should().NotBeNull();
        result.Id.Should().Be(1);
        result.UserName.Should().Be("TestUser");
    }

    [Fact]
    public void ToList_Should_ReturnEntities_When_DataReaderHasMultipleRows()
    {
        var mockReader = new Mock<DbDataReader>();
        mockReader.Setup(r => r.HasRows).Returns(true);
        var readSequence = mockReader.SetupSequence(r => r.Read());
        readSequence.Returns(true);
        readSequence.Returns(true);
        readSequence.Returns(false);

        mockReader.Setup(r => r.FieldCount).Returns(2);
        mockReader.Setup(r => r.GetName(0)).Returns("id");
        mockReader.Setup(r => r.GetName(1)).Returns("user_name");
        mockReader.Setup(r => r.GetFieldType(0)).Returns(typeof(long));
        mockReader.Setup(r => r.GetFieldType(1)).Returns(typeof(string));
        mockReader.Setup(r => r.IsDBNull(It.IsAny<int>())).Returns(false);

        var callCount = 0;
        mockReader.Setup(r => r.GetInt64(0)).Returns(() =>
        {
            callCount++;
            return (long)callCount;
        });
        mockReader.Setup(r => r.GetValue(0)).Returns(() => (long)callCount);
        mockReader.Setup(r => r.GetString(1)).Returns(() => $"User{callCount}");
        mockReader.Setup(r => r.GetValue(1)).Returns(() => $"User{callCount}");

        var context = CreateContext<User>(mockReader);

        var result = _sut.ToList<User>(context);

        result.Should().HaveCount(2);
        result[0].Id.Should().Be(1);
        result[0].UserName.Should().Be("User1");
        result[1].Id.Should().Be(2);
        result[1].UserName.Should().Be("User2");
    }
}
