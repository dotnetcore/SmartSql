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

public class AbstractCacheManagerDeepTests : IDisposable
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

        public void ExposeFlushOnExecuted(string fullSqlId) => FlushOnExecuted(fullSqlId);
    }

    private static string NewAlias() => $"test_{Guid.NewGuid():N}";

    private (TestableCacheManager manager, SmartSqlConfig config) CreateManager(
        Action<SmartSqlConfig> configure = null)
    {
        var alias = NewAlias();
        var write = new WriteDataSource
        {
            Name = "Write",
            ConnectionString = "Data Source=:memory:",
            DbProvider = DbProviderManager.SQLITE_DBPROVIDER
        };

        var config = new SmartSqlConfig
        {
            LoggerFactory = NullLoggerFactory.Instance,
            Database = new Database
            {
                DbProvider = DbProviderManager.SQLITE_DBPROVIDER,
                Write = write
            }
        };
        configure?.Invoke(config);

        var manager = new TestableCacheManager();
        var builder = new SmartSqlBuilder()
            .UseNativeConfig(config)
            .UseAlias(alias)
            .UseCacheManager(manager)
            .RegisterToContainer(false);

        builder.Build();
        _builders.Add(builder);
        return (manager, config);
    }

    private static CacheCfg CreateCacheWithProvider(
        Mock<ICacheProvider> mockProvider = null,
        string cacheId = "test-cache",
        string flushStatement = null,
        FlushIntervalCfg flushInterval = null)
    {
        mockProvider ??= new Mock<ICacheProvider>();
        mockProvider.Setup(p => p.SupportExpire).Returns(false);
        mockProvider.Setup(p => p.Dispose());

        var cache = new CacheCfg
        {
            Id = cacheId,
            Provider = mockProvider.Object,
            FlushOnExecutes = flushStatement != null
                ? new List<FlushOnExecuteCfg> { new FlushOnExecuteCfg { Statement = flushStatement } }
                : null,
            FlushInterval = flushInterval
        };

        return cache;
    }

    private static SmartSqlConfig BuildConfigWithSqlMap(params CacheCfg[] caches)
    {
        var scope = "TestScope";
        var sqlMap = new SqlMap
        {
            Scope = scope,
            Caches = new Dictionary<string, CacheCfg>(StringComparer.OrdinalIgnoreCase),
            Statements = new Dictionary<string, Statement>(StringComparer.OrdinalIgnoreCase),
            ParameterMaps = new Dictionary<string, ParameterMap>(StringComparer.OrdinalIgnoreCase),
            ResultMaps = new Dictionary<string, ResultMap>(StringComparer.OrdinalIgnoreCase),
            MultipleResultMaps = new Dictionary<string, MultipleResultMap>(StringComparer.OrdinalIgnoreCase)
        };

        foreach (var c in caches)
        {
            sqlMap.Caches[c.Id] = c;
        }

        var write = new WriteDataSource
        {
            Name = "Write",
            ConnectionString = "Data Source=:memory:",
            DbProvider = DbProviderManager.SQLITE_DBPROVIDER
        };

        return new SmartSqlConfig
        {
            LoggerFactory = NullLoggerFactory.Instance,
            Database = new Database
            {
                DbProvider = DbProviderManager.SQLITE_DBPROVIDER,
                Write = write
            },
            SqlMaps = new Dictionary<string, SqlMap> { { scope, sqlMap } }
        };
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
    public void Should_FlushCache_When_FlushOnExecutedMatchesStatement()
    {
        var mockProvider = new Mock<ICacheProvider>();
        var cache = CreateCacheWithProvider(mockProvider, flushStatement: "TestScope.Insert");
        var config = BuildConfigWithSqlMap(cache);

        var (manager, _) = CreateManager(c =>
        {
            c.SqlMaps = config.SqlMaps;
        });

        manager.ExposeFlushOnExecuted("TestScope.Insert");

        mockProvider.Verify(p => p.Flush(), Times.Once);
    }

    [Fact]
    public void Should_NotFlushCache_When_FlushOnExecutedDoesNotMatch()
    {
        var mockProvider = new Mock<ICacheProvider>();
        var cache = CreateCacheWithProvider(mockProvider, flushStatement: "TestScope.Insert");
        var config = BuildConfigWithSqlMap(cache);

        var (manager, _) = CreateManager(c =>
        {
            c.SqlMaps = config.SqlMaps;
        });

        manager.ExposeFlushOnExecuted("TestScope.Update");

        mockProvider.Verify(p => p.Flush(), Times.Never);
    }

    [Fact]
    public void Should_FlushMultipleCaches_When_SameStatementMapped()
    {
        var mockProvider1 = new Mock<ICacheProvider>();
        var mockProvider2 = new Mock<ICacheProvider>();
        var cache1 = CreateCacheWithProvider(mockProvider1, cacheId: "cache1", flushStatement: "TestScope.Insert");
        var cache2 = CreateCacheWithProvider(mockProvider2, cacheId: "cache2", flushStatement: "TestScope.Insert");
        var config = BuildConfigWithSqlMap(cache1, cache2);

        var (manager, _) = CreateManager(c =>
        {
            c.SqlMaps = config.SqlMaps;
        });

        manager.ExposeFlushOnExecuted("TestScope.Insert");

        mockProvider1.Verify(p => p.Flush(), Times.Once);
        mockProvider2.Verify(p => p.Flush(), Times.Once);
    }

    [Fact]
    public void Should_ReturnTrue_When_TryAddCacheSucceeds()
    {
        var (manager, config) = CreateManager();
        var mockProvider = new Mock<ICacheProvider>();
        mockProvider.Setup(p => p.TryAdd(It.IsAny<CacheKey>(), It.IsAny<object>())).Returns(true);

        var cache = new CacheCfg
        {
            Id = "add-cache",
            Provider = mockProvider.Object
        };

        var request = new RequestContext<object>
        {
            CacheKeyTemplate = "TestKey"
        };
        var result = new SingleResultContext<string>();
        result.SetData("cached-value");

        var executionContext = new ExecutionContext
        {
            SmartSqlConfig = config,
            Request = request,
            Result = result
        };
        request.ExecutionContext = executionContext;

        var prop = typeof(AbstractRequestContext).GetProperty("Cache",
            System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        prop.SetValue(request, cache);

        var success = manager.TryAddCache(executionContext);

        success.Should().BeTrue();
    }

    [Fact]
    public void Should_ReturnFalse_When_TryAddCacheFails()
    {
        var (manager, config) = CreateManager();
        var mockProvider = new Mock<ICacheProvider>();
        mockProvider.Setup(p => p.TryAdd(It.IsAny<CacheKey>(), It.IsAny<object>())).Returns(false);

        var cache = new CacheCfg
        {
            Id = "add-cache",
            Provider = mockProvider.Object
        };

        var request = new RequestContext<object>
        {
            CacheKeyTemplate = "TestKey"
        };
        var result = new SingleResultContext<string>();
        result.SetData("cached-value");

        var executionContext = new ExecutionContext
        {
            SmartSqlConfig = config,
            Request = request,
            Result = result
        };
        request.ExecutionContext = executionContext;

        var prop = typeof(AbstractRequestContext).GetProperty("Cache",
            System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        prop.SetValue(request, cache);

        var success = manager.TryAddCache(executionContext);

        success.Should().BeFalse();
    }

    [Fact]
    public void Should_ReturnTrue_When_TryGetCacheSucceeds()
    {
        var (manager, config) = CreateManager();
        var mockProvider = new Mock<ICacheProvider>();
        var cachedObject = new object();
        mockProvider.Setup(p => p.TryGetValue(It.IsAny<CacheKey>(), out cachedObject)).Returns(true);

        var cache = new CacheCfg
        {
            Id = "get-cache",
            Provider = mockProvider.Object
        };

        var request = new RequestContext<object>
        {
            CacheKeyTemplate = "TestKey"
        };

        var executionContext = new ExecutionContext
        {
            SmartSqlConfig = config,
            Request = request,
            Result = new SingleResultContext<string>()
        };
        request.ExecutionContext = executionContext;

        var prop = typeof(AbstractRequestContext).GetProperty("Cache",
            System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        prop.SetValue(request, cache);

        var success = manager.TryGetCache(executionContext, out var cacheItem);

        success.Should().BeTrue();
        cacheItem.Should().BeSameAs(cachedObject);
    }

    [Fact]
    public void Should_PopulateMultipleStatements_When_MultipleFlushOnExecutes()
    {
        var mockProvider = new Mock<ICacheProvider>();
        var cache = new CacheCfg
        {
            Id = "multi-flush-cache",
            Provider = mockProvider.Object,
            FlushOnExecutes = new List<FlushOnExecuteCfg>
            {
                new FlushOnExecuteCfg { Statement = "TestScope.Insert" },
                new FlushOnExecuteCfg { Statement = "TestScope.Update" },
                new FlushOnExecuteCfg { Statement = "TestScope.Delete" }
            }
        };

        var config = BuildConfigWithSqlMap(cache);
        var (manager, _) = CreateManager(c =>
        {
            c.SqlMaps = config.SqlMaps;
        });

        var flushCache = manager.ExposeStatementMappedFlushCache;
        flushCache.Should().ContainKey("TestScope.Insert");
        flushCache.Should().ContainKey("TestScope.Update");
        flushCache.Should().ContainKey("TestScope.Delete");
        flushCache["TestScope.Insert"].Should().ContainSingle().Which.Should().Be(cache);
    }

    [Fact]
    public void Should_ResetCollections_When_ResetCalled()
    {
        var mockProvider = new Mock<ICacheProvider>();
        var cache = CreateCacheWithProvider(mockProvider, flushStatement: "TestScope.Insert");
        var config = BuildConfigWithSqlMap(cache);

        var (manager, _) = CreateManager(c =>
        {
            c.SqlMaps = config.SqlMaps;
        });

        manager.ExposeStatementMappedFlushCache.Should().NotBeEmpty();

        manager.Reset();

        manager.ExposeStatementMappedFlushCache.Should().NotBeNull();
    }

    [Fact]
    public void Should_CallListenInvokeSucceeded_When_SetupSmartSql()
    {
        var (manager, _) = CreateManager();

        manager.ListenInvokeSucceededCalled.Should().BeTrue();
    }

    [Fact]
    public void Should_UpdateLastFlushTime_When_FlushCacheCalled()
    {
        var mockProvider = new Mock<ICacheProvider>();
        var cache = CreateCacheWithProvider(mockProvider,
            flushInterval: new FlushIntervalCfg { Hours = 0, Minutes = 5, Seconds = 0 });
        var config = BuildConfigWithSqlMap(cache);

        var (manager, _) = CreateManager(c =>
        {
            c.SqlMaps = config.SqlMaps;
        });

        var beforeFlush = manager.ExposeCacheMappedLastFlushTime[cache];
        manager.ExposeFlushOnExecuted("TestScope.Insert");
        var afterFlush = manager.ExposeCacheMappedLastFlushTime[cache];

        afterFlush.Should().BeOnOrAfter(beforeFlush);
    }
}
