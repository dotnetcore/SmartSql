using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Reflection;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using SmartSql.AutoConverter;
using SmartSql.Configuration;
using SmartSql.Data;
using SmartSql.Deserializer;
using SmartSql.Exceptions;
using SmartSql.Test.Unit.TestEntities;
using Xunit;

namespace SmartSql.Test.Unit.Deserializer;

public class EntityDeserializerDeepTests
{
    private readonly EntityDeserializer _sut = new();
    private readonly SmartSqlConfig _config;

    public EntityDeserializerDeepTests()
    {
        _config = new SmartSqlConfig();
    }

    private static void SetAutoConverter(AbstractRequestContext request, IAutoConverter converter)
    {
        var prop = typeof(AbstractRequestContext).GetProperty("AutoConverter",
            BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        prop.SetValue(request, converter);
    }

    private static void SetResultMap(AbstractRequestContext request, ResultMap resultMap)
    {
        var prop = typeof(AbstractRequestContext).GetProperty("ResultMap",
            BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        prop.SetValue(request, resultMap);
    }

    private ExecutionContext CreateContext<T>(Mock<DbDataReader> mockReader, ResultMap resultMap = null)
    {
        var wrapper = new DataReaderWrapper(mockReader.Object);
        var request = new RequestContext();
        SetAutoConverter(request, NoneAutoConverter.INSTANCE);
        if (resultMap != null)
        {
            SetResultMap(request, resultMap);
        }
        return new ExecutionContext
        {
            SmartSqlConfig = _config,
            DataReaderWrapper = wrapper,
            Request = request,
            Result = new SingleResultContext<T>()
        };
    }

    [Fact]
    public void CanDeserialize_Should_ReturnTrue_ForAnyType()
    {
        var context = CreateContext<string>(new Mock<DbDataReader>());

        _sut.CanDeserialize(context, typeof(string)).Should().BeTrue();
        _sut.CanDeserialize(context, typeof(int)).Should().BeTrue();
        _sut.CanDeserialize(context, typeof(DateTime)).Should().BeTrue();
        _sut.CanDeserialize(context, typeof(object)).Should().BeTrue();
    }

    [Fact]
    public void CanDeserialize_Should_ReturnTrue_When_IsMultipleFlag()
    {
        var context = CreateContext<object>(new Mock<DbDataReader>());

        _sut.CanDeserialize(context, typeof(object), isMultiple: true).Should().BeTrue();
    }

    [Fact]
    public void ToSingle_Should_ReturnDefault_When_DataReaderHasNoRows()
    {
        var mockReader = new Mock<DbDataReader>();
        mockReader.Setup(r => r.HasRows).Returns(false);

        var context = CreateContext<User>(mockReader);

        var result = _sut.ToSingle<User>(context);

        result.Should().BeNull();
    }

    [Fact]
    public void ToSingle_Should_ReturnDefault_When_DataReaderHasNoRowsForValueType()
    {
        var mockReader = new Mock<DbDataReader>();
        mockReader.Setup(r => r.HasRows).Returns(false);

        var context = CreateContext<int>(mockReader);

        var result = _sut.ToSingle<int>(context);

        result.Should().Be(0);
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
    public void ToSingle_Should_DeserializeEntity_When_HasMultipleColumnTypes()
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
    public void ToSingle_Should_DeserializeDifferentEntity_When_DifferentColumns()
    {
        var mockReader = new Mock<DbDataReader>();
        mockReader.Setup(r => r.HasRows).Returns(true);
        mockReader.Setup(r => r.Read()).Returns(true);
        mockReader.Setup(r => r.FieldCount).Returns(2);
        mockReader.Setup(r => r.GetName(0)).Returns("Id");
        mockReader.Setup(r => r.GetName(1)).Returns("Name");
        mockReader.Setup(r => r.GetFieldType(0)).Returns(typeof(long));
        mockReader.Setup(r => r.GetFieldType(1)).Returns(typeof(string));
        mockReader.Setup(r => r.IsDBNull(It.IsAny<int>())).Returns(false);
        mockReader.Setup(r => r.GetInt64(0)).Returns(99L);
        mockReader.Setup(r => r.GetValue(0)).Returns(99L);
        mockReader.Setup(r => r.GetString(1)).Returns("EntityName");
        mockReader.Setup(r => r.GetValue(1)).Returns("EntityName");

        var context = CreateContext<Entity>(mockReader);

        var result = _sut.ToSingle<Entity>(context);

        result.Should().NotBeNull();
        result.Id.Should().Be(99);
        result.Name.Should().Be("EntityName");
    }

    [Fact]
    public void ToSingle_Should_HandleAllNullColumns_When_ReaderReturnsDbNull()
    {
        var mockReader = new Mock<DbDataReader>();
        mockReader.Setup(r => r.HasRows).Returns(true);
        mockReader.Setup(r => r.Read()).Returns(true);
        mockReader.Setup(r => r.FieldCount).Returns(2);
        mockReader.Setup(r => r.GetName(0)).Returns("id");
        mockReader.Setup(r => r.GetName(1)).Returns("user_name");
        mockReader.Setup(r => r.GetFieldType(0)).Returns(typeof(long));
        mockReader.Setup(r => r.GetFieldType(1)).Returns(typeof(string));
        mockReader.Setup(r => r.IsDBNull(It.IsAny<int>())).Returns(true);

        var context = CreateContext<User>(mockReader);

        var result = _sut.ToSingle<User>(context);

        result.Should().NotBeNull();
        result.Id.Should().Be(0);
        result.UserName.Should().BeNull();
    }

    [Fact]
    public void ToList_Should_DeserializeMultipleRows()
    {
        var mockReader = new Mock<DbDataReader>();
        mockReader.Setup(r => r.HasRows).Returns(true);
        var readSequence = mockReader.SetupSequence(r => r.Read());
        readSequence.Returns(true);
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
        mockReader.Setup(r => r.GetInt64(0)).Returns(() => ++callCount);
        mockReader.Setup(r => r.GetValue(0)).Returns(() => (long)callCount);
        mockReader.Setup(r => r.GetString(1)).Returns(() => $"User{callCount}");
        mockReader.Setup(r => r.GetValue(1)).Returns(() => $"User{callCount}");

        var context = CreateContext<User>(mockReader);

        var result = _sut.ToList<User>(context);

        result.Should().HaveCount(3);
        result[0].Id.Should().Be(1);
        result[1].Id.Should().Be(2);
        result[2].Id.Should().Be(3);
    }

    [Fact]
    public async Task ToSingleAsync_Should_ReturnDefault_When_NoRows()
    {
        var mockReader = new Mock<DbDataReader>();
        mockReader.Setup(r => r.HasRows).Returns(false);

        var context = CreateContext<User>(mockReader);

        var result = await _sut.ToSingleAsync<User>(context);

        result.Should().BeNull();
    }

    [Fact]
    public async Task ToSingleAsync_Should_DeserializeEntity_When_HasRows()
    {
        var mockReader = new Mock<DbDataReader>();
        mockReader.Setup(r => r.HasRows).Returns(true);
        mockReader.Setup(r => r.ReadAsync(It.IsAny<System.Threading.CancellationToken>())).ReturnsAsync(true);
        mockReader.Setup(r => r.FieldCount).Returns(2);
        mockReader.Setup(r => r.GetName(0)).Returns("id");
        mockReader.Setup(r => r.GetName(1)).Returns("user_name");
        mockReader.Setup(r => r.GetFieldType(0)).Returns(typeof(long));
        mockReader.Setup(r => r.GetFieldType(1)).Returns(typeof(string));
        mockReader.Setup(r => r.IsDBNull(It.IsAny<int>())).Returns(false);
        mockReader.Setup(r => r.GetInt64(0)).Returns(99L);
        mockReader.Setup(r => r.GetValue(0)).Returns(99L);
        mockReader.Setup(r => r.GetString(1)).Returns("AsyncUser");
        mockReader.Setup(r => r.GetValue(1)).Returns("AsyncUser");

        var context = CreateContext<User>(mockReader);

        var result = await _sut.ToSingleAsync<User>(context);

        result.Should().NotBeNull();
        result.Id.Should().Be(99);
        result.UserName.Should().Be("AsyncUser");
    }

    [Fact]
    public async Task ToListAsync_Should_ReturnEmptyList_When_NoRows()
    {
        var mockReader = new Mock<DbDataReader>();
        mockReader.Setup(r => r.HasRows).Returns(false);

        var context = CreateContext<User>(mockReader);

        var result = await _sut.ToListAsync<User>(context);

        result.Should().BeEmpty();
    }

    [Fact]
    public async Task ToListAsync_Should_DeserializeMultipleRows()
    {
        var mockReader = new Mock<DbDataReader>();
        mockReader.Setup(r => r.HasRows).Returns(true);

        var readSequence = mockReader.SetupSequence(r => r.ReadAsync(It.IsAny<System.Threading.CancellationToken>()));
        readSequence.ReturnsAsync(true);
        readSequence.ReturnsAsync(true);
        readSequence.ReturnsAsync(false);

        mockReader.Setup(r => r.FieldCount).Returns(2);
        mockReader.Setup(r => r.GetName(0)).Returns("id");
        mockReader.Setup(r => r.GetName(1)).Returns("user_name");
        mockReader.Setup(r => r.GetFieldType(0)).Returns(typeof(long));
        mockReader.Setup(r => r.GetFieldType(1)).Returns(typeof(string));
        mockReader.Setup(r => r.IsDBNull(It.IsAny<int>())).Returns(false);

        var callCount = 0;
        mockReader.Setup(r => r.GetInt64(0)).Returns(() => ++callCount);
        mockReader.Setup(r => r.GetValue(0)).Returns(() => (long)callCount);
        mockReader.Setup(r => r.GetString(1)).Returns(() => $"AsyncUser{callCount}");
        mockReader.Setup(r => r.GetValue(1)).Returns(() => $"AsyncUser{callCount}");

        var context = CreateContext<User>(mockReader);

        var result = await _sut.ToListAsync<User>(context);

        result.Should().HaveCount(2);
        result[0].Id.Should().Be(1);
        result[1].Id.Should().Be(2);
    }

    [Fact]
    public void ToSingle_Should_ThrowSmartSqlException_When_NoParameterlessConstructor()
    {
        var mockReader = new Mock<DbDataReader>();
        mockReader.Setup(r => r.HasRows).Returns(true);
        mockReader.Setup(r => r.Read()).Returns(true);
        mockReader.Setup(r => r.FieldCount).Returns(1);
        mockReader.Setup(r => r.GetName(0)).Returns("value");
        mockReader.Setup(r => r.GetFieldType(0)).Returns(typeof(string));

        var context = CreateContext<NoDefaultCtor>(mockReader);

        var act = () => _sut.ToSingle<NoDefaultCtor>(context);

        act.Should().Throw<SmartSqlException>();
    }

    [Fact]
    public void ThrowDeserializeException_Should_ThrowWithDetails()
    {
        var mockReader = new Mock<DbDataReader>();
        mockReader.Setup(r => r.GetName(0)).Returns("expected_column");

        var ex = Record.Exception(() =>
            EntityDeserializer.ThrowDeserializeException(
                new InvalidOperationException("test"),
                new User { Id = 1 },
                0,
                "expected_column",
                "PropertyName",
                mockReader.Object));

        ex.Should().BeOfType<SmartSqlException>();
        ex.Message.Should().Contain("PropertyName");
        ex.Message.Should().Contain("expected_column");
        ex.Message.Should().Contain("Deserialize Error");
    }

    [Fact]
    public void DeserializeIdentity_Should_BeEqual_When_AliasResultIndexAndRealSqlMatch()
    {
        var context1 = CreateContext<object>(new Mock<DbDataReader>());
        var context2 = CreateContext<object>(new Mock<DbDataReader>());
        context1.SmartSqlConfig.Alias = "Test";
        context2.SmartSqlConfig.Alias = "Test";

        var identity1 = EntityDeserializer.DeserializeIdentity.Of(context1);
        var identity2 = EntityDeserializer.DeserializeIdentity.Of(context2);

        identity1.Equals(identity2).Should().BeTrue();
        identity1.GetHashCode().Should().Be(identity2.GetHashCode());
    }

    [Fact]
    public void DeserializeIdentity_Should_NotBeEqual_When_AliasDiffers()
    {
        var context1 = CreateContext<object>(new Mock<DbDataReader>());
        var mockReader2 = new Mock<DbDataReader>();
        var wrapper2 = new DataReaderWrapper(mockReader2.Object);
        var request2 = new RequestContext();
        SetAutoConverter(request2, NoneAutoConverter.INSTANCE);
        // Use a separate config with different Alias
        var config2 = new SmartSqlConfig { Alias = "DifferentAlias" };
        var context2 = new ExecutionContext
        {
            SmartSqlConfig = config2,
            DataReaderWrapper = wrapper2,
            Request = request2,
            Result = new SingleResultContext<object>()
        };
        context1.SmartSqlConfig.Alias = "OriginalAlias";

        var identity1 = EntityDeserializer.DeserializeIdentity.Of(context1);
        var identity2 = EntityDeserializer.DeserializeIdentity.Of(context2);

        identity1.Equals(identity2).Should().BeFalse();
    }

    [Fact]
    public void DeserializeIdentity_Should_NotBeEqual_When_ResultIndexDiffers()
    {
        var context1 = CreateContext<object>(new Mock<DbDataReader>());
        var mockReader2 = new Mock<DbDataReader>();
        mockReader2.Setup(r => r.NextResult()).Returns(false);
        var wrapper2 = new DataReaderWrapper(mockReader2.Object);
        var request2 = new RequestContext();
        SetAutoConverter(request2, NoneAutoConverter.INSTANCE);
        var context2 = new ExecutionContext
        {
            SmartSqlConfig = _config,
            DataReaderWrapper = wrapper2,
            Request = request2,
            Result = new SingleResultContext<object>()
        };
        wrapper2.NextResult(); // Increment ResultIndex to 1

        var identity1 = EntityDeserializer.DeserializeIdentity.Of(context1);
        var identity2 = EntityDeserializer.DeserializeIdentity.Of(context2);

        identity1.Equals(identity2).Should().BeFalse();
    }

    public class NoDefaultCtor
    {
        public NoDefaultCtor(string required)
        {
            Value = required;
        }

        public string Value { get; set; }
    }
}
