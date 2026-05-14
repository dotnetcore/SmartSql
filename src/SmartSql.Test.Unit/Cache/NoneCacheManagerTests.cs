using FluentAssertions;
using SmartSql.Cache;
using Xunit;

namespace SmartSql.Test.Unit.Cache;

public class NoneCacheManagerTests
{
    private readonly NoneCacheManager _manager = new();

    [Fact]
    public void Should_ReturnFalse_When_TryGetCache()
    {
        var result = _manager.TryGetCache(null!, out var cacheItem);

        result.Should().BeFalse();
        cacheItem.Should().BeNull();
    }

    [Fact]
    public void Should_ReturnFalse_When_TryAddCache()
    {
        var result = _manager.TryAddCache(null!);

        result.Should().BeFalse();
    }

    [Fact]
    public void Should_NotThrow_When_DisposeCalled()
    {
        var action = () => _manager.Dispose();

        action.Should().NotThrow();
    }
}
