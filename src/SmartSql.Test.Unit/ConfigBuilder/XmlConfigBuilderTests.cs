using FluentAssertions;
using SmartSql.ConfigBuilder;
using SmartSql.Configuration;
using Xunit;

namespace SmartSql.Test.Unit.ConfigBuilder;

public class XmlConfigBuilderTests
{
    [Fact]
    public void Should_LoadConfig_When_ValidXmlFile()
    {
        var configBuilder = new XmlConfigBuilder(ResourceType.File, "SmartSqlMapConfig-UnitTest.xml");

        var config = configBuilder.Build();

        config.Should().NotBeNull();
    }

    [Fact]
    public void Should_ParseDatabase_When_ValidConfig()
    {
        var configBuilder = new XmlConfigBuilder(ResourceType.File, "SmartSqlMapConfig-UnitTest.xml");

        var config = configBuilder.Build();

        config.Database.Should().NotBeNull();
        config.Database.Write.Should().NotBeNull();
        config.Database.Write.Name.Should().Be("WriteDB");
        config.Database.DbProvider.Should().NotBeNull();
    }

    [Fact]
    public void Should_ParseDbProvider_When_ConfigHasDbProvider()
    {
        var configBuilder = new XmlConfigBuilder(ResourceType.File, "SmartSqlMapConfig-UnitTest.xml");

        var config = configBuilder.Build();

        config.Database.DbProvider.Name.Should().Be("SQLite");
    }

    [Fact]
    public void Should_ParseConnectionString_When_ConfigHasPropertyReference()
    {
        var configBuilder = new XmlConfigBuilder(ResourceType.File, "SmartSqlMapConfig-UnitTest.xml");

        var config = configBuilder.Build();

        config.Database.Write.ConnectionString.Should().Be("Data Source=:memory:");
    }

    [Fact]
    public void Should_ParseProperties_When_ConfigHasProperties()
    {
        var configBuilder = new XmlConfigBuilder(ResourceType.File, "SmartSqlMapConfig-UnitTest.xml");

        var config = configBuilder.Build();

        config.Properties.Should().NotBeNull();
        var value = config.Properties.GetPropertyValue("${ConnectionString}");
        value.Should().Be("Data Source=:memory:");
    }

    [Fact]
    public void Should_ParseSettings_When_ConfigHasSettings()
    {
        var configBuilder = new XmlConfigBuilder(ResourceType.File, "SmartSqlMapConfig-UnitTest.xml");

        var config = configBuilder.Build();

        config.Settings.Should().NotBeNull();
        config.Settings.IgnoreParameterCase.Should().BeFalse();
        config.Settings.ParameterPrefix.Should().Be("$");
        config.Settings.IsCacheEnabled.Should().BeFalse();
    }

    [Fact]
    public void Should_ParseEmptySqlMaps_When_ConfigHasNoMaps()
    {
        var configBuilder = new XmlConfigBuilder(ResourceType.File, "SmartSqlMapConfig-UnitTest.xml");

        var config = configBuilder.Build();

        config.SqlMaps.Should().NotBeNull();
        config.SqlMaps.Should().BeEmpty();
    }

    [Fact]
    public void Should_ReturnSameConfig_When_BuildCalledTwice()
    {
        var configBuilder = new XmlConfigBuilder(ResourceType.File, "SmartSqlMapConfig-UnitTest.xml");

        var config1 = configBuilder.Build();
        var config2 = configBuilder.Build();

        config2.Should().BeSameAs(config1);
        configBuilder.Initialized.Should().BeTrue();
    }

    [Fact]
    public void Should_ParseEmptyTypeHandlers_When_ConfigHasEmptyTypeHandlers()
    {
        var configBuilder = new XmlConfigBuilder(ResourceType.File, "SmartSqlMapConfig-UnitTest.xml");

        var config = configBuilder.Build();

        config.TypeHandlerFactory.Should().NotBeNull();
    }

    [Fact]
    public void Should_ParseEmptyIdGenerators_When_ConfigHasEmptyIdGenerators()
    {
        var configBuilder = new XmlConfigBuilder(ResourceType.File, "SmartSqlMapConfig-UnitTest.xml");

        var config = configBuilder.Build();

        config.IdGenerators.Should().NotBeNull();
        config.IdGenerators.Should().ContainKey("Default");
    }
}
