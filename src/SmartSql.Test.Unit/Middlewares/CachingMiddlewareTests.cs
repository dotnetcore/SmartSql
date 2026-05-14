using System;
using System.Data.Common;
using System.Reflection;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using SmartSql.Cache;
using SmartSql.Configuration;
using SmartSql.DataSource;
using SmartSql.DbSession;
using SmartSql.Middlewares;
using Xunit;
using CacheCfg = SmartSql.Configuration.Cache;

namespace SmartSql.Test.Unit.Middlewares;

public class CachingMiddlewareTests
{
    private static CachingMiddleware CreateMiddleware(ICacheManager cacheManager = null)
    {
        var config = new SmartSqlConfig
        {
            Database = new Database
            {
                DbProvider = new DbProvider { ParameterPrefix = "@" }
            },
            CacheManager = cacheManager ?? new Mock<ICacheManager>().Object
        };

        var builder = new SmartSqlBuilder();
        var configProp = typeof(SmartSqlBuilder).GetProperty("SmartSqlConfig");
        configProp.SetValue(builder, config);

        var middleware = new CachingMiddleware();
        middleware.SetupSmartSql(builder);

        return middleware;
    }

    private static void SetCache(AbstractRequestContext request, CacheCfg cache)
    {
        var prop = typeof(AbstractRequestContext).GetProperty("Cache",
            BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        prop.SetValue(request, cache);
    }

    private static ExecutionContext CreateContext(
        bool withTransaction = false,
        bool withCache = true)
    {
        var mockSession = new Mock<IDbSession>();
        if (withTransaction)
        {
            mockSession.Setup(s => s.Transaction).Returns(new Mock<DbTransaction>().Object);
        }

        var req = new RequestContext<object>();
        if (withCache)
        {
            SetCache(req, new CacheCfg { Id = "TestCache" });
        }

        return new ExecutionContext
        {
            Request = req,
            Result = new SingleResultContext<string>(),
            DbSession = mockSession.Object,
            SmartSqlConfig = new SmartSqlConfig
            {
                Database = new Database
                {
                    DbProvider = new DbProvider { ParameterPrefix = "@" }
                }
            }
        };
    }

    [Fact]
    public void Should_HaveOrder200()
    {
        var middleware = new CachingMiddleware();

        middleware.Order.Should().Be(200);
    }

    [Fact]
    public void Should_InvokeNext_When_CacheIsNull()
    {
        var mockCacheManager = new Mock<ICacheManager>();
        var middleware = CreateMiddleware(mockCacheManager.Object);
        var context = CreateContext(withCache: false);
        var nextCalled = false;

        var mockNext = new Mock<IMiddleware>();
        mockNext.Setup(m => m.Invoke<string>(context)).Callback(() => nextCalled = true);
        middleware.Next = mockNext.Object;

        middleware.Invoke<string>(context);

        nextCalled.Should().BeTrue();
        mockCacheManager.Verify(m => m.TryGetCache(It.IsAny<ExecutionContext>(), out It.Ref<object>.IsAny), Times.Never);
    }

    [Fact]
    public void Should_InvokeNext_When_TransactionExists()
    {
        var mockCacheManager = new Mock<ICacheManager>();
        var middleware = CreateMiddleware(mockCacheManager.Object);
        var context = CreateContext(withTransaction: true, withCache: true);
        var nextCalled = false;

        var mockNext = new Mock<IMiddleware>();
        mockNext.Setup(m => m.Invoke<string>(context)).Callback(() => nextCalled = true);
        middleware.Next = mockNext.Object;

        middleware.Invoke<string>(context);

        nextCalled.Should().BeTrue();
        mockCacheManager.Verify(m => m.TryGetCache(It.IsAny<ExecutionContext>(), out It.Ref<object>.IsAny), Times.Never);
    }

    [Fact]
    public void Should_ReturnCachedValue_When_CacheHit()
    {
        var cachedValue = "CachedResult";
        var mockCacheManager = new Mock<ICacheManager>();
        mockCacheManager.Setup(m => m.TryGetCache(It.IsAny<ExecutionContext>(), out It.Ref<object>.IsAny))
            .Returns((ExecutionContext ctx, out object item) =>
            {
                item = cachedValue;
                return true;
            });

        var middleware = CreateMiddleware(mockCacheManager.Object);
        var context = CreateContext(withTransaction: false, withCache: true);
        var nextCalled = false;

        var mockNext = new Mock<IMiddleware>();
        mockNext.Setup(m => m.Invoke<string>(context)).Callback(() => nextCalled = true);
        middleware.Next = mockNext.Object;

        middleware.Invoke<string>(context);

        nextCalled.Should().BeFalse();
        context.Result.GetData().Should().Be(cachedValue);
        context.Result.FromCache.Should().BeTrue();
    }

    [Fact]
    public void Should_InvokeNextAndAddToCache_When_CacheMiss()
    {
        var mockCacheManager = new Mock<ICacheManager>();
        mockCacheManager.Setup(m => m.TryGetCache(It.IsAny<ExecutionContext>(), out It.Ref<object>.IsAny))
            .Returns((ExecutionContext ctx, out object item) =>
            {
                item = null;
                return false;
            });
        mockCacheManager.Setup(m => m.TryAddCache(It.IsAny<ExecutionContext>())).Returns(true);

        var middleware = CreateMiddleware(mockCacheManager.Object);
        var context = CreateContext(withTransaction: false, withCache: true);
        var nextCalled = false;

        var mockNext = new Mock<IMiddleware>();
        mockNext.Setup(m => m.Invoke<string>(context)).Callback(() => nextCalled = true);
        middleware.Next = mockNext.Object;

        middleware.Invoke<string>(context);

        nextCalled.Should().BeTrue();
        mockCacheManager.Verify(m => m.TryAddCache(context), Times.Once);
    }

    [Fact]
    public async Task Should_InvokeNextAsync_When_CacheIsNull()
    {
        var mockCacheManager = new Mock<ICacheManager>();
        var middleware = CreateMiddleware(mockCacheManager.Object);
        var context = CreateContext(withCache: false);
        var nextCalled = false;

        var mockNext = new Mock<IMiddleware>();
        mockNext.Setup(m => m.InvokeAsync<string>(context)).Callback(() => nextCalled = true).Returns(Task.CompletedTask);
        middleware.Next = mockNext.Object;

        await middleware.InvokeAsync<string>(context);

        nextCalled.Should().BeTrue();
    }

    [Fact]
    public async Task Should_ReturnCachedValueAsync_When_CacheHit()
    {
        var cachedValue = "AsyncCachedResult";
        var mockCacheManager = new Mock<ICacheManager>();
        mockCacheManager.Setup(m => m.TryGetCache(It.IsAny<ExecutionContext>(), out It.Ref<object>.IsAny))
            .Returns((ExecutionContext ctx, out object item) =>
            {
                item = cachedValue;
                return true;
            });

        var middleware = CreateMiddleware(mockCacheManager.Object);
        var context = CreateContext(withTransaction: false, withCache: true);
        var nextCalled = false;

        var mockNext = new Mock<IMiddleware>();
        mockNext.Setup(m => m.InvokeAsync<string>(context)).Callback(() => nextCalled = true).Returns(Task.CompletedTask);
        middleware.Next = mockNext.Object;

        await middleware.InvokeAsync<string>(context);

        nextCalled.Should().BeFalse();
        context.Result.GetData().Should().Be(cachedValue);
        context.Result.FromCache.Should().BeTrue();
    }

    [Fact]
    public async Task Should_InvokeNextAndAddToCacheAsync_When_CacheMiss()
    {
        var mockCacheManager = new Mock<ICacheManager>();
        mockCacheManager.Setup(m => m.TryGetCache(It.IsAny<ExecutionContext>(), out It.Ref<object>.IsAny))
            .Returns((ExecutionContext ctx, out object item) =>
            {
                item = null;
                return false;
            });
        mockCacheManager.Setup(m => m.TryAddCache(It.IsAny<ExecutionContext>())).Returns(true);

        var middleware = CreateMiddleware(mockCacheManager.Object);
        var context = CreateContext(withTransaction: false, withCache: true);
        var nextCalled = false;

        var mockNext = new Mock<IMiddleware>();
        mockNext.Setup(m => m.InvokeAsync<string>(context)).Callback(() => nextCalled = true).Returns(Task.CompletedTask);
        middleware.Next = mockNext.Object;

        await middleware.InvokeAsync<string>(context);

        nextCalled.Should().BeTrue();
        mockCacheManager.Verify(m => m.TryAddCache(context), Times.Once);
    }

    [Fact]
    public async Task Should_InvokeNextAsync_When_TransactionExists()
    {
        var mockCacheManager = new Mock<ICacheManager>();
        var middleware = CreateMiddleware(mockCacheManager.Object);
        var context = CreateContext(withTransaction: true, withCache: true);
        var nextCalled = false;

        var mockNext = new Mock<IMiddleware>();
        mockNext.Setup(m => m.InvokeAsync<string>(context)).Callback(() => nextCalled = true).Returns(Task.CompletedTask);
        middleware.Next = mockNext.Object;

        await middleware.InvokeAsync<string>(context);

        nextCalled.Should().BeTrue();
        mockCacheManager.Verify(m => m.TryGetCache(It.IsAny<ExecutionContext>(), out It.Ref<object>.IsAny), Times.Never);
    }

    [Fact]
    public void Should_NotThrow_When_NextIsNull()
    {
        var mockCacheManager = new Mock<ICacheManager>();
        mockCacheManager.Setup(m => m.TryGetCache(It.IsAny<ExecutionContext>(), out It.Ref<object>.IsAny))
            .Returns((ExecutionContext ctx, out object item) =>
            {
                item = null;
                return false;
            });
        mockCacheManager.Setup(m => m.TryAddCache(It.IsAny<ExecutionContext>())).Returns(true);

        var middleware = CreateMiddleware(mockCacheManager.Object);
        var context = CreateContext(withTransaction: false, withCache: true);
        middleware.Next = null;

        var act = () => middleware.Invoke<string>(context);

        act.Should().NotThrow();
    }

    [Fact]
    public async Task Should_NotThrowAsync_When_NextIsNull()
    {
        var mockCacheManager = new Mock<ICacheManager>();
        mockCacheManager.Setup(m => m.TryGetCache(It.IsAny<ExecutionContext>(), out It.Ref<object>.IsAny))
            .Returns((ExecutionContext ctx, out object item) =>
            {
                item = null;
                return false;
            });
        mockCacheManager.Setup(m => m.TryAddCache(It.IsAny<ExecutionContext>())).Returns(true);

        var middleware = CreateMiddleware(mockCacheManager.Object);
        var context = CreateContext(withTransaction: false, withCache: true);
        middleware.Next = null;

        var act = async () => await middleware.InvokeAsync<string>(context);

        await act.Should().NotThrowAsync();
    }

    [Fact]
    public void Should_SetFromCacheFlag_When_CacheHit()
    {
        var cachedValue = "TestCachedData";
        var mockCacheManager = new Mock<ICacheManager>();
        mockCacheManager.Setup(m => m.TryGetCache(It.IsAny<ExecutionContext>(), out It.Ref<object>.IsAny))
            .Returns((ExecutionContext ctx, out object item) =>
            {
                item = cachedValue;
                return true;
            });

        var middleware = CreateMiddleware(mockCacheManager.Object);
        var context = CreateContext(withTransaction: false, withCache: true);
        var mockNext = new Mock<IMiddleware>();
        middleware.Next = mockNext.Object;

        middleware.Invoke<string>(context);

        context.Result.FromCache.Should().BeTrue();
    }

    [Fact]
    public void Should_AddToCache_When_CacheMissAndNoTransaction()
    {
        var mockCacheManager = new Mock<ICacheManager>();
        mockCacheManager.Setup(m => m.TryGetCache(It.IsAny<ExecutionContext>(), out It.Ref<object>.IsAny))
            .Returns((ExecutionContext ctx, out object item) =>
            {
                item = null;
                return false;
            });
        mockCacheManager.Setup(m => m.TryAddCache(It.IsAny<ExecutionContext>())).Returns(true);

        var middleware = CreateMiddleware(mockCacheManager.Object);
        var context = CreateContext(withTransaction: false, withCache: true);
        var mockNext = new Mock<IMiddleware>();
        middleware.Next = mockNext.Object;

        middleware.Invoke<string>(context);

        mockCacheManager.Verify(m => m.TryAddCache(context), Times.Once);
    }

    [Fact]
    public void Should_StillAddToCache_When_TransactionExists()
    {
        var mockCacheManager = new Mock<ICacheManager>();
        var middleware = CreateMiddleware(mockCacheManager.Object);
        var context = CreateContext(withTransaction: true, withCache: true);
        var mockNext = new Mock<IMiddleware>();
        middleware.Next = mockNext.Object;

        middleware.Invoke<string>(context);

        // TryGetCache is skipped due to short-circuit evaluation,
        // but TryAddCache is called in the else branch
        mockCacheManager.Verify(m => m.TryGetCache(It.IsAny<ExecutionContext>(), out It.Ref<object>.IsAny), Times.Never);
        mockCacheManager.Verify(m => m.TryAddCache(context), Times.Once);
    }
}
