using System;
using System.Collections.Generic;
using FluentAssertions;
using SmartSql.AutoConverter;
using SmartSql.Configuration;
using SmartSql.Deserializer;
using SmartSql.Exceptions;
using SmartSql.IdGenerator;
using SmartSql.Reflection.ObjectFactoryBuilder;
using SmartSql.Utils;
using Xunit;
using CacheCfg = SmartSql.Configuration.Cache;
using DatabaseCfg = SmartSql.DataSource.Database;

namespace SmartSql.Test.Unit.Configuration;

public class SmartSqlConfigTests
{
    #region Constructor

    [Fact]
    public void Should_InitializeSettings_When_Created()
    {
        var config = new SmartSqlConfig();

        config.Settings.Should().NotBeNull();
        config.Settings.ParameterPrefix.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void Should_InitializeSqlMaps_When_Created()
    {
        var config = new SmartSqlConfig();

        config.SqlMaps.Should().NotBeNull();
        config.SqlMaps.Should().BeEmpty();
    }

    [Fact]
    public void Should_InitializeFilters_When_Created()
    {
        var config = new SmartSqlConfig();

        config.Filters.Should().NotBeNull();
    }

    [Fact]
    public void Should_InitializeObjectFactoryBuilder_When_Created()
    {
        var config = new SmartSqlConfig();

        config.ObjectFactoryBuilder.Should().NotBeNull();
        config.ObjectFactoryBuilder.Should().BeOfType<ExpressionObjectFactoryBuilder>();
    }

    [Fact]
    public void Should_InitializeTagBuilderFactory_When_Created()
    {
        var config = new SmartSqlConfig();

        config.TagBuilderFactory.Should().NotBeNull();
    }

    [Fact]
    public void Should_InitializeTypeHandlerFactory_When_Created()
    {
        var config = new SmartSqlConfig();

        config.TypeHandlerFactory.Should().NotBeNull();
    }

    [Fact]
    public void Should_InitializeLoggerFactory_When_Created()
    {
        var config = new SmartSqlConfig();

        config.LoggerFactory.Should().NotBeNull();
    }

    [Fact]
    public void Should_InitializeDeserializerFactory_When_Created()
    {
        var config = new SmartSqlConfig();

        config.DeserializerFactory.Should().NotBeNull();
        config.DeserializerFactory.Should().BeOfType<DeserializerFactory>();
    }

    [Fact]
    public void Should_InitializeProperties_When_Created()
    {
        var config = new SmartSqlConfig();

        config.Properties.Should().NotBeNull();
    }

    [Fact]
    public void Should_InitializeIdGenerators_When_Created()
    {
        var config = new SmartSqlConfig();

        config.IdGenerators.Should().NotBeNull();
    }

    [Fact]
    public void Should_InitializeAutoConverters_When_Created()
    {
        var config = new SmartSqlConfig();

        config.AutoConverters.Should().NotBeNull();
        config.AutoConverters.Should().BeEmpty();
    }

    [Fact]
    public void Should_InitializeDefaultAutoConverter_When_Created()
    {
        var config = new SmartSqlConfig();

        config.DefaultAutoConverter.Should().NotBeNull();
        config.DefaultAutoConverter.Should().BeSameAs(NoneAutoConverter.INSTANCE);
    }

    [Fact]
    public void Should_InitializeDbSessionFactory_When_Created()
    {
        var config = new SmartSqlConfig();

        config.DbSessionFactory.Should().NotBeNull();
    }

    [Fact]
    public void Should_InitializeSessionStore_When_Created()
    {
        var config = new SmartSqlConfig();

        config.SessionStore.Should().NotBeNull();
    }

    [Fact]
    public void Should_InitializeStatementAnalyzer_When_Created()
    {
        var config = new SmartSqlConfig();

        config.StatementAnalyzer.Should().NotBeNull();
    }

    [Fact]
    public void Should_InitializeInvokeSucceedListener_When_Created()
    {
        var config = new SmartSqlConfig();

        config.InvokeSucceedListener.Should().NotBeNull();
    }

    #endregion

    #region Property Setters

    [Fact]
    public void Should_SetAlias_When_Assigned()
    {
        var config = new SmartSqlConfig();

        config.Alias = "TestAlias";

        config.Alias.Should().Be("TestAlias");
    }

    [Fact]
    public void Should_SetDatabase_When_Assigned()
    {
        var config = new SmartSqlConfig();
        var database = new DatabaseCfg();

        config.Database = database;

        config.Database.Should().BeSameAs(database);
    }

    [Fact]
    public void Should_SetPipeline_When_Assigned()
    {
        var config = new SmartSqlConfig();
        var mockPipeline = new MockMiddleware();

        config.Pipeline = mockPipeline;

        config.Pipeline.Should().BeSameAs(mockPipeline);
    }

    [Fact]
    public void Should_SetCacheManager_When_Assigned()
    {
        var config = new SmartSqlConfig();

        config.CacheManager = null;

        config.CacheManager.Should().BeNull();
    }

    [Fact]
    public void Should_SetCommandExecuter_When_Assigned()
    {
        var config = new SmartSqlConfig();

        config.CommandExecuter = null;

        config.CommandExecuter.Should().BeNull();
    }

    #endregion

    #region GetSqlMap

    [Fact]
    public void Should_ReturnSqlMap_When_ScopeExists()
    {
        var config = new SmartSqlConfig();
        var sqlMap = new SqlMap { Scope = "TestScope" };
        config.SqlMaps["TestScope"] = sqlMap;

        var result = config.GetSqlMap("TestScope");

        result.Should().BeSameAs(sqlMap);
    }

    [Fact]
    public void Should_ThrowException_When_ScopeDoesNotExist()
    {
        var config = new SmartSqlConfig();

        var act = () => config.GetSqlMap("NonExistentScope");

        act.Should().Throw<SmartSqlException>()
            .WithMessage("*SqlMap.Scope:NonExistentScope*");
    }

    #endregion

    #region GetStatement

    [Fact]
    public void Should_ReturnStatement_When_StatementExists()
    {
        var config = new SmartSqlConfig();
        var sqlMap = new SqlMap
        {
            Scope = "TestScope",
            Statements = new Dictionary<string, Statement>(StringComparer.OrdinalIgnoreCase)
        };
        var statement = new Statement
        {
            Id = "GetById",
            SqlMap = sqlMap,
            StatementType = StatementType.Select
        };
        sqlMap.Statements["TestScope.GetById"] = statement;
        config.SqlMaps["TestScope"] = sqlMap;

        var result = config.GetStatement("TestScope.GetById");

        result.Should().BeSameAs(statement);
    }

    [Fact]
    public void Should_ThrowException_When_StatementDoesNotExist()
    {
        var config = new SmartSqlConfig();
        var sqlMap = new SqlMap
        {
            Scope = "TestScope",
            Statements = new Dictionary<string, Statement>(StringComparer.OrdinalIgnoreCase)
        };
        config.SqlMaps["TestScope"] = sqlMap;

        var act = () => config.GetStatement("TestScope.NonExistent");

        act.Should().Throw<SmartSqlException>()
            .WithMessage("*Statement.FullSqlId:TestScope.NonExistent*");
    }

    #endregion

    #region GetCache

    [Fact]
    public void Should_ReturnCache_When_CacheExists()
    {
        var config = new SmartSqlConfig();
        var sqlMap = new SqlMap
        {
            Scope = "TestScope",
            Caches = new Dictionary<string, CacheCfg>(StringComparer.OrdinalIgnoreCase)
        };
        var cache = new CacheCfg { Id = "TestScope.TestCache" };
        sqlMap.Caches["TestScope.TestCache"] = cache;
        config.SqlMaps["TestScope"] = sqlMap;

        var result = config.GetCache("TestScope.TestCache");

        result.Should().BeSameAs(cache);
    }

    [Fact]
    public void Should_ThrowException_When_CacheDoesNotExist()
    {
        var config = new SmartSqlConfig();
        var sqlMap = new SqlMap
        {
            Scope = "TestScope",
            Caches = new Dictionary<string, CacheCfg>(StringComparer.OrdinalIgnoreCase)
        };
        config.SqlMaps["TestScope"] = sqlMap;

        var act = () => config.GetCache("TestScope.NonExistentCache");

        act.Should().Throw<SmartSqlException>()
            .WithMessage("*Cache.Id:TestScope.NonExistentCache*");
    }

    #endregion

    #region GetResultMap

    [Fact]
    public void Should_ReturnResultMap_When_ResultMapExists()
    {
        var config = new SmartSqlConfig();
        var sqlMap = new SqlMap
        {
            Scope = "TestScope",
            ResultMaps = new Dictionary<string, ResultMap>(StringComparer.OrdinalIgnoreCase)
        };
        var resultMap = new ResultMap { Id = "TestScope.TestResult" };
        sqlMap.ResultMaps["TestScope.TestResult"] = resultMap;
        config.SqlMaps["TestScope"] = sqlMap;

        var result = config.GetResultMap("TestScope.TestResult");

        result.Should().BeSameAs(resultMap);
    }

    [Fact]
    public void Should_ThrowException_When_ResultMapDoesNotExist()
    {
        var config = new SmartSqlConfig();
        var sqlMap = new SqlMap
        {
            Scope = "TestScope",
            ResultMaps = new Dictionary<string, ResultMap>(StringComparer.OrdinalIgnoreCase)
        };
        config.SqlMaps["TestScope"] = sqlMap;

        var act = () => config.GetResultMap("TestScope.NonExistentResult");

        act.Should().Throw<SmartSqlException>()
            .WithMessage("*ResultMap.Id:TestScope.NonExistentResult*");
    }

    #endregion

    #region GetParameterMap

    [Fact]
    public void Should_ReturnParameterMap_When_ParameterMapExists()
    {
        var config = new SmartSqlConfig();
        var sqlMap = new SqlMap
        {
            Scope = "TestScope",
            ParameterMaps = new Dictionary<string, ParameterMap>(StringComparer.OrdinalIgnoreCase)
        };
        var parameterMap = new ParameterMap { Id = "TestScope.TestParam" };
        sqlMap.ParameterMaps["TestScope.TestParam"] = parameterMap;
        config.SqlMaps["TestScope"] = sqlMap;

        var result = config.GetParameterMap("TestScope.TestParam");

        result.Should().BeSameAs(parameterMap);
    }

    [Fact]
    public void Should_ThrowException_When_ParameterMapDoesNotExist()
    {
        var config = new SmartSqlConfig();
        var sqlMap = new SqlMap
        {
            Scope = "TestScope",
            ParameterMaps = new Dictionary<string, ParameterMap>(StringComparer.OrdinalIgnoreCase)
        };
        config.SqlMaps["TestScope"] = sqlMap;

        var act = () => config.GetParameterMap("TestScope.NonExistentParam");

        act.Should().Throw<SmartSqlException>()
            .WithMessage("*ParameterMap.Id:TestScope.NonExistentParam*");
    }

    #endregion

    #region GetMultipleResultMap

    [Fact]
    public void Should_ReturnMultipleResultMap_When_MultipleResultMapExists()
    {
        var config = new SmartSqlConfig();
        var sqlMap = new SqlMap
        {
            Scope = "TestScope",
            MultipleResultMaps = new Dictionary<string, MultipleResultMap>(StringComparer.OrdinalIgnoreCase)
        };
        var multipleResultMap = new MultipleResultMap { Id = "TestScope.TestMultiple" };
        sqlMap.MultipleResultMaps["TestScope.TestMultiple"] = multipleResultMap;
        config.SqlMaps["TestScope"] = sqlMap;

        var result = config.GetMultipleResultMap("TestScope.TestMultiple");

        result.Should().BeSameAs(multipleResultMap);
    }

    [Fact]
    public void Should_ThrowException_When_MultipleResultMapDoesNotExist()
    {
        var config = new SmartSqlConfig();
        var sqlMap = new SqlMap
        {
            Scope = "TestScope",
            MultipleResultMaps = new Dictionary<string, MultipleResultMap>(StringComparer.OrdinalIgnoreCase)
        };
        config.SqlMaps["TestScope"] = sqlMap;

        var act = () => config.GetMultipleResultMap("TestScope.NonExistentMultiple");

        act.Should().Throw<SmartSqlException>()
            .WithMessage("*MultipleResultMap.Id:TestScope.NonExistentMultiple*");
    }

    #endregion

    #region Multiple Scope Support

    [Fact]
    public void Should_SupportMultipleScopes_When_MultipleSqlMapsAdded()
    {
        var config = new SmartSqlConfig();
        var sqlMap1 = new SqlMap { Scope = "Scope1" };
        var sqlMap2 = new SqlMap { Scope = "Scope2" };
        config.SqlMaps["Scope1"] = sqlMap1;
        config.SqlMaps["Scope2"] = sqlMap2;

        config.SqlMaps.Should().HaveCount(2);
        config.GetSqlMap("Scope1").Should().BeSameAs(sqlMap1);
        config.GetSqlMap("Scope2").Should().BeSameAs(sqlMap2);
    }

    #endregion

    #region Settings Tests

    [Fact]
    public void Should_HaveDefaultValues_When_Created()
    {
        var settings = new Settings();

        settings.ParameterPrefix.Should().BeNull();
    }

    [Fact]
    public void Should_SetIgnoreParameterCase_When_Assigned()
    {
        var settings = new Settings();

        settings.IgnoreParameterCase = true;

        settings.IgnoreParameterCase.Should().BeTrue();
    }

    [Fact]
    public void Should_SetIsCacheEnabled_When_Assigned()
    {
        var settings = new Settings();

        settings.IsCacheEnabled = true;

        settings.IsCacheEnabled.Should().BeTrue();
    }

    [Fact]
    public void Should_SetParameterPrefix_When_Assigned()
    {
        var settings = new Settings();

        settings.ParameterPrefix = "@";

        settings.ParameterPrefix.Should().Be("@");
    }

    [Fact]
    public void Should_SetEnablePropertyChangedTrack_When_Assigned()
    {
        var settings = new Settings();

        settings.EnablePropertyChangedTrack = true;

        settings.EnablePropertyChangedTrack.Should().BeTrue();
    }

    [Fact]
    public void Should_SetIgnoreDbNull_When_Assigned()
    {
        var settings = new Settings();

        settings.IgnoreDbNull = true;

        settings.IgnoreDbNull.Should().BeTrue();
    }

    #endregion

    private class MockMiddleware : IMiddleware
    {
        public int Order => 0;
        public IMiddleware Next { get; set; }
        public void Invoke<TResult>(ExecutionContext executionContext)
        {
        }

        public System.Threading.Tasks.Task InvokeAsync<TResult>(ExecutionContext executionContext)
        {
            return System.Threading.Tasks.Task.CompletedTask;
        }

        public void SetupSmartSql(SmartSqlBuilder smartSqlBuilder)
        {
        }
    }
}
