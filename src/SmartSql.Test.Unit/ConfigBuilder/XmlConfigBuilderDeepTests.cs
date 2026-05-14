using FluentAssertions;
using SmartSql.ConfigBuilder;
using SmartSql.Configuration;
using SmartSql.Exceptions;
using Xunit;

namespace SmartSql.Test.Unit.ConfigBuilder;

public class XmlConfigBuilderDeepTests
{
    #region Settings

    [Fact]
    public void Should_ParseAllSettings_When_ConfigHasAllSettingsAttributes()
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
    public void Should_UseDefaultSettings_When_ConfigHasNoSettingsNode()
    {
        var configBuilder = new XmlConfigBuilder(ResourceType.File, "SmartSqlMapConfig-NoSettings.xml");

        var config = configBuilder.Build();

        config.Settings.Should().NotBeNull();
        config.Settings.IgnoreParameterCase.Should().BeFalse();
        config.Settings.IsCacheEnabled.Should().BeFalse();
    }

    [Fact]
    public void Should_ParseSettings_When_IgnoreParameterCaseIsTrue()
    {
        var configBuilder = new XmlConfigBuilder(ResourceType.File, "SmartSqlMapConfig-DeepTest.xml");

        var config = configBuilder.Build();

        config.Settings.IgnoreParameterCase.Should().BeTrue();
    }

    [Fact]
    public void Should_ParseSettings_When_IgnoreParameterCaseIsFalse()
    {
        var configBuilder = new XmlConfigBuilder(ResourceType.File, "SmartSqlMapConfig-UnitTest.xml");

        var config = configBuilder.Build();

        config.Settings.IgnoreParameterCase.Should().BeFalse();
    }

    #endregion

    #region Properties

    [Fact]
    public void Should_ParseProperties_When_MultiplePropertiesDefined()
    {
        var configBuilder = new XmlConfigBuilder(ResourceType.File, "SmartSqlMapConfig-DeepTest.xml");

        var config = configBuilder.Build();

        config.Properties.Should().NotBeNull();
        var dbProvider = config.Properties.GetPropertyValue("${DbProvider}");
        dbProvider.Should().Be("SQLite");
        var connStr = config.Properties.GetPropertyValue("${ConnectionString}");
        connStr.Should().Be("Data Source=:memory:");
        var readConnStr = config.Properties.GetPropertyValue("${ReadConnStr}");
        readConnStr.Should().Be("Data Source=:memory:Read");
    }

    [Fact]
    public void Should_ResolvePropertyReferences_When_UsedInConnectionString()
    {
        var configBuilder = new XmlConfigBuilder(ResourceType.File, "SmartSqlMapConfig-DeepTest.xml");

        var config = configBuilder.Build();

        config.Database.Write.ConnectionString.Should().Be("Data Source=:memory:");
    }

    [Fact]
    public void Should_ResolvePropertyReferences_When_UsedInDbProvider()
    {
        var configBuilder = new XmlConfigBuilder(ResourceType.File, "SmartSqlMapConfig-DeepTest.xml");

        var config = configBuilder.Build();

        config.Database.DbProvider.Name.Should().Be("SQLite");
    }

    #endregion

    #region Database

    [Fact]
    public void Should_ParseDatabase_When_WriteDataSource()
    {
        var configBuilder = new XmlConfigBuilder(ResourceType.File, "SmartSqlMapConfig-DeepTest.xml");

        var config = configBuilder.Build();

        config.Database.Should().NotBeNull();
        config.Database.Write.Should().NotBeNull();
        config.Database.Write.Name.Should().Be("WriteDB");
        config.Database.Write.ConnectionString.Should().Be("Data Source=:memory:");
    }

    [Fact]
    public void Should_ParseDatabase_When_MultipleReadDataSources()
    {
        var configBuilder = new XmlConfigBuilder(ResourceType.File, "SmartSqlMapConfig-MultipleRead.xml");

        var config = configBuilder.Build();

        config.Database.Reads.Should().NotBeNull();
        config.Database.Reads.Count.Should().Be(3);
        config.Database.Reads.Should().ContainKey("ReadDb-1");
        config.Database.Reads.Should().ContainKey("ReadDb-2");
        config.Database.Reads.Should().ContainKey("ReadDb-3");
    }

    [Fact]
    public void Should_ParseReadDataSourceWeight_When_WeightSpecified()
    {
        var configBuilder = new XmlConfigBuilder(ResourceType.File, "SmartSqlMapConfig-MultipleRead.xml");

        var config = configBuilder.Build();

        config.Database.Reads["ReadDb-1"].Weight.Should().Be(100);
        config.Database.Reads["ReadDb-2"].Weight.Should().Be(50);
        config.Database.Reads["ReadDb-3"].Weight.Should().Be(200);
    }

    [Fact]
    public void Should_ParseReadDataSourceConnectionString_When_PropertyReferenced()
    {
        var configBuilder = new XmlConfigBuilder(ResourceType.File, "SmartSqlMapConfig-MultipleRead.xml");

        var config = configBuilder.Build();

        config.Database.Reads["ReadDb-1"].ConnectionString.Should().Be("Data Source=:memory:R1");
        config.Database.Reads["ReadDb-2"].ConnectionString.Should().Be("Data Source=:memory:R2");
        config.Database.Reads["ReadDb-3"].ConnectionString.Should().Be("Data Source=:memory:R3");
    }

    [Fact]
    public void Should_ParseDbProvider_When_PropertyReferenced()
    {
        var configBuilder = new XmlConfigBuilder(ResourceType.File, "SmartSqlMapConfig-DeepTest.xml");

        var config = configBuilder.Build();

        config.Database.DbProvider.Should().NotBeNull();
        config.Database.DbProvider.Name.Should().Be("SQLite");
    }

    [Fact]
    public void Should_ParseReadDataSource_When_TwoReadDbs()
    {
        var configBuilder = new XmlConfigBuilder(ResourceType.File, "SmartSqlMapConfig-DeepTest.xml");

        var config = configBuilder.Build();

        config.Database.Reads.Count.Should().Be(2);
        config.Database.Reads["ReadDb-1"].Weight.Should().Be(100);
        config.Database.Reads["ReadDb-2"].Weight.Should().Be(50);
    }

    [Fact]
    public void Should_AssignDbProviderToReadDataSources_When_ReadDbsConfigured()
    {
        var configBuilder = new XmlConfigBuilder(ResourceType.File, "SmartSqlMapConfig-DeepTest.xml");

        var config = configBuilder.Build();

        foreach (var readDb in config.Database.Reads.Values)
        {
            readDb.DbProvider.Should().NotBeNull();
            readDb.DbProvider.Name.Should().Be("SQLite");
        }
    }

    #endregion

    #region IdGenerators

    [Fact]
    public void Should_ParseIdGenerators_When_ConfigHasGenerators()
    {
        var configBuilder = new XmlConfigBuilder(ResourceType.File, "SmartSqlMapConfig-DeepTest.xml");

        var config = configBuilder.Build();

        config.IdGenerators.Should().NotBeNull();
        config.IdGenerators.Should().ContainKey("SnowflakeId");
    }

    [Fact]
    public void Should_ParseIdGeneratorProperties_When_GeneratorHasProperties()
    {
        var configBuilder = new XmlConfigBuilder(ResourceType.File, "SmartSqlMapConfig-DeepTest.xml");

        var config = configBuilder.Build();

        config.IdGenerators["SnowflakeId"].Should().NotBeNull();
    }

    [Fact]
    public void Should_ParseEmptyIdGenerators_When_ConfigHasNoGenerators()
    {
        var configBuilder = new XmlConfigBuilder(ResourceType.File, "SmartSqlMapConfig-UnitTest.xml");

        var config = configBuilder.Build();

        config.IdGenerators.Should().NotBeNull();
        config.IdGenerators.Should().BeEmpty();
    }

    #endregion

    #region SqlMaps

    [Fact]
    public void Should_ParseMultipleSqlMaps_When_ConfigHasMultipleMaps()
    {
        var configBuilder = new XmlConfigBuilder(ResourceType.File, "SmartSqlMapConfig-DeepTest.xml");

        var config = configBuilder.Build();

        config.SqlMaps.Should().NotBeNull();
        config.SqlMaps.Should().ContainKey("TestScope");
        config.SqlMaps.Should().ContainKey("CacheScope");
    }

    [Fact]
    public void Should_ParseSqlMapStatements_When_SqlMapLoaded()
    {
        var configBuilder = new XmlConfigBuilder(ResourceType.File, "SmartSqlMapConfig-DeepTest.xml");

        var config = configBuilder.Build();

        config.SqlMaps["TestScope"].Statements.Should().ContainKey("TestScope.GetAll");
        config.SqlMaps["TestScope"].Statements.Should().ContainKey("TestScope.GetById");
    }

    [Fact]
    public void Should_ParseCacheSqlMap_When_SqlMapWithCacheLoaded()
    {
        var configBuilder = new XmlConfigBuilder(ResourceType.File, "SmartSqlMapConfig-DeepTest.xml");

        var config = configBuilder.Build();

        config.SqlMaps["CacheScope"].Should().NotBeNull();
        config.SqlMaps["CacheScope"].Statements.Should().ContainKey("CacheScope.GetAll");
        config.SqlMaps["CacheScope"].Statements.Should().ContainKey("CacheScope.InsertUser");
    }

    [Fact]
    public void Should_ParseResultMaps_When_SqlMapHasResultMaps()
    {
        var configBuilder = new XmlConfigBuilder(ResourceType.File, "SmartSqlMapConfig-DeepTest.xml");

        var config = configBuilder.Build();

        config.SqlMaps["CacheScope"].ResultMaps.Should().ContainKey("CacheScope.UserResult");
    }

    [Fact]
    public void Should_ParseParameterMaps_When_SqlMapHasParameterMaps()
    {
        var configBuilder = new XmlConfigBuilder(ResourceType.File, "SmartSqlMapConfig-DeepTest.xml");

        var config = configBuilder.Build();

        config.SqlMaps["CacheScope"].ParameterMaps.Should().ContainKey("CacheScope.UserParameter");
    }

    [Fact]
    public void Should_ParseMultipleResultMaps_When_SqlMapHasMultipleResultMaps()
    {
        var configBuilder = new XmlConfigBuilder(ResourceType.File, "SmartSqlMapConfig-DeepTest.xml");

        var config = configBuilder.Build();

        config.SqlMaps["CacheScope"].MultipleResultMaps.Should().ContainKey("CacheScope.UserMultipleResult");
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
    public void Should_ResolveResultMapReference_When_StatementHasResultMap()
    {
        var configBuilder = new XmlConfigBuilder(ResourceType.File, "SmartSqlMapConfig-DeepTest.xml");

        var config = configBuilder.Build();

        var statement = config.SqlMaps["CacheScope"].Statements["CacheScope.GetAll"];
        statement.ResultMap.Should().NotBeNull();
    }

    [Fact]
    public void Should_ResolveParameterMapReference_When_StatementHasParameterMap()
    {
        var configBuilder = new XmlConfigBuilder(ResourceType.File, "SmartSqlMapConfig-DeepTest.xml");

        var config = configBuilder.Build();

        var statement = config.SqlMaps["CacheScope"].Statements["CacheScope.InsertUser"];
        statement.ParameterMap.Should().NotBeNull();
    }

    [Fact]
    public void Should_ResolveMultipleResultMapReference_When_StatementHasMultipleResultMap()
    {
        var configBuilder = new XmlConfigBuilder(ResourceType.File, "SmartSqlMapConfig-DeepTest.xml");

        var config = configBuilder.Build();

        // The CacheScope SqlMap defines a MultipleResultMap
        var mResultMap = config.SqlMaps["CacheScope"].MultipleResultMaps["CacheScope.UserMultipleResult"];
        mResultMap.Should().NotBeNull();
        mResultMap.Root.Should().NotBeNull();
        mResultMap.Root.MapId.Should().Be("CacheScope.UserResult");
        mResultMap.Root.Map.Should().NotBeNull();
    }

    #endregion

    #region AbstractConfigBuilder - Template Method Pattern

    [Fact]
    public void Should_SetInitialized_When_BuildCompleted()
    {
        var configBuilder = new XmlConfigBuilder(ResourceType.File, "SmartSqlMapConfig-DeepTest.xml");

        configBuilder.Build();

        configBuilder.Initialized.Should().BeTrue();
    }

    [Fact]
    public void Should_ReturnSameInstance_When_BuildCalledMultipleTimes()
    {
        var configBuilder = new XmlConfigBuilder(ResourceType.File, "SmartSqlMapConfig-DeepTest.xml");

        var first = configBuilder.Build();
        var second = configBuilder.Build();

        second.Should().BeSameAs(first);
    }

    [Fact]
    public void Should_HaveNoParent_When_NoParentSet()
    {
        var configBuilder = new XmlConfigBuilder(ResourceType.File, "SmartSqlMapConfig-UnitTest.xml");

        var config = configBuilder.Build();

        configBuilder.Parent.Should().BeNull();
    }

    [Fact]
    public void Should_ThrowException_When_SetParentToSelf()
    {
        var configBuilder = new XmlConfigBuilder(ResourceType.File, "SmartSqlMapConfig-UnitTest.xml");

        var act = () => configBuilder.SetParent(configBuilder);

        act.Should().Throw<SmartSqlException>()
            .WithMessage("*can't be self*");
    }

    #endregion

    #region Statement Analysis

    [Fact]
    public void Should_AnalyseStatementType_When_SelectStatement()
    {
        var configBuilder = new XmlConfigBuilder(ResourceType.File, "SmartSqlMapConfig-DeepTest.xml");

        var config = configBuilder.Build();

        var statement = config.SqlMaps["TestScope"].Statements["TestScope.GetAll"];
        statement.StatementType.Should().Be(StatementType.Select);
    }

    #endregion

    #region TypeHandlers

    [Fact]
    public void Should_HaveTypeHandlerFactory_When_ConfigBuilt()
    {
        var configBuilder = new XmlConfigBuilder(ResourceType.File, "SmartSqlMapConfig-DeepTest.xml");

        var config = configBuilder.Build();

        config.TypeHandlerFactory.Should().NotBeNull();
    }

    #endregion

    #region Complete Config

    [Fact]
    public void Should_BuildCompleteConfig_When_FullConfigProvided()
    {
        var configBuilder = new XmlConfigBuilder(ResourceType.File, "SmartSqlMapConfig-DeepTest.xml");

        var config = configBuilder.Build();

        config.Should().NotBeNull();
        config.Settings.Should().NotBeNull();
        config.Properties.Should().NotBeNull();
        config.Database.Should().NotBeNull();
        config.Database.Write.Should().NotBeNull();
        config.Database.Reads.Should().NotBeEmpty();
        config.Database.DbProvider.Should().NotBeNull();
        config.SqlMaps.Should().NotBeEmpty();
        config.TypeHandlerFactory.Should().NotBeNull();
        config.IdGenerators.Should().NotBeEmpty();
        config.StatementAnalyzer.Should().NotBeNull();
        config.TagBuilderFactory.Should().NotBeNull();
    }

    [Fact]
    public void Should_HaveCorrectScopeNames_When_MultipleMapsLoaded()
    {
        var configBuilder = new XmlConfigBuilder(ResourceType.File, "SmartSqlMapConfig-DeepTest.xml");

        var config = configBuilder.Build();

        config.SqlMaps.Keys.Should().Contain(new[] { "TestScope", "CacheScope" });
    }

    [Fact]
    public void Should_ParseDynamicTagSqlMap_When_SeparateConfigReferencesIt()
    {
        var configBuilder = new XmlConfigBuilder(ResourceType.File, "SmartSqlMapConfig-DeepTest.xml");

        var config = configBuilder.Build();

        // DynamicTag SqlMap is not included in the DeepTest config, only Simple and CacheAndMaps
        config.SqlMaps.Should().HaveCount(2);
    }

    #endregion

    #region Config with no read data sources

    [Fact]
    public void Should_BuildConfigWithNoReadDbs_When_NoReadNodes()
    {
        var configBuilder = new XmlConfigBuilder(ResourceType.File, "SmartSqlMapConfig-UnitTest.xml");

        var config = configBuilder.Build();

        config.Database.Reads.Should().NotBeNull();
        config.Database.Reads.Should().BeEmpty();
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

    #endregion
}
