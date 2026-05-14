using FluentAssertions;
using SmartSql.Cache;
using SmartSql.Cache.Default;
using Xunit;

namespace SmartSql.Test.Unit.Cache;

public class NoneCacheProviderTests
{
    private readonly NoneCacheProvider _provider = new();

    private CacheKey CreateKey(string key) => new CacheKey(key, typeof(string));

    [Fact]
    public void Should_ReturnTrue_When_TryAdd()
    {
        var key = CreateKey("key1");

        var result = _provider.TryAdd(key, "value1");

        result.Should().BeTrue();
    }

    [Fact]
    public void Should_ReturnFalse_When_TryGetValue()
    {
        var key = CreateKey("key1");

        var result = _provider.TryGetValue(key, out var item);

        result.Should().BeFalse();
        item.Should().BeNull();
    }

    [Fact]
    public void Should_ReturnTrue_When_SupportExpire()
    {
        _provider.SupportExpire.Should().BeTrue();
    }

    [Fact]
    public void Should_ReturnFalse_When_TryGetValueAfterAdd()
    {
        var key = CreateKey("key1");
        _provider.TryAdd(key, "value1");

        var result = _provider.TryGetValue(key, out var item);

        result.Should().BeFalse();
        item.Should().BeNull();
    }
}
