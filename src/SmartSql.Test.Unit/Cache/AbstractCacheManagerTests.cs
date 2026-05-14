using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using SmartSql.Cache;
using SmartSql.Configuration;
using SmartSql.DataSource;
using SmartSql.DbSession;
using Xunit;
using CacheCfg = SmartSql.Configuration.Cache;
using FlushIntervalCfg = SmartSql.Configuration.FlushInterval;
using FlushOnExecuteCfg = SmartSql.Configuration.FlushOnExecute;

namespace SmartSql.Test.Unit.Cache;

public class AbstractCacheManagerTests : IDisposable
{
    private readonly List<SmartSqlBuilder> _builders = new();

    private class TestableCacheManager : AbstractCacheManager
    {
        public bool ListenInvokeSucceededCalled { get; private set; }

        protected override void ListenInvokeSucceeded()
        {
            ListenInvokeSucceededCalled = true;
        }

        public ConcurrentDictionary<string, IList<CacheCfg>> ExposeStatementMappedFlushCache =>
            StatementMappedFlushCache;

        public ConcurrentDictionary<CacheCfg, DateTime> ExposeCacheMappedLastFlushTime =>
            CacheMappedLastFlushTime;
    }

    private static string NewAlias() => $"test_{Guid.NewGuid():N}";

    private static SmartSqlConfig CreateTestConfig(string alias = null)
    {
        var write = new WriteDataSource
        {
            Name = "Write",
            ConnectionString = "Data Source=:memory:",
            DbProvider = DbProviderManager.SQLITE_DBPROVIDER
        };

        return new SmartSqlConfig
        {
            Alias = alias ?? NewAlias(),
            LoggerFactory = NullLoggerFactory.Instance,
            Database = new Database
            {
                DbProvider = DbProviderManager.SQLITE_DBPROVIDER,
                Write = write
            }
        };
    }

    private (TestableCacheManager manager, SmartSqlConfig config) CreateManager(
        Action<SmartSqlConfig> configure = null)
    {
        var alias = NewAlias();
        var config = CreateTestConfig(alias);
        configure?.Invoke(config);

        var manager = new TestableCacheManager();
        var builder = new SmartSqlBuilder()
            .UseNativeConfig(config)
            .UseCacheManager(manager)
            .RegisterToContainer(false);

        builder.Build();
        _builders.Add(builder);
        return (manager, config);
    }

    private static SmartSqlConfig CreateConfigWithCache(
        out CacheCfg cache,
        string cacheId = "test-cache",
        string scope = "TestScope",
        string flushStatement = null,
        FlushIntervalCfg flushInterval = null)
    {
        var mockProvider = new Mock<ICacheProvider>();
        mockProvider.Setup(p => p.SupportExpire).Returns(false);
        mockProvider.Setup(p => p.TryAdd(It.IsAny<CacheKey>(), It.IsAny<object>())).Returns(true);
        mockProvider.Setup(p => p.Dispose());

        cache = new CacheCfg
        {
            Id = cacheId,
            Provider = mockProvider.Object,
            FlushOnExecutes = flushStatement != null
                ? new List<FlushOnExecuteCfg> { new FlushOnExecuteCfg { Statement = flushStatement } }
                : null,
            FlushInterval = flushInterval
        };

        var write = new WriteDataSource
        {
            Name = "Write",
            ConnectionString = "Data Source=:memory:",
            DbProvider = DbProviderManager.SQLITE_DBPROVIDER
        };

        var sqlMap = new SqlMap
        {
            Scope = scope,
            Caches = new Dictionary<string, CacheCfg> { { cacheId, cache } },
            Statements = new Dictionary<string, Statement>(),
            ParameterMaps = new Dictionary<string, ParameterMap>(),
            ResultMaps = new Dictionary<string, ResultMap>(),
            MultipleResultMaps = new Dictionary<string, MultipleResultMap>()
        };

        var config = new SmartSqlConfig
        {
            LoggerFactory = NullLoggerFactory.Instance,
            Database = new Database
            {
                DbProvider = DbProviderManager.SQLITE_DBPROVIDER,
                Write = write
            },
            SqlMaps = new Dictionary<string, SqlMap> { { scope, sqlMap } }
        };

        return config;
    }

    public void Dispose()
    {
        foreach (var builder in _builders)
        {
            builder.Dispose();
        }

        _builders.Clear();
    }

    [Fact]
    public void Should_SetupSmartSql_When_Called()
    {
        var alias = NewAlias();
        var config = CreateTestConfig(alias);

        var manager = new TestableCacheManager();
        var builder = new SmartSqlBuilder()
            .UseNativeConfig(config)
            .UseCacheManager(manager)
            .RegisterToContainer(false);

        builder.Build();
        _builders.Add(builder);

        manager.ListenInvokeSucceededCalled.Should().BeTrue();
    }

    [Fact]
    public void Should_PopulateStatementMappedFlushCache_When_ResetWithFlushOnExecutes()
    {
        var config = CreateConfigWithCache(out var cache, flushStatement: "TestScope.Insert");

        var (manager, _) = CreateManager(c =>
        {
            c.SqlMaps = config.SqlMaps;
        });

        var flushCache = manager.ExposeStatementMappedFlushCache;
        flushCache.Should().ContainKey("TestScope.Insert");
        flushCache["TestScope.Insert"].Should().Contain(cache);
    }

    [Fact]
    public void Should_PopulateCacheMappedLastFlushTime_When_ResetWithFlushInterval()
    {
        var config = CreateConfigWithCache(out var cache,
            flushInterval: new FlushIntervalCfg { Hours = 0, Minutes = 5, Seconds = 0 });

        var (manager, _) = CreateManager(c =>
        {
            c.SqlMaps = config.SqlMaps;
        });

        var lastFlush = manager.ExposeCacheMappedLastFlushTime;
        lastFlush.Should().ContainKey(cache);
    }

    [Fact]
    public void Should_ReturnFalse_When_TryGetCacheWithNoCacheOnRequest()
    {
        var (manager, _) = CreateManager();

        var request = new RequestContext<object>
        {
            ExecutionContext = new ExecutionContext
            {
                SmartSqlConfig = new SmartSqlConfig { LoggerFactory = NullLoggerFactory.Instance }
            }
        };

        var result = manager.TryGetCache(
            new ExecutionContext
            {
                Request = request,
                SmartSqlConfig = new SmartSqlConfig { LoggerFactory = NullLoggerFactory.Instance }
            },
            out var cacheItem);

        result.Should().BeFalse();
        cacheItem.Should().BeNull();
    }

    [Fact]
    public void Should_ReturnFalse_When_TryAddCacheWithNoCacheOnRequest()
    {
        var (manager, _) = CreateManager();

        var result = manager.TryAddCache(new ExecutionContext
        {
            Request = new RequestContext<object>(),
            SmartSqlConfig = new SmartSqlConfig { LoggerFactory = NullLoggerFactory.Instance }
        });

        result.Should().BeFalse();
    }

    [Fact]
    public void Should_ReturnFalse_When_TryGetCacheWithoutCacheConfigured()
    {
        var (manager, config) = CreateManager();

        var request = new RequestContext<object>
        {
            CacheKeyTemplate = "TestKey",
            ExecutionContext = new ExecutionContext
            {
                SmartSqlConfig = config
            }
        };
        request.SetupParameters();

        var executionContext = new ExecutionContext
        {
            Request = request,
            SmartSqlConfig = config
        };

        var result = manager.TryGetCache(executionContext, out var cacheItem);

        result.Should().BeFalse();
        cacheItem.Should().BeNull();
    }

    [Fact]
    public void Should_NotPopulateStatementMappedFlushCache_When_NoFlushOnExecutes()
    {
        var config = CreateConfigWithCache(out _);

        var (manager, _) = CreateManager(c =>
        {
            c.SqlMaps = config.SqlMaps;
        });

        var flushCache = manager.ExposeStatementMappedFlushCache;
        flushCache.Should().BeEmpty();
    }

    [Fact]
    public void Should_NotPopulateCacheMappedLastFlushTime_When_NoFlushInterval()
    {
        var config = CreateConfigWithCache(out _);

        var (manager, _) = CreateManager(c =>
        {
            c.SqlMaps = config.SqlMaps;
        });

        var lastFlush = manager.ExposeCacheMappedLastFlushTime;
        lastFlush.Should().BeEmpty();
    }
}
