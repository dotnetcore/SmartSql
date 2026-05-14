using System;
using System.Collections.Generic;
using FluentAssertions;
using Xunit;

namespace SmartSql.Test.Unit;

public class ResultContextTests
{
    [Fact]
    public void Should_HaveDefaultState_When_SingleResultContextCreated()
    {
        var context = new SingleResultContext<string>();

        context.End.Should().BeFalse();
        context.IsList.Should().BeFalse();
        context.FromCache.Should().BeFalse();
        context.ResultType.Should().Be(typeof(string));
        context.GetData().Should().BeNull();
    }

    [Fact]
    public void Should_SetData_When_SingleResultSetDataCalled()
    {
        var context = new SingleResultContext<string>();

        context.SetData("hello");

        context.Data.Should().Be("hello");
        context.GetData().Should().Be("hello");
        context.End.Should().BeTrue();
        context.FromCache.Should().BeFalse();
    }

    [Fact]
    public void Should_SetFromCache_When_SingleResultSetDataCalledWithFromCache()
    {
        var context = new SingleResultContext<int>();

        context.SetData(42, fromCache: true);

        context.Data.Should().Be(42);
        context.FromCache.Should().BeTrue();
        context.End.Should().BeTrue();
    }

    [Fact]
    public void Should_HaveDefaultState_When_ListResultContextCreated()
    {
        var context = new ListResultContext<int>();

        context.End.Should().BeFalse();
        context.IsList.Should().BeTrue();
        context.FromCache.Should().BeFalse();
        context.ResultType.Should().Be(typeof(IList<int>));
        context.GetData().Should().BeNull();
    }

    [Fact]
    public void Should_SetData_When_ListResultSetDataCalled()
    {
        var context = new ListResultContext<string>();
        var data = new List<string> { "a", "b" };

        context.SetData(data);

        context.Data.Should().BeEquivalentTo(data);
        context.GetData().Should().BeSameAs(data);
        context.End.Should().BeTrue();
        context.FromCache.Should().BeFalse();
    }

    [Fact]
    public void Should_SetFromCache_When_ListResultSetDataCalledWithFromCache()
    {
        var context = new ListResultContext<long>();
        var data = new List<long> { 1, 2, 3 };

        context.SetData(data, fromCache: true);

        context.Data.Should().BeEquivalentTo(data);
        context.FromCache.Should().BeTrue();
        context.End.Should().BeTrue();
    }

    [Fact]
    public void Should_ReturnCorrectResultType_When_ValueTypeUsed()
    {
        var context = new SingleResultContext<int>();

        context.ResultType.Should().Be(typeof(int));
    }

    [Fact]
    public void Should_ReturnCorrectResultType_When_ListContextValueTypeUsed()
    {
        var context = new ListResultContext<DateTime>();

        context.ResultType.Should().Be(typeof(IList<DateTime>));
    }
}
