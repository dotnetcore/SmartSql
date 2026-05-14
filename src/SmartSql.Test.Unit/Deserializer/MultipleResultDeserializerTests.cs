using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Reflection;
using FluentAssertions;
using Moq;
using SmartSql.Configuration;
using SmartSql.Data;
using SmartSql.Deserializer;
using SmartSql.Exceptions;
using SmartSql.TypeHandlers;
using Xunit;

namespace SmartSql.Test.Unit.Deserializer;

public class MultipleResultDeserializerTests
{
    private readonly SmartSqlConfig _config = new();

    private static void SetMultipleResultMap(AbstractRequestContext request, MultipleResultMap map)
    {
        var prop = typeof(AbstractRequestContext).GetProperty("MultipleResultMap",
            BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        prop.SetValue(request, map);
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
        SetMultipleResultMap(request, multipleResultMap);
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
    public void ToList_Should_Throw_When_Called()
    {
        var sut = CreateSut();
        var context = CreateContext<object>(new Mock<DbDataReader>(), new MultipleResultMap());

        var act = () => sut.ToList<object>(context);

        act.Should().Throw<SmartSqlException>();
    }

    [Fact]
    public void ToSingle_Should_Throw_When_NoMultipleResultMap()
    {
        var sut = CreateSut();
        var mockReader = new Mock<DbDataReader>();
        mockReader.Setup(r => r.HasRows).Returns(true);

        var context = CreateContext<object>(mockReader, null);

        var act = () => sut.ToSingle<object>(context);

        act.Should().Throw<NullReferenceException>();
    }
}
