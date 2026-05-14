using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using SmartSql.ConfigBuilder;
using SmartSql.Configuration;
using SmartSql.CUD;
using SmartSql.DataSource;
using SmartSql.Test.Unit.TestEntities;
using Xunit;
using CacheConfig = SmartSql.Configuration.Cache;

namespace SmartSql.Test.Unit.CUD;

public class CUDConfigBuilderTests
{
    private static SmartSqlConfig CreateTestSmartSqlConfig()
    {
        return new SmartSqlConfig
        {
            Database = new Database
            {
                DbProvider = DbProviderManager.MYSQL_DBPROVIDER
            },
            SqlMaps = new Dictionary<string, SqlMap>()
        };
    }

    private class StubConfigBuilder : IConfigBuilder
    {
        private readonly SmartSqlConfig _config;

        public StubConfigBuilder(SmartSqlConfig config)
        {
            _config = config;
        }

        public bool Initialized { get; private set; }
        public SmartSqlConfig SmartSqlConfig => _config;

        public IConfigBuilder Parent => throw new NotImplementedException();

        public SmartSqlConfig Build()
        {
            Initialized = true;
            return _config;
        }

        public void SetParent(IConfigBuilder configBuilder)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
        }
    }

    [Fact]
    public void Should_GenerateCUDStatements_When_EntityRegistered()
    {
        // Arrange
        var smartSqlConfig = CreateTestSmartSqlConfig();
        var parentBuilder = new StubConfigBuilder(smartSqlConfig);
        var entityTypes = new List<Type> { typeof(AllPrimitive) };
        var builder = new CUDConfigBuilder(parentBuilder, entityTypes);

        // Act
        var result = builder.Build();

        // Assert
        result.SqlMaps.Should().ContainKey("CUD_AllPrimitive");
        var sqlMap = result.SqlMaps["CUD_AllPrimitive"];
        sqlMap.Statements.Should().ContainKey("CUD_AllPrimitive.GetById");
        sqlMap.Statements.Should().ContainKey("CUD_AllPrimitive.Insert");
        sqlMap.Statements.Should().ContainKey("CUD_AllPrimitive.InsertReturnId");
        sqlMap.Statements.Should().ContainKey("CUD_AllPrimitive.Update");
        sqlMap.Statements.Should().ContainKey("CUD_AllPrimitive.DeleteById");
        sqlMap.Statements.Should().ContainKey("CUD_AllPrimitive.DeleteAll");
        sqlMap.Statements.Should().ContainKey("CUD_AllPrimitive.DeleteMany");
    }

    [Fact]
    public void Should_CreateNewSqlMap_When_ScopeDoesNotExist()
    {
        // Arrange
        var smartSqlConfig = CreateTestSmartSqlConfig();
        var parentBuilder = new StubConfigBuilder(smartSqlConfig);
        var entityTypes = new List<Type> { typeof(AllPrimitive) };
        var builder = new CUDConfigBuilder(parentBuilder, entityTypes);

        // Act
        var result = builder.Build();

        // Assert
        result.SqlMaps.Should().ContainKey("CUD_AllPrimitive");
        var sqlMap = result.SqlMaps["CUD_AllPrimitive"];
        sqlMap.Scope.Should().Be("CUD_AllPrimitive");
    }

    [Fact]
    public void Should_UseExistingSqlMap_When_ScopeAlreadyExists()
    {
        // Arrange
        var smartSqlConfig = CreateTestSmartSqlConfig();
        var existingSqlMap = new SqlMap
        {
            Scope = "CUD_AllPrimitive",
            Statements = new Dictionary<string, Statement>(),
            Caches = new Dictionary<string, CacheConfig>(),
            MultipleResultMaps = new Dictionary<string, MultipleResultMap>(),
            ParameterMaps = new Dictionary<string, ParameterMap>(),
            ResultMaps = new Dictionary<string, ResultMap>()
        };
        smartSqlConfig.SqlMaps.Add("CUD_AllPrimitive", existingSqlMap);

        var parentBuilder = new StubConfigBuilder(smartSqlConfig);
        var entityTypes = new List<Type> { typeof(AllPrimitive) };
        var builder = new CUDConfigBuilder(parentBuilder, entityTypes);

        // Act
        var result = builder.Build();

        // Assert
        result.SqlMaps["CUD_AllPrimitive"].Should().BeSameAs(existingSqlMap);
        existingSqlMap.Statements.Should().ContainKey("CUD_AllPrimitive.GetById");
    }

    [Fact]
    public void Should_SkipExistingStatements_When_SqlMapAlreadyContainsStatement()
    {
        // Arrange
        var smartSqlConfig = CreateTestSmartSqlConfig();
        var existingStatement = new Statement { Id = "GetById" };
        var existingSqlMap = new SqlMap
        {
            Scope = "CUD_AllPrimitive",
            Statements = new Dictionary<string, Statement>
            {
                { "CUD_AllPrimitive.GetById", existingStatement }
            },
            Caches = new Dictionary<string, CacheConfig>(),
            MultipleResultMaps = new Dictionary<string, MultipleResultMap>(),
            ParameterMaps = new Dictionary<string, ParameterMap>(),
            ResultMaps = new Dictionary<string, ResultMap>()
        };
        smartSqlConfig.SqlMaps.Add("CUD_AllPrimitive", existingSqlMap);

        var parentBuilder = new StubConfigBuilder(smartSqlConfig);
        var entityTypes = new List<Type> { typeof(AllPrimitive) };
        var builder = new CUDConfigBuilder(parentBuilder, entityTypes);

        // Act
        var result = builder.Build();

        // Assert
        result.SqlMaps["CUD_AllPrimitive"].Statements["CUD_AllPrimitive.GetById"].Should().BeSameAs(existingStatement);
        // Other statements should still be added
        result.SqlMaps["CUD_AllPrimitive"].Statements.Should().ContainKey("CUD_AllPrimitive.Insert");
    }

    [Fact]
    public void Should_HandleMultipleEntities_When_MultipleTypesRegistered()
    {
        // Arrange
        var smartSqlConfig = CreateTestSmartSqlConfig();
        var parentBuilder = new StubConfigBuilder(smartSqlConfig);
        var entityTypes = new List<Type> { typeof(AllPrimitive) };
        var builder = new CUDConfigBuilder(parentBuilder, entityTypes);

        // Act
        var result = builder.Build();

        // Assert
        result.SqlMaps["CUD_AllPrimitive"].Statements.Should().HaveCount(7);
    }

    [Fact]
    public void Should_ReturnSameConfig_When_BuildCalledTwice()
    {
        // Arrange
        var smartSqlConfig = CreateTestSmartSqlConfig();
        var parentBuilder = new StubConfigBuilder(smartSqlConfig);
        var entityTypes = new List<Type> { typeof(AllPrimitive) };
        var builder = new CUDConfigBuilder(parentBuilder, entityTypes);

        // Act
        var firstResult = builder.Build();
        var secondResult = builder.Build();

        // Assert
        firstResult.Should().BeSameAs(secondResult);
        builder.Initialized.Should().BeTrue();
    }

    [Fact]
    public void Should_SetInitializedToTrue_When_BuildCalled()
    {
        // Arrange
        var smartSqlConfig = CreateTestSmartSqlConfig();
        var parentBuilder = new StubConfigBuilder(smartSqlConfig);
        var entityTypes = new List<Type> { typeof(AllPrimitive) };
        var builder = new CUDConfigBuilder(parentBuilder, entityTypes);

        // Act & Assert
        builder.Initialized.Should().BeFalse();
        builder.Build();
        builder.Initialized.Should().BeTrue();
    }

    [Fact]
    public void Should_SetSqlMapPath_When_NewSqlMapCreated()
    {
        // Arrange
        var smartSqlConfig = CreateTestSmartSqlConfig();
        var parentBuilder = new StubConfigBuilder(smartSqlConfig);
        var entityTypes = new List<Type> { typeof(AllPrimitive) };
        var builder = new CUDConfigBuilder(parentBuilder, entityTypes);

        // Act
        var result = builder.Build();

        // Assert
        var sqlMap = result.SqlMaps["CUD_AllPrimitive"];
        sqlMap.Path.Should().Be(typeof(AllPrimitive).AssemblyQualifiedName);
    }

    [Fact]
    public void Should_SetSmartSqlConfigOnSqlMap_When_NewSqlMapCreated()
    {
        // Arrange
        var smartSqlConfig = CreateTestSmartSqlConfig();
        var parentBuilder = new StubConfigBuilder(smartSqlConfig);
        var entityTypes = new List<Type> { typeof(AllPrimitive) };
        var builder = new CUDConfigBuilder(parentBuilder, entityTypes);

        // Act
        var result = builder.Build();

        // Assert
        var sqlMap = result.SqlMaps["CUD_AllPrimitive"];
        sqlMap.SmartSqlConfig.Should().BeSameAs(smartSqlConfig);
    }

    [Fact]
    public void Should_HandleEmptyEntityTypeList_When_NoEntitiesRegistered()
    {
        // Arrange
        var smartSqlConfig = CreateTestSmartSqlConfig();
        var parentBuilder = new StubConfigBuilder(smartSqlConfig);
        var entityTypes = new List<Type>();
        var builder = new CUDConfigBuilder(parentBuilder, entityTypes);

        // Act
        var result = builder.Build();

        // Assert
        result.SqlMaps.Should().BeEmpty();
    }
}
