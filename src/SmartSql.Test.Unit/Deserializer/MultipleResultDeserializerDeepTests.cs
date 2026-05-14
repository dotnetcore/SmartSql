using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Reflection;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using SmartSql.Configuration;
using SmartSql.Data;
using SmartSql.Deserializer;
using SmartSql.Exceptions;
using SmartSql.Test.Unit.TestEntities;
using Xunit;

namespace SmartSql.Test.Unit.Deserializer;

public class MultipleResultDeserializerDeepTests
{
    private readonly SmartSqlConfig _config;

    public MultipleResultDeserializerDeepTests()
    {
        _config = new SmartSqlConfig();
    }

    private static void SetMultipleResultMap(AbstractRequestContext request, MultipleResultMap map)
    {
        typeof(AbstractRequestContext).GetProperty("MultipleResultMap",
            BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
            .SetValue(request, map);
    }

    private MultipleResultDeserializer CreateSut()
    {
        var deserializerFactory = new DeserializerFactory();
        deserializerFactory.Add(new ValueTypeDeserializer());
        return new MultipleResultDeserializer(deserializerFactory);
    }

    private ExecutionContext CreateContext<T>(Mock<DbDataReader> mockReader, MultipleResultMap multipleResultMap)
    {
        var wrapper = new DataReaderWrapper(mockReader.Object);
        var request = new RequestContext();
        if (multipleResultMap != null)
        {
            SetMultipleResultMap(request, multipleResultMap);
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
    public void CanDeserialize_Should_ReturnTrue_When_IsMultipleAndNotValueTuple()
    {
        var sut = CreateSut();
        var context = CreateContext<object>(new Mock<DbDataReader>(), new MultipleResultMap());

        sut.CanDeserialize(context, typeof(object), isMultiple: true).Should().BeTrue();
    }

    [Fact]
    public void CanDeserialize_Should_ReturnTrue_When_IsMultipleAndString()
    {
        var sut = CreateSut();
        var context = CreateContext<string>(new Mock<DbDataReader>(), new MultipleResultMap());

        // MultipleResultDeserializer returns true for any non-ValueTuple when isMultiple=true
        sut.CanDeserialize(context, typeof(string), isMultiple: true).Should().BeTrue();
    }

    [Fact]
    public void CanDeserialize_Should_ReturnTrue_When_IsMultipleAndList()
    {
        var sut = CreateSut();
        var context = CreateContext<List<User>>(new Mock<DbDataReader>(), new MultipleResultMap());

        // MultipleResultDeserializer returns true for any non-ValueTuple when isMultiple=true
        sut.CanDeserialize(context, typeof(List<User>), isMultiple: true).Should().BeTrue();
    }

    [Fact]
    public void CanDeserialize_Should_ReturnTrue_When_IsMultipleAndCustomClass()
    {
        var sut = CreateSut();
        var context = CreateContext<UserWithAddress>(new Mock<DbDataReader>(), new MultipleResultMap());

        sut.CanDeserialize(context, typeof(UserWithAddress), isMultiple: true).Should().BeTrue();
    }

    [Fact]
    public void CanDeserialize_Should_ReturnFalse_When_NotMultiple()
    {
        var sut = CreateSut();
        var context = CreateContext<object>(new Mock<DbDataReader>(), new MultipleResultMap());

        sut.CanDeserialize(context, typeof(object), isMultiple: false).Should().BeFalse();
    }

    [Fact]
    public void CanDeserialize_Should_ReturnFalse_When_IsMultipleButValueTuple()
    {
        var sut = CreateSut();
        var context = CreateContext<ValueTuple<int, string>>(new Mock<DbDataReader>(), new MultipleResultMap());

        sut.CanDeserialize(context, typeof(ValueTuple<int, string>), isMultiple: true).Should().BeFalse();
    }

    [Fact]
    public void ToList_Should_ThrowSmartSqlException_When_Called()
    {
        var sut = CreateSut();
        var context = CreateContext<object>(new Mock<DbDataReader>(), new MultipleResultMap());

        var act = () => sut.ToList<object>(context);

        act.Should().Throw<SmartSqlException>()
            .WithMessage("*MultipleResultDeserializer*");
    }

    [Fact]
    public async Task ToListAsync_Should_ThrowSmartSqlException_When_Called()
    {
        var sut = CreateSut();
        var context = CreateContext<object>(new Mock<DbDataReader>(), new MultipleResultMap());

        var act = async () => await sut.ToListAsync<object>(context);

        await act.Should().ThrowAsync<SmartSqlException>()
            .WithMessage("*MultipleResultDeserializer*");
    }

    [Fact]
    public void ToSingle_Should_CreateEmptyInstance_When_NoRootAndNoResults()
    {
        var sut = CreateSut();
        var mockReader = new Mock<DbDataReader>();
        mockReader.Setup(r => r.HasRows).Returns(false);

        var multipleResultMap = new MultipleResultMap
        {
            Root = null,
            Results = new List<Result>()
        };

        var context = CreateContext<UserWithAddress>(mockReader, multipleResultMap);

        var result = sut.ToSingle<UserWithAddress>(context);

        result.Should().NotBeNull();
    }

    public class UserWithAddress
    {
        public UserWithAddress()
        {
            Address = new Address();
        }

        public long Id { get; set; }
        public Address Address { get; set; }
    }

    public class Address
    {
        public string Street { get; set; }
    }
}
