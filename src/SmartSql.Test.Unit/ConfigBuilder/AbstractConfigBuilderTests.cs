using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using FluentAssertions;
using SmartSql.ConfigBuilder;
using SmartSql.Configuration;
using SmartSql.Exceptions;
using SmartSql.Utils;
using Xunit;

namespace SmartSql.Test.Unit.ConfigBuilder;

public class AbstractConfigBuilderTests
{
    #region Build Order and Initialization

    [Fact]
    public void Should_SetInitialized_When_BuildSucceeds()
    {
        var configBuilder = new XmlConfigBuilder(ResourceType.File, "SmartSqlMapConfig-UnitTest.xml");

        configBuilder.Build();

        configBuilder.Initialized.Should().BeTrue();
    }

    [Fact]
    public void Should_ReturnSameConfig_When_CalledMultipleTimes()
    {
        var configBuilder = new XmlConfigBuilder(ResourceType.File, "SmartSqlMapConfig-UnitTest.xml");

        var config1 = configBuilder.Build();
        var config2 = configBuilder.Build();

        config2.Should().BeSameAs(config1);
    }

    [Fact]
    public void Should_InitializeNewConfig_When_NoParent()
    {
        var configBuilder = new XmlConfigBuilder(ResourceType.File, "SmartSqlMapConfig-UnitTest.xml");

        var config = configBuilder.Build();

        config.Should().NotBeNull();
        config.Settings.Should().NotBeNull();
        config.Properties.Should().NotBeNull();
        config.SqlMaps.Should().NotBeNull();
    }

    #endregion

    #region SetParent

    [Fact]
    public void Should_SetParent_When_ValidParent()
    {
        var parent = new XmlConfigBuilder(ResourceType.File, "SmartSqlMapConfig-UnitTest.xml");
        var child = new XmlConfigBuilder(ResourceType.File, "SmartSqlMapConfig-UnitTest.xml");

        child.SetParent(parent);

        child.Parent.Should().BeSameAs(parent);
    }

    [Fact]
    public void Should_ThrowException_When_SetParentToSelf()
    {
        var configBuilder = new XmlConfigBuilder(ResourceType.File, "SmartSqlMapConfig-UnitTest.xml");

        var act = () => configBuilder.SetParent(configBuilder);

        act.Should().Throw<SmartSqlException>()
            .WithMessage("*can't be self*");
    }

    [Fact]
    public void Should_HaveParent_When_ParentIsSet()
    {
        var parent = new XmlConfigBuilder(ResourceType.File, "SmartSqlMapConfig-UnitTest.xml");
        var child = new XmlConfigBuilder(ResourceType.File, "SmartSqlMapConfig-UnitTest.xml");
        child.SetParent(parent);

        child.Parent.Should().BeSameAs(parent);
    }

    #endregion

    #region Dispose

    [Fact]
    public void Should_Dispose_When_Called()
    {
        var configBuilder = new XmlConfigBuilder(ResourceType.File, "SmartSqlMapConfig-UnitTest.xml");
        configBuilder.Build();

        var act = () => configBuilder.Dispose();

        act.Should().NotThrow();
    }

    [Fact]
    public void Should_DisposeMultipleTimes_When_CalledMultipleTimes()
    {
        var configBuilder = new XmlConfigBuilder(ResourceType.File, "SmartSqlMapConfig-UnitTest.xml");
        configBuilder.Build();

        var act = () =>
        {
            configBuilder.Dispose();
            configBuilder.Dispose();
        };

        act.Should().NotThrow();
    }

    #endregion

    #region Settings Parsing

    [Fact]
    public void Should_ParseSettings_When_SettingsExist()
    {
        var configBuilder = new XmlConfigBuilder(ResourceType.File, "SmartSqlMapConfig-DeepTest.xml");

        var config = configBuilder.Build();

        config.Settings.Should().NotBeNull();
        config.Settings.IgnoreParameterCase.Should().BeTrue();
        config.Settings.ParameterPrefix.Should().Be("@");
        config.Settings.IsCacheEnabled.Should().BeTrue();
        config.Settings.EnablePropertyChangedTrack.Should().BeTrue();
        config.Settings.IgnoreDbNull.Should().BeTrue();
    }

    [Fact]
    public void Should_HaveSettings_When_SettingsNotSpecified()
    {
        var configBuilder = new XmlConfigBuilder(ResourceType.File, "SmartSqlMapConfig-NoSettings.xml");

        var config = configBuilder.Build();

        config.Settings.Should().NotBeNull();
        config.Settings.ParameterPrefix.Should().NotBeNullOrEmpty();
    }

    #endregion

    #region Properties Parsing

    [Fact]
    public void Should_ParseProperties_When_PropertiesExist()
    {
        var configBuilder = new XmlConfigBuilder(ResourceType.File, "SmartSqlMapConfig-DeepTest.xml");

        var config = configBuilder.Build();

        config.Properties.Should().NotBeNull();
        config.Properties.GetPropertyValue("${DbProvider}").Should().Be("SQLite");
        config.Properties.GetPropertyValue("${ConnectionString}").Should().Be("Data Source=:memory:");
        config.Properties.GetPropertyValue("${ReadConnStr}").Should().Be("Data Source=:memory:Read");
    }

    #endregion

    #region IdGenerators Parsing

    [Fact]
    public void Should_ParseIdGenerators_When_IdGeneratorsExist()
    {
        var configBuilder = new XmlConfigBuilder(ResourceType.File, "SmartSqlMapConfig-DeepTest.xml");

        var config = configBuilder.Build();

        config.IdGenerators.Should().NotBeNull();
        config.IdGenerators.Should().ContainKey("SnowflakeId");
        config.IdGenerators["SnowflakeId"].Should().NotBeNull();
    }

    [Fact]
    public void Should_BeEmpty_When_NoIdGeneratorsConfigured()
    {
        var configBuilder = new XmlConfigBuilder(ResourceType.File, "SmartSqlMapConfig-UnitTest.xml");

        var config = configBuilder.Build();

        config.IdGenerators.Should().NotBeNull();
        config.IdGenerators.Should().BeEmpty();
    }

    #endregion

    #region Database Parsing

    [Fact]
    public void Should_ParseDatabase_When_FullDatabaseConfig()
    {
        var configBuilder = new XmlConfigBuilder(ResourceType.File, "SmartSqlMapConfig-DeepTest.xml");

        var config = configBuilder.Build();

        config.Database.Should().NotBeNull();
        config.Database.DbProvider.Should().NotBeNull();
        config.Database.Write.Should().NotBeNull();
        config.Database.Reads.Should().NotBeEmpty();
    }

    [Fact]
    public void Should_ParseDbProvider_When_DbProviderConfigured()
    {
        var configBuilder = new XmlConfigBuilder(ResourceType.File, "SmartSqlMapConfig-DeepTest.xml");

        var config = configBuilder.Build();

        config.Database.DbProvider.Name.Should().Be("SQLite");
    }

    [Fact]
    public void Should_ParseWriteDataSource_When_WriteConfigured()
    {
        var configBuilder = new XmlConfigBuilder(ResourceType.File, "SmartSqlMapConfig-DeepTest.xml");

        var config = configBuilder.Build();

        config.Database.Write.Should().NotBeNull();
        config.Database.Write.Name.Should().Be("WriteDB");
        config.Database.Write.ConnectionString.Should().Be("Data Source=:memory:");
        config.Database.Write.DbProvider.Should().BeSameAs(config.Database.DbProvider);
    }

    [Fact]
    public void Should_ParseReadDataSources_When_ReadsConfigured()
    {
        var configBuilder = new XmlConfigBuilder(ResourceType.File, "SmartSqlMapConfig-DeepTest.xml");

        var config = configBuilder.Build();

        config.Database.Reads.Should().HaveCount(2);
        config.Database.Reads["ReadDb-1"].Should().NotBeNull();
        config.Database.Reads["ReadDb-2"].Should().NotBeNull();
        config.Database.Reads["ReadDb-1"].Weight.Should().Be(100);
        config.Database.Reads["ReadDb-2"].Weight.Should().Be(50);
    }

    [Fact]
    public void Should_AssignDbProviderToReadDataSources_When_ReadsExist()
    {
        var configBuilder = new XmlConfigBuilder(ResourceType.File, "SmartSqlMapConfig-DeepTest.xml");

        var config = configBuilder.Build();

        foreach (var readDb in config.Database.Reads.Values)
        {
            readDb.DbProvider.Should().BeSameAs(config.Database.DbProvider);
        }
    }

    [Fact]
    public void Should_BeEmpty_When_NoReadDataSources()
    {
        var configBuilder = new XmlConfigBuilder(ResourceType.File, "SmartSqlMapConfig-UnitTest.xml");

        var config = configBuilder.Build();

        config.Database.Reads.Should().NotBeNull();
        config.Database.Reads.Should().BeEmpty();
    }

    #endregion

    #region SqlMaps Parsing

    [Fact]
    public void Should_ParseSqlMaps_When_SqlMapsExist()
    {
        var configBuilder = new XmlConfigBuilder(ResourceType.File, "SmartSqlMapConfig-DeepTest.xml");

        var config = configBuilder.Build();

        config.SqlMaps.Should().NotBeEmpty();
        config.SqlMaps.Should().ContainKey("TestScope");
        config.SqlMaps.Should().ContainKey("CacheScope");
    }

    [Fact]
    public void Should_BeEmpty_When_NoSqlMapsConfigured()
    {
        var configBuilder = new XmlConfigBuilder(ResourceType.File, "SmartSqlMapConfig-UnitTest.xml");

        var config = configBuilder.Build();

        config.SqlMaps.Should().NotBeNull();
        config.SqlMaps.Should().BeEmpty();
    }

    [Fact]
    public void Should_ParseStatements_When_SqlMapsHaveStatements()
    {
        var configBuilder = new XmlConfigBuilder(ResourceType.File, "SmartSqlMapConfig-DeepTest.xml");

        var config = configBuilder.Build();

        config.SqlMaps["TestScope"].Statements.Should().ContainKey("TestScope.GetAll");
        config.SqlMaps["TestScope"].Statements.Should().ContainKey("TestScope.GetById");
    }

    #endregion

    #region TypeHandlers Parsing

    [Fact]
    public void Should_HaveTypeHandlerFactory_When_Built()
    {
        var configBuilder = new XmlConfigBuilder(ResourceType.File, "SmartSqlMapConfig-DeepTest.xml");

        var config = configBuilder.Build();

        config.TypeHandlerFactory.Should().NotBeNull();
    }

    #endregion

    #region TagBuilders Parsing

    [Fact]
    public void Should_HaveTagBuilderFactory_When_Built()
    {
        var configBuilder = new XmlConfigBuilder(ResourceType.File, "SmartSqlMapConfig-DeepTest.xml");

        var config = configBuilder.Build();

        config.TagBuilderFactory.Should().NotBeNull();
    }

    #endregion

    #region Statement Analysis

    [Fact]
    public void Should_AnalyseStatementType_When_SqlMapsLoaded()
    {
        var configBuilder = new XmlConfigBuilder(ResourceType.File, "SmartSqlMapConfig-DeepTest.xml");

        var config = configBuilder.Build();

        var selectStatement = config.SqlMaps["TestScope"].Statements["TestScope.GetAll"];
        selectStatement.StatementType.Should().Be(StatementType.Select);
    }

    #endregion

    #region Exception Handling

    [Fact]
    public void Should_ThrowException_When_DbProviderNotConfigured()
    {
        var xml = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<SmartSqlMapConfig xmlns=""http://SmartSql.net/schemas/SmartSqlMapConfig.xsd"">
  <Database>
    <Write Name=""WriteDB"" ConnectionString=""Data Source=:memory:""/>
  </Database>
</SmartSqlMapConfig>";
        var tempPath = Path.Combine(Path.GetTempPath(), $"test_config_{Guid.NewGuid()}.xml");
        File.WriteAllText(tempPath, xml);

        try
        {
            var configBuilder = new XmlConfigBuilder(ResourceType.File, tempPath);

            var act = () => configBuilder.Build();

            act.Should().Throw<SmartSqlException>()
                .WithMessage("*DbProvider*");
        }
        finally
        {
            File.Delete(tempPath);
        }
    }

    [Fact]
    public void Should_ThrowException_When_WriteDataSourceNotConfigured()
    {
        var xml = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<SmartSqlMapConfig xmlns=""http://SmartSql.net/schemas/SmartSqlMapConfig.xsd"">
  <Database>
    <DbProvider Name=""SQLite""/>
  </Database>
</SmartSqlMapConfig>";
        var tempPath = Path.Combine(Path.GetTempPath(), $"test_config_{Guid.NewGuid()}.xml");
        File.WriteAllText(tempPath, xml);

        try
        {
            var configBuilder = new XmlConfigBuilder(ResourceType.File, tempPath);

            var act = () => configBuilder.Build();

            act.Should().Throw<SmartSqlException>()
                .WithMessage("*Write*");
        }
        finally
        {
            File.Delete(tempPath);
        }
    }

    #endregion

    #region EnsureDependency Tests

    [Fact]
    public void Should_ResolveResultMapReference_When_StatementHasResultMap()
    {
        var configBuilder = new XmlConfigBuilder(ResourceType.File, "SmartSqlMapConfig-DeepTest.xml");

        var config = configBuilder.Build();

        var statement = config.SqlMaps["CacheScope"].Statements["CacheScope.GetAll"];
        statement.ResultMap.Should().NotBeNull();
        statement.ResultMap.Id.Should().Be("CacheScope.UserResult");
    }

    [Fact]
    public void Should_ResolveParameterMapReference_When_StatementHasParameterMap()
    {
        var configBuilder = new XmlConfigBuilder(ResourceType.File, "SmartSqlMapConfig-DeepTest.xml");

        var config = configBuilder.Build();

        var statement = config.SqlMaps["CacheScope"].Statements["CacheScope.InsertUser"];
        statement.ParameterMap.Should().NotBeNull();
        statement.ParameterMap.Id.Should().Be("CacheScope.UserParameter");
    }

    [Fact]
    public void Should_ResolveCacheReference_When_StatementHasCache()
    {
        var configBuilder = new XmlConfigBuilder(ResourceType.File, "SmartSqlMapConfig-DeepTest.xml");

        var config = configBuilder.Build();

        var statement = config.SqlMaps["CacheScope"].Statements["CacheScope.GetAll"];
        statement.Cache.Should().NotBeNull();
    }

    [Fact]
    public void Should_ResolveMultipleResultMapReference_When_StatementHasMultipleResultMap()
    {
        var configBuilder = new XmlConfigBuilder(ResourceType.File, "SmartSqlMapConfig-DeepTest.xml");

        var config = configBuilder.Build();

        var mResultMap = config.SqlMaps["CacheScope"].MultipleResultMaps["CacheScope.UserMultipleResult"];
        mResultMap.Should().NotBeNull();
        mResultMap.Root.Should().NotBeNull();
        mResultMap.Root.MapId.Should().Be("CacheScope.UserResult");
        mResultMap.Root.Map.Should().NotBeNull();
    }

    #endregion
}
