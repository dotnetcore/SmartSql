using System.Collections.Generic;
using FluentAssertions;
using SmartSql.Configuration;
using Xunit;

namespace SmartSql.Test.Unit.Configuration;

public class ResultMapTests
{
    [Fact]
    public void Should_SetId_When_Assigned()
    {
        var resultMap = new ResultMap { Id = "TestMap" };

        resultMap.Id.Should().Be("TestMap");
    }

    [Fact]
    public void Should_SetConstructor_When_Assigned()
    {
        var ctor = new Constructor
        {
            Args = new List<Arg>
            {
                new Arg { Column = "Id", CSharpType = typeof(long) }
            }
        };
        var resultMap = new ResultMap { Constructor = ctor };

        resultMap.Constructor.Should().NotBeNull();
        resultMap.Constructor.Args.Should().HaveCount(1);
    }

    [Fact]
    public void Should_ReturnProperty_When_ColumnExists()
    {
        var resultMap = new ResultMap
        {
            Properties = new Dictionary<string, Property>
            {
                ["Id"] = new Property { Name = "Id", Column = "id" }
            }
        };

        var prop = resultMap.GetProperty("Id");

        prop.Should().NotBeNull();
        prop.Name.Should().Be("Id");
    }

    [Fact]
    public void Should_ReturnNull_When_ColumnDoesNotExist()
    {
        var resultMap = new ResultMap
        {
            Properties = new Dictionary<string, Property>()
        };

        var prop = resultMap.GetProperty("NonExistent");

        prop.Should().BeNull();
    }

    [Fact]
    public void Should_SetPropertyFields_When_Assigned()
    {
        var prop = new Property
        {
            Name = "UserName",
            Column = "user_name",
            TypeHandler = "JsonTypeHandler"
        };

        prop.Name.Should().Be("UserName");
        prop.Column.Should().Be("user_name");
        prop.TypeHandler.Should().Be("JsonTypeHandler");
    }

    [Fact]
    public void Should_SetArgFields_When_Assigned()
    {
        var arg = new Arg
        {
            Column = "id",
            CSharpType = typeof(long),
            Handler = null
        };

        arg.Column.Should().Be("id");
        arg.CSharpType.Should().Be(typeof(long));
        arg.Handler.Should().BeNull();
    }
}
