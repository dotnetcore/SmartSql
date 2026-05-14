using System.Collections.Generic;
using FluentAssertions;
using SmartSql.Configuration;
using Xunit;

namespace SmartSql.Test.Unit.Configuration;

public class MultipleResultMapTests
{
    [Fact]
    public void Should_SetId_When_Assigned()
    {
        var map = new MultipleResultMap { Id = "MultiMap" };

        map.Id.Should().Be("MultiMap");
    }

    [Fact]
    public void Should_SetRoot_When_Assigned()
    {
        var resultMap = new ResultMap { Id = "RootMap" };
        var result = new Result { Property = Result.ROOT_PROPERTY, MapId = "RootMap", Map = resultMap };
        var map = new MultipleResultMap { Root = result };

        map.Root.Should().NotBeNull();
        map.Root.Property.Should().Be(Result.ROOT_PROPERTY);
    }

    [Fact]
    public void Should_ReturnResultMapByIndex_When_IndexIsValid()
    {
        var resultMap1 = new ResultMap { Id = "Map1" };
        var resultMap2 = new ResultMap { Id = "Map2" };
        var map = new MultipleResultMap
        {
            Results = new List<Result>
            {
                new Result { MapId = "Map1", Map = resultMap1 },
                new Result { MapId = "Map2", Map = resultMap2 }
            }
        };

        map.GetResultMap(0).Id.Should().Be("Map1");
        map.GetResultMap(1).Id.Should().Be("Map2");
    }

    [Fact]
    public void Should_ThrowArgumentOutOfRange_When_IndexIsInvalid()
    {
        var map = new MultipleResultMap
        {
            Results = new List<Result>()
        };

        var act = () => map.GetResultMap(0);

        act.Should().Throw<System.ArgumentOutOfRangeException>();
    }

    [Fact]
    public void Should_ReturnNullMap_When_ResultMapIsNull()
    {
        var map = new MultipleResultMap
        {
            Results = new List<Result>
            {
                new Result { MapId = "Null", Map = null }
            }
        };

        map.GetResultMap(0).Should().BeNull();
    }

    [Fact]
    public void Should_SetResultProperty_When_Assigned()
    {
        var result = new Result
        {
            Property = "OrderItems",
            MapId = "ItemMap",
            Map = new ResultMap { Id = "ItemMap" }
        };

        result.Property.Should().Be("OrderItems");
        result.MapId.Should().Be("ItemMap");
        result.Map.Should().NotBeNull();
    }

    [Fact]
    public void Should_HaveRootPropertyConstant_When_Accessed()
    {
        Result.ROOT_PROPERTY.Should().Be("__ROOT__");
    }
}
