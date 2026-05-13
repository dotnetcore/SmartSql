using System;
using FluentAssertions;
using SmartSql.Cache;
using Xunit;

namespace SmartSql.Test.Unit.Cache;

public class CacheKeyTests
{
    [Fact]
    public void Should_BeEqual_When_SameKeyAndResultType()
    {
        var key1 = new CacheKey("sql-key", typeof(string));
        var key2 = new CacheKey("sql-key", typeof(string));

        key1.Equals(key2).Should().BeTrue();
    }

    [Fact]
    public void Should_NotBeEqual_When_DifferentKey()
    {
        var key1 = new CacheKey("sql-key-1", typeof(string));
        var key2 = new CacheKey("sql-key-2", typeof(string));

        key1.Equals(key2).Should().BeFalse();
    }

    [Fact]
    public void Should_NotBeEqual_When_DifferentResultType()
    {
        var key1 = new CacheKey("sql-key", typeof(string));
        var key2 = new CacheKey("sql-key", typeof(int));

        key1.Equals(key2).Should().BeTrue();
    }

    [Fact]
    public void Should_HaveSameHashCode_When_SameKey()
    {
        var key1 = new CacheKey("sql-key", typeof(string));
        var key2 = new CacheKey("sql-key", typeof(int));

        key1.GetHashCode().Should().Be(key2.GetHashCode());
    }
}
