using System;
using System.Collections.Generic;
using FluentAssertions;
using SmartSql;
using SmartSql.ConfigBuilder;
using SmartSql.Configuration;
using SmartSql.DataSource;
using SmartSql.Exceptions;
using Xunit;

namespace SmartSql.Test.Unit;

public class SmartSqlBuilderTests
{
    [Fact]
    public void Should_BuildSuccessfully_When_UseDataSourceWithDbProvider()
    {
        var builder = new SmartSqlBuilder()
            .UseAlias("TestBuilder_DataSource")
            .UseDataSource(DbProvider.SQLITE, "Data Source=:memory:")
            .RegisterToContainer(false);

        var result = builder.Build();

        result.Should().NotBeNull();
        result.SmartSqlConfig.Should().NotBeNull();
        result.SmartSqlConfig.Database.Should().NotBeNull();
        result.SmartSqlConfig.Database.Write.ConnectionString.Should().Be("Data Source=:memory:");
    }

    [Fact]
    public void Should_SetAlias_When_UseAliasCalled()
    {
        var builder = new SmartSqlBuilder()
            .UseAlias("TestBuilder_Alias")
            .UseDataSource(DbProvider.SQLITE, "Data Source=:memory:")
            .RegisterToContainer(false);

        builder.Build();

        builder.Alias.Should().Be("TestBuilder_Alias");
    }

    [Fact]
    public void Should_SetDefaultAlias_When_UseAliasNotCalled()
    {
        var builder = new SmartSqlBuilder()
            .UseDataSource(DbProvider.SQLITE, "Data Source=:memory:")
            .RegisterToContainer(false);

        builder.Build();

        builder.Alias.Should().Be(SmartSqlBuilder.DEFAULT_ALIAS);
    }

    [Fact]
    public void Should_Throw_When_UseAliasWithNull()
    {
        var act = () => new SmartSqlBuilder().UseAlias(null);

        act.Should().Throw<ArgumentNullException>()
            .WithMessage("*alias*");
    }

    [Fact]
    public void Should_Throw_When_UseAliasWithEmpty()
    {
        var act = () => new SmartSqlBuilder().UseAlias(string.Empty);

        act.Should().Throw<ArgumentNullException>()
            .WithMessage("*alias*");
    }

    [Fact]
    public void Should_Throw_When_UseDataSourceWithNullDbProviderName()
    {
        var act = () => new SmartSqlBuilder().UseDataSource(null, "Data Source=:memory:");

        act.Should().Throw<ArgumentNullException>()
            .WithMessage("*dbProviderName*");
    }

    [Fact]
    public void Should_Throw_When_UseDataSourceWithNullConnectionString()
    {
        var act = () => new SmartSqlBuilder().UseDataSource(DbProvider.SQLITE, null);

        act.Should().Throw<ArgumentNullException>()
            .WithMessage("*connectionString*");
    }

    [Fact]
    public void Should_Throw_When_UseDataSourceWithUnknownProvider()
    {
        var act = () => new SmartSqlBuilder().UseDataSource("UnknownProvider", "Data Source=:memory:");

        act.Should().Throw<SmartSqlException>()
            .WithMessage("*UnknownProvider*");
    }

    [Fact]
    public void Should_Throw_When_UseNativeConfigWithNull()
    {
        var act = () => new SmartSqlBuilder().UseNativeConfig(null);

        act.Should().Throw<ArgumentNullException>()
            .WithMessage("*smartSqlConfig*");
    }

    [Fact]
    public void Should_BuildWithNativeConfig_When_UseNativeConfigProvided()
    {
        var config = CreateBasicSmartSqlConfig();
        var builder = new SmartSqlBuilder()
            .UseAlias("TestBuilder_Native")
            .UseNativeConfig(config)
            .RegisterToContainer(false);

        builder.Build();

        builder.SmartSqlConfig.Database.Write.ConnectionString.Should().Be("Data Source=:memory:");
    }

    [Fact]
    public void Should_BuildOnce_When_BuildCalledMultipleTimes()
    {
        var builder = new SmartSqlBuilder()
            .UseAlias("TestBuilder_Idempotent")
            .UseDataSource(DbProvider.SQLITE, "Data Source=:memory:")
            .RegisterToContainer(false);

        var first = builder.Build();
        var second = builder.Build();

        first.Should().BeSameAs(second);
        builder.Built.Should().BeTrue();
    }

    [Fact]
    public void Should_StoreCacheFlag_When_UseCacheCalled()
    {
        var builder = new SmartSqlBuilder()
            .UseAlias("TestBuilder_Cache")
            .UseDataSource(DbProvider.SQLITE, "Data Source=:memory:")
            .UseCache(true)
            .RegisterToContainer(false);

        builder.IsCacheEnabled.Should().BeTrue();
    }

    [Fact]
    public void Should_StoreCacheDisabled_When_UseCacheFalse()
    {
        var builder = new SmartSqlBuilder()
            .UseAlias("TestBuilder_NoCache")
            .UseDataSource(DbProvider.SQLITE, "Data Source=:memory:")
            .UseCache(false)
            .RegisterToContainer(false);

        builder.IsCacheEnabled.Should().BeFalse();
    }

    [Fact]
    public void Should_IgnoreDbNull_When_UseIgnoreDbNullEnabled()
    {
        var builder = new SmartSqlBuilder()
            .UseAlias("TestBuilder_IgnoreDbNull")
            .UseDataSource(DbProvider.SQLITE, "Data Source=:memory:")
            .UseIgnoreDbNull(true)
            .RegisterToContainer(false);

        builder.Build();

        builder.SmartSqlConfig.Settings.IgnoreDbNull.Should().BeTrue();
    }

    [Fact]
    public void Should_CreateSqlMapper_When_Built()
    {
        var builder = new SmartSqlBuilder()
            .UseAlias("TestBuilder_SqlMapper")
            .UseDataSource(DbProvider.SQLITE, "Data Source=:memory:")
            .RegisterToContainer(false);

        builder.Build();

        builder.SqlMapper.Should().NotBeNull();
    }

    [Fact]
    public void Should_CreateDbSessionFactory_When_Built()
    {
        var builder = new SmartSqlBuilder()
            .UseAlias("TestBuilder_SessionFactory")
            .UseDataSource(DbProvider.SQLITE, "Data Source=:memory:")
            .RegisterToContainer(false);

        builder.Build();

        builder.DbSessionFactory.Should().NotBeNull();
    }

    [Fact]
    public void Should_SetProperties_When_UsePropertiesCalled()
    {
        var properties = new List<KeyValuePair<string, string>>
        {
            new("TestKey", "TestValue")
        };
        var builder = new SmartSqlBuilder()
            .UseAlias("TestBuilder_Properties")
            .UseDataSource(DbProvider.SQLITE, "Data Source=:memory:")
            .UseProperties(properties)
            .RegisterToContainer(false);

        builder.ImportProperties.Should().ContainSingle();
        builder.ImportProperties[0].Key.Should().Be("TestKey");
        builder.ImportProperties[0].Value.Should().Be("TestValue");
    }

    [Fact]
    public void Should_HavePipeline_When_Built()
    {
        var builder = new SmartSqlBuilder()
            .UseAlias("TestBuilder_Pipeline")
            .UseDataSource(DbProvider.SQLITE, "Data Source=:memory:")
            .RegisterToContainer(false);

        builder.Build();

        builder.SmartSqlConfig.Pipeline.Should().NotBeNull();
    }

    [Fact]
    public void Should_UseXmlConfig_When_UseXmlConfigCalled()
    {
        var builder = new SmartSqlBuilder()
            .UseAlias("TestBuilder_XmlConfig")
            .UseXmlConfig(ResourceType.File, "SmartSqlMapConfig-UnitTest.xml")
            .RegisterToContainer(false);

        builder.Build();

        builder.SmartSqlConfig.Should().NotBeNull();
        builder.SmartSqlConfig.Database.DbProvider.Name.Should().Be("SQLite");
    }

    private static SmartSqlConfig CreateBasicSmartSqlConfig()
    {
        return new SmartSqlConfig
        {
            Database = new Database
            {
                DbProvider = DbProviderManager.SQLITE_DBPROVIDER,
                Write = new WriteDataSource
                {
                    Name = "Write",
                    ConnectionString = "Data Source=:memory:"
                }
            }
        };
    }
}
