using System;
using System.Collections.Generic;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using SmartSql;
using SmartSql.Cache;
using SmartSql.Command;
using SmartSql.ConfigBuilder;
using SmartSql.Configuration;
using SmartSql.DataSource;
using SmartSql.DbSession;
using SmartSql.Filters;
using SmartSql.Middlewares;
using Xunit;

namespace SmartSql.Test.Unit;

public class SmartSqlBuilderDeepTests
{
    [Fact]
    public void Should_RegisterEntity_When_RegisterEntityTypeCalled()
    {
        var builder = new SmartSqlBuilder()
            .UseAlias("TestBuilder_RegisterEntity")
            .UseDataSource(DbProvider.SQLITE, "Data Source=:memory:")
            .RegisterToContainer(false)
            .RegisterEntity(typeof(TestEntity));

        builder.Build();

        builder.EntityTypes.Should().ContainSingle();
        builder.EntityTypes[0].Should().Be(typeof(TestEntity));
    }

    [Fact]
    public void Should_RegisterMultipleEntities_When_RegisterEntityCalledMultipleTimes()
    {
        var builder = new SmartSqlBuilder()
            .UseAlias("TestBuilder_MultipleEntities")
            .UseDataSource(DbProvider.SQLITE, "Data Source=:memory:")
            .RegisterToContainer(false)
            .RegisterEntity(typeof(TestEntity))
            .RegisterEntity(typeof(AnotherEntity));

        builder.Build();

        builder.EntityTypes.Should().HaveCount(2);
        builder.EntityTypes.Should().Contain(typeof(TestEntity));
        builder.EntityTypes.Should().Contain(typeof(AnotherEntity));
    }

    [Fact]
    public void Should_ChainMethods_When_UsingFluentInterface()
    {
        var builder = new SmartSqlBuilder()
            .UseAlias("TestBuilder_Fluent")
            .UseDataSource(DbProvider.SQLITE, "Data Source=:memory:")
            .UseCache(true)
            .UseIgnoreDbNull(true)
            .RegisterToContainer(false)
            .RegisterEntity(typeof(TestEntity));

        builder.Should().NotBeNull();
        builder.Alias.Should().Be("TestBuilder_Fluent");
        builder.IsCacheEnabled.Should().BeTrue();
        builder.IgnoreDbNull.Should().BeTrue();
    }

    [Fact]
    public void Should_AddFilter_When_AddFilterGenericCalled()
    {
        var builder = new SmartSqlBuilder()
            .UseAlias("TestBuilder_AddFilter")
            .UseDataSource(DbProvider.SQLITE, "Data Source=:memory:")
            .RegisterToContainer(false)
            .AddFilter<TestFilter>();

        builder.Build();

        builder.Filters.Should().ContainSingle();
    }

    [Fact]
    public void Should_AddFilter_When_AddFilterInstanceCalled()
    {
        var filter = new TestFilter();
        var builder = new SmartSqlBuilder()
            .UseAlias("TestBuilder_AddFilterInstance")
            .UseDataSource(DbProvider.SQLITE, "Data Source=:memory:")
            .RegisterToContainer(false)
            .AddFilter(filter);

        builder.Build();

        builder.Filters.Should().ContainSingle();
        builder.Filters[0].Should().BeSameAs(filter);
    }

    [Fact]
    public void Should_UseDataSourceFilter_When_UseDataSourceFilterCalled()
    {
        var mockFilter = new Mock<IDataSourceFilter>().Object;
        var builder = new SmartSqlBuilder()
            .UseAlias("TestBuilder_DataSourceFilter")
            .UseDataSource(DbProvider.SQLITE, "Data Source=:memory:")
            .UseDataSourceFilter(mockFilter)
            .RegisterToContainer(false);

        builder.Build();

        builder.SmartSqlConfig.DataSourceFilter.Should().BeSameAs(mockFilter);
    }

    [Fact]
    public void Should_UseCustomCacheManager_When_UseCacheManagerCalled()
    {
        var mockCacheManager = new Mock<ICacheManager>().Object;
        var builder = new SmartSqlBuilder()
            .UseAlias("TestBuilder_CustomCacheManager")
            .UseDataSource(DbProvider.SQLITE, "Data Source=:memory:")
            .UseCacheManager(mockCacheManager)
            .RegisterToContainer(false);

        builder.Build();

        builder.CacheManager.Should().BeSameAs(mockCacheManager);
    }

    [Fact]
    public void Should_AddMiddleware_When_AddMiddlewareCalled()
    {
        var mockMiddleware = new Mock<IMiddleware>().Object;
        var builder = new SmartSqlBuilder()
            .UseAlias("TestBuilder_AddMiddleware")
            .UseDataSource(DbProvider.SQLITE, "Data Source=:memory:")
            .RegisterToContainer(false)
            .AddMiddleware(mockMiddleware);

        builder.Build();

        builder.Middlewares.Should().ContainSingle();
        builder.Middlewares[0].Should().BeSameAs(mockMiddleware);
    }

    [Fact]
    public void Should_UseLoggerFactory_When_UseLoggerFactoryCalled()
    {
        var loggerFactory = NullLoggerFactory.Instance;
        var builder = new SmartSqlBuilder()
            .UseAlias("TestBuilder_LoggerFactory")
            .UseDataSource(DbProvider.SQLITE, "Data Source=:memory:")
            .UseLoggerFactory(loggerFactory)
            .RegisterToContainer(false);

        builder.Build();

        builder.LoggerFactory.Should().BeSameAs(loggerFactory);
    }

    [Fact]
    public void Should_NotThrow_When_UseLoggerFactoryWithNull()
    {
        var builder = new SmartSqlBuilder()
            .UseAlias("TestBuilder_NullLogger")
            .UseDataSource(DbProvider.SQLITE, "Data Source=:memory:")
            .UseLoggerFactory(null)
            .RegisterToContainer(false);

        var act = () => builder.Build();

        act.Should().NotThrow();
    }

    [Fact]
    public void Should_UseProperties_When_UsePropertiesCalled()
    {
        var properties = new Dictionary<string, string>
        {
            { "Key1", "Value1" },
            { "Key2", "Value2" }
        };
        var builder = new SmartSqlBuilder()
            .UseAlias("TestBuilder_PropertiesDict")
            .UseDataSource(DbProvider.SQLITE, "Data Source=:memory:")
            .UseProperties((IEnumerable<KeyValuePair<string, string>>)properties)
            .RegisterToContainer(false);

        builder.ImportProperties.Should().HaveCount(2);
        builder.ImportProperties.Should().Contain(new KeyValuePair<string, string>("Key1", "Value1"));
        builder.ImportProperties.Should().Contain(new KeyValuePair<string, string>("Key2", "Value2"));
    }

    [Fact]
    public void Should_UsePropertiesFromEnv_When_UsePropertiesFromEnvCalled()
    {
        var builder = new SmartSqlBuilder()
            .UseAlias("TestBuilder_EnvProperties")
            .UseDataSource(DbProvider.SQLITE, "Data Source=:memory:")
            .UsePropertiesFromEnv()
            .RegisterToContainer(false);

        builder.ImportProperties.Should().NotBeEmpty();
    }

    [Fact]
    public void Should_UseCUDConfigBuilder_When_UseCUDConfigBuilderCalled()
    {
        var builder = new SmartSqlBuilder()
            .UseAlias("TestBuilder_CUDConfig")
            .UseDataSource(DbProvider.SQLITE, "Data Source=:memory:")
            .UseCUDConfigBuilder(true)
            .RegisterToContainer(false)
            .RegisterEntity(typeof(TestEntity));

        builder.Build();

        builder.IsUseCUDConfigBuilder.Should().BeTrue();
    }

    [Fact]
    public void Should_NotRegisterToContainer_When_RegisterToContainerFalse()
    {
        var builder = new SmartSqlBuilder()
            .UseAlias("TestBuilder_NoContainer")
            .UseDataSource(DbProvider.SQLITE, "Data Source=:memory:")
            .RegisterToContainer(false);

        builder.Build();

        builder.Registered.Should().BeFalse();
    }

    [Fact]
    public void Should_ListenInvokeSucceeded_When_ListenInvokeSucceededCalled()
    {
        var contextCaptured = new List<ExecutionContext>();
        var builder = new SmartSqlBuilder()
            .UseAlias("TestBuilder_InvokeSucceeded")
            .UseDataSource(DbProvider.SQLITE, "Data Source=:memory:")
            .RegisterToContainer(false)
            .ListenInvokeSucceeded(ctx => contextCaptured.Add(ctx));

        builder.Build();

        builder.InvokeSucceeded.Should().NotBeNull();
    }

    [Fact]
    public void Should_ReturnSameBuilder_When_BuildCalledMultipleTimes()
    {
        var builder = new SmartSqlBuilder()
            .UseAlias("TestBuilder_IdempotentBuild")
            .UseDataSource(DbProvider.SQLITE, "Data Source=:memory:")
            .RegisterToContainer(false);

        var result1 = builder.Build();
        var result2 = builder.Build();
        var result3 = builder.Build();

        result1.Should().BeSameAs(result2);
        result2.Should().BeSameAs(result3);
        result1.Should().BeSameAs(builder);
    }

    [Fact]
    public void Should_SetCommandExecuter_When_UseCommandExecuterCalled()
    {
        var mockExecuter = new Mock<ICommandExecuter>().Object;
        var builder = new SmartSqlBuilder()
            .UseAlias("TestBuilder_CommandExecuter")
            .UseDataSource(DbProvider.SQLITE, "Data Source=:memory:")
            .UseCommandExecuter(mockExecuter)
            .RegisterToContainer(false);

        builder.Build();

        builder.SmartSqlConfig.CommandExecuter.Should().BeSameAs(mockExecuter);
    }

    [Fact]
    public void Should_ReturnBuilder_When_UseCommandExecuterCalled()
    {
        var mockExecuter = new Mock<ICommandExecuter>().Object;
        var builder = new SmartSqlBuilder()
            .UseCommandExecuter(mockExecuter);

        builder.Should().NotBeNull();
        builder.Should().BeOfType<SmartSqlBuilder>();
    }

    [Fact]
    public void Should_SetIsCacheEnabled_When_UseCacheCalledWithFalse()
    {
        var builder = new SmartSqlBuilder()
            .UseCache(false);

        builder.IsCacheEnabled.Should().BeFalse();
    }

    [Fact]
    public void Should_SetIsCacheEnabled_When_UseCacheCalledWithTrue()
    {
        var builder = new SmartSqlBuilder()
            .UseCache(true);

        builder.IsCacheEnabled.Should().BeTrue();
    }

    [Fact]
    public void Should_BuildPipeline_When_Built()
    {
        var builder = new SmartSqlBuilder()
            .UseAlias("TestBuilder_PipelineBuild")
            .UseDataSource(DbProvider.SQLITE, "Data Source=:memory:")
            .UseCache(true)
            .RegisterToContainer(false);

        builder.Build();

        builder.SmartSqlConfig.Pipeline.Should().NotBeNull();
    }

    [Fact]
    public void Should_NotDuplicatePipeline_When_BuiltTwice()
    {
        var builder = new SmartSqlBuilder()
            .UseAlias("TestBuilder_PipelineOnce")
            .UseDataSource(DbProvider.SQLITE, "Data Source=:memory:")
            .UseCache(true)
            .RegisterToContainer(false);

        builder.Build();
        var pipeline1 = builder.SmartSqlConfig.Pipeline;

        builder.Build();
        var pipeline2 = builder.SmartSqlConfig.Pipeline;

        pipeline1.Should().BeSameAs(pipeline2);
    }

    [Fact]
    public void Should_HaveNoneCacheManager_When_CacheDisabled()
    {
        var builder = new SmartSqlBuilder()
            .UseAlias("TestBuilder_NoneCache")
            .UseDataSource(DbProvider.SQLITE, "Data Source=:memory:")
            .UseCache(false)
            .RegisterToContainer(false);

        builder.Build();

        builder.SmartSqlConfig.CacheManager.Should().BeOfType<NoneCacheManager>();
    }

    [Fact]
    public void Should_HaveCacheManager_When_CacheEnabled()
    {
        var builder = new SmartSqlBuilder()
            .UseAlias("TestBuilder_WithCache")
            .UseDataSource(DbProvider.SQLITE, "Data Source=:memory:")
            .UseCache(true)
            .RegisterToContainer(false);

        builder.Build();

        builder.SmartSqlConfig.CacheManager.Should().NotBeNull();
    }

    private class TestEntity
    {
        public long Id { get; set; }
        public string Name { get; set; }
    }

    private class AnotherEntity
    {
        public Guid Id { get; set; }
        public string Value { get; set; }
    }

    private class TestFilter : IFilter
    {
    }
}
