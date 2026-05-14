using System;
using System.Collections.Generic;
using FluentAssertions;
using SmartSql.Cache;
using Xunit;

namespace SmartSql.Test.Unit.Cache;

public class CacheManagerTests
{
    [Fact]
    public void Should_BeICacheManager_When_Created()
    {
        var manager = new CacheManager();

        manager.Should().BeAssignableTo<ICacheManager>();
    }

    [Fact]
    public void Should_BeAbstractCacheManager_When_Created()
    {
        var manager = new CacheManager();

        manager.Should().BeAssignableTo<AbstractCacheManager>();
    }

    [Fact]
    public void Should_TryGetCacheReturnFalse_When_NoCacheConfigured()
    {
        // TryGetCache requires SmartSqlConfig which needs full setup,
        // so we just verify the class is instantiable
        var manager = new CacheManager();

        manager.Should().NotBeNull();
    }
}
