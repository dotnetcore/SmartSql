using System;
using FluentAssertions;
using SmartSql.DataSource;
using Xunit;

namespace SmartSql.Test.Unit.DataSource;

public class AbstractDataSourceTests
{
    private class TestDataSource : AbstractDataSource { }

    [Fact]
    public void Should_SetName_When_Assigned()
    {
        var ds = new TestDataSource { Name = "TestDb" };

        ds.Name.Should().Be("TestDb");
    }

    [Fact]
    public void Should_SetConnectionString_When_Assigned()
    {
        var ds = new TestDataSource { ConnectionString = "Server=localhost;Database=test" };

        ds.ConnectionString.Should().Be("Server=localhost;Database=test");
    }

    [Fact]
    public void Should_SetDbProvider_When_Assigned()
    {
        var provider = new DbProvider { Name = "TestProvider" };
        var ds = new TestDataSource { DbProvider = provider };

        ds.DbProvider.Should().Be(provider);
        ds.DbProvider.Name.Should().Be("TestProvider");
    }

    [Fact]
    public void Should_BeNull_When_NotInitialized()
    {
        var ds = new TestDataSource();

        ds.Name.Should().BeNull();
        ds.ConnectionString.Should().BeNull();
        ds.DbProvider.Should().BeNull();
    }
}
