using System.Collections.Generic;
using FluentAssertions;
using SmartSql.ConfigBuilder;
using SmartSql.Configuration;
using Xunit;

namespace SmartSql.Test.Unit.ConfigBuilder;

public class RootConfigBuilderTests
{
    #region Constructor

    [Fact]
    public void Should_CreateSmartSqlConfig_When_NoArgs()
    {
        var builder = new RootConfigBuilder();

        builder.SmartSqlConfig.Should().NotBeNull();
    }

    [Fact]
    public void Should_BeInitialized_When_Created()
    {
        var builder = new RootConfigBuilder();

        builder.Initialized.Should().BeTrue();
    }

    [Fact]
    public void Should_HaveNoParent_When_Created()
    {
        var builder = new RootConfigBuilder();

        builder.Parent.Should().BeNull();
    }

    #endregion

    #region Constructor with Properties

    [Fact]
    public void Should_ImportProperties_When_PropertiesProvided()
    {
        var properties = new List<KeyValuePair<string, string>>
        {
            new KeyValuePair<string, string>("Key1", "Value1"),
            new KeyValuePair<string, string>("Key2", "Value2")
        };

        var builder = new RootConfigBuilder(properties);

        builder.SmartSqlConfig.Should().NotBeNull();
    }

    [Fact]
    public void Should_NotImportProperties_When_NullProvided()
    {
        var builder = new RootConfigBuilder(null);

        builder.SmartSqlConfig.Should().NotBeNull();
    }

    [Fact]
    public void Should_BeInitialized_When_CreatedWithProperties()
    {
        var properties = new List<KeyValuePair<string, string>>
        {
            new KeyValuePair<string, string>("${Key1}", "Value1")
        };

        var builder = new RootConfigBuilder(properties);

        builder.Initialized.Should().BeTrue();
    }

    #endregion

    #region Build

    [Fact]
    public void Should_ReturnSmartSqlConfig_When_Built()
    {
        var builder = new RootConfigBuilder();

        var config = builder.Build();

        config.Should().BeSameAs(builder.SmartSqlConfig);
    }

    [Fact]
    public void Should_ReturnSameConfig_When_BuiltMultipleTimes()
    {
        var builder = new RootConfigBuilder();

        var config1 = builder.Build();
        var config2 = builder.Build();

        config2.Should().BeSameAs(config1);
    }

    #endregion

    #region SetParent

    [Fact]
    public void Should_SetParent_When_ValidParent()
    {
        var builder = new RootConfigBuilder();
        var parent = new RootConfigBuilder();

        builder.SetParent(parent);

        // RootConfigBuilder.SetParent does nothing
        builder.Parent.Should().BeNull();
    }

    #endregion

    #region Dispose

    [Fact]
    public void Should_Dispose_When_Called()
    {
        var builder = new RootConfigBuilder();

        var act = () => builder.Dispose();

        act.Should().NotThrow();
    }

    #endregion

    #region SmartSqlConfig Defaults

    [Fact]
    public void Should_HaveSettings_When_Created()
    {
        var builder = new RootConfigBuilder();

        builder.SmartSqlConfig.Settings.Should().NotBeNull();
        builder.SmartSqlConfig.Settings.ParameterPrefix.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void Should_HaveDefaultSqlMaps_When_Created()
    {
        var builder = new RootConfigBuilder();

        builder.SmartSqlConfig.SqlMaps.Should().NotBeNull();
        builder.SmartSqlConfig.SqlMaps.Should().BeEmpty();
    }

    [Fact]
    public void Should_HaveDefaultIdGenerators_When_Created()
    {
        var builder = new RootConfigBuilder();

        builder.SmartSqlConfig.IdGenerators.Should().NotBeNull();
        builder.SmartSqlConfig.IdGenerators.Should().NotBeEmpty();
    }

    #endregion
}
