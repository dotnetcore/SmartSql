using System;
using System.Collections.Generic;
using FluentAssertions;
using SmartSql.Cache;
using SmartSql.Cache.Default;
using SmartSql.Configuration;
using Xunit;

namespace SmartSql.Test.Unit.Cache;

public class CacheProviderUtilTests
{
    [Fact]
    public void Should_ReturnLruCacheProvider_When_TypeIsLru()
    {
        var cache = new SmartSql.Configuration.Cache
        {
            Type = "Lru",
            Parameters = new Dictionary<string, object>()
        };

        var provider = CacheProviderUtil.Create(cache);

        provider.Should().BeOfType<LruCacheProvider>();
    }

    [Fact]
    public void Should_ReturnFifoCacheProvider_When_TypeIsFifo()
    {
        var cache = new SmartSql.Configuration.Cache
        {
            Type = "Fifo",
            Parameters = new Dictionary<string, object>()
        };

        var provider = CacheProviderUtil.Create(cache);

        provider.Should().BeOfType<FifoCacheProvider>();
    }

    [Fact]
    public void Should_ThrowException_When_TypeDoesNotMatch()
    {
        var cache = new SmartSql.Configuration.Cache
        {
            Type = "LRU",
            Parameters = new Dictionary<string, object>()
        };

        var act = () => CacheProviderUtil.Create(cache);

        act.Should().Throw<Exception>();
    }

    [Fact]
    public void Should_InitializeProviderWithParameters_When_TypeIsLru()
    {
        var cache = new SmartSql.Configuration.Cache
        {
            Type = "Lru",
            Parameters = new Dictionary<string, object>
            {
                ["CacheSize"] = 50
            }
        };

        var provider = CacheProviderUtil.Create(cache);

        provider.Should().NotBeNull();
    }

    [Fact]
    public void Should_InitializeProviderWithParameters_When_TypeIsFifo()
    {
        var cache = new SmartSql.Configuration.Cache
        {
            Type = "Fifo",
            Parameters = new Dictionary<string, object>
            {
                ["CacheSize"] = 25
            }
        };

        var provider = CacheProviderUtil.Create(cache);

        provider.Should().NotBeNull();
    }
}
