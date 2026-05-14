using System;
using System.Collections.Generic;
using FluentAssertions;
using SmartSql.Configuration;
using Xunit;

namespace SmartSql.Test.Unit.Configuration;

public class TypeHandlerConfigTests
{
    [Fact]
    public void Should_SetName_When_Assigned()
    {
        var handler = new SmartSql.Configuration.TypeHandler { Name = "JsonHandler" };

        handler.Name.Should().Be("JsonHandler");
    }

    [Fact]
    public void Should_SetPropertyType_When_Assigned()
    {
        var handler = new SmartSql.Configuration.TypeHandler { PropertyType = typeof(string) };

        handler.PropertyType.Should().Be(typeof(string));
    }

    [Fact]
    public void Should_SetFieldType_When_Assigned()
    {
        var handler = new SmartSql.Configuration.TypeHandler { FieldType = typeof(int) };

        handler.FieldType.Should().Be(typeof(int));
    }

    [Fact]
    public void Should_SetHandlerType_When_Assigned()
    {
        var handler = new SmartSql.Configuration.TypeHandler { HandlerType = typeof(object) };

        handler.HandlerType.Should().Be(typeof(object));
    }

    [Fact]
    public void Should_SetProperties_When_Assigned()
    {
        var props = new Dictionary<string, object> { ["DateFormat"] = "yyyy-MM-dd" };
        var handler = new SmartSql.Configuration.TypeHandler { Properties = props };

        handler.Properties.Should().ContainKey("DateFormat");
        handler.Properties["DateFormat"].Should().Be("yyyy-MM-dd");
    }

    [Fact]
    public void Should_BeNull_When_NotInitialized()
    {
        var handler = new SmartSql.Configuration.TypeHandler();

        handler.Name.Should().BeNull();
        handler.PropertyType.Should().BeNull();
        handler.FieldType.Should().BeNull();
        handler.HandlerType.Should().BeNull();
        handler.Properties.Should().BeNull();
    }
}
