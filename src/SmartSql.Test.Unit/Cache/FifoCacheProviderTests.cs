using System.Collections.Generic;
using FluentAssertions;
using SmartSql.Cache;
using SmartSql.Cache.Default;
using Xunit;

namespace SmartSql.Test.Unit.Cache;

public class FifoCacheProviderTests
{
    private FifoCacheProvider CreateProvider(int cacheSize = 3)
    {
        var provider = new FifoCacheProvider();
        provider.Initialize(new Dictionary<string, object> {{"CacheSize", cacheSize}});
        return provider;
    }

    private CacheKey CreateKey(string key) => new CacheKey(key, typeof(string));

    [Fact]
    public void Should_AddAndGetItem_When_CacheIsEmpty()
    {
        using var provider = CreateProvider();
        var key = CreateKey("key1");

        provider.TryAdd(key, "value1");
        var found = provider.TryGetValue(key, out var item);

        found.Should().BeTrue();
        item.Should().Be("value1");
    }

    [Fact]
    public void Should_ReturnFalse_When_KeyNotFound()
    {
        using var provider = CreateProvider();
        var key = CreateKey("nonexistent");

        var found = provider.TryGetValue(key, out var item);

        found.Should().BeFalse();
        item.Should().BeNull();
    }

    [Fact]
    public void Should_ReturnFalse_When_AddingDuplicateKey()
    {
        using var provider = CreateProvider();
        var key = CreateKey("key1");

        provider.TryAdd(key, "value1");
        var result = provider.TryAdd(key, "value2");

        result.Should().BeFalse();

        provider.TryGetValue(key, out var item);
        item.Should().Be("value1");
    }

    [Fact]
    public void Should_EvictOldest_When_CapacityExceeded()
    {
        using var provider = CreateProvider(cacheSize: 2);
        var key1 = CreateKey("key1");
        var key2 = CreateKey("key2");
        var key3 = CreateKey("key3");

        provider.TryAdd(key1, "value1");
        provider.TryAdd(key2, "value2");
        provider.TryAdd(key3, "value3");

        provider.TryGetValue(key1, out _).Should().BeFalse();
        provider.TryGetValue(key2, out _).Should().BeTrue();
        provider.TryGetValue(key3, out _).Should().BeTrue();
    }

    [Fact]
    public void Should_ClearAllItems_When_FlushCalled()
    {
        using var provider = CreateProvider();
        var key1 = CreateKey("key1");
        var key2 = CreateKey("key2");

        provider.TryAdd(key1, "value1");
        provider.TryAdd(key2, "value2");

        provider.Flush();

        provider.TryGetValue(key1, out _).Should().BeFalse();
        provider.TryGetValue(key2, out _).Should().BeFalse();
    }
}
