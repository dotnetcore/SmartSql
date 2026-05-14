using System.Threading;
using FluentAssertions;
using SmartSql.Test.Entities;
using SmartSql.Test.Repositories;
using Xunit;

namespace SmartSql.Test.Integration.DyRepository;

public class UsedCacheRepositoryTests : IntegrationTestBase
{
    private readonly IUsedCacheRepository _usedCacheRepository;

    public UsedCacheRepositoryTests(SmartSqlFixture fixture) : base(fixture)
    {
        _usedCacheRepository = fixture.UsedCacheRepository;
    }

    [Fact]
    public void Should_ReturnCachedDateTime_When_CacheIsEnabled()
    {
        var datetime = _usedCacheRepository.GetNow();
        Thread.Sleep(2000);
        var datetime1 = _usedCacheRepository.GetNow();
        datetime1.Should().Be(datetime);
    }

    [Fact]
    public void Should_ReturnSameUser_When_CacheIsHit()
    {
        var userId = _usedCacheRepository.Insert(new User { UserName = "SmartSql", Status = UserStatus.Ok });
        var user = _usedCacheRepository.GetUserById(userId);
        var user1 = _usedCacheRepository.GetUserById(userId);
        user1.Should().Be(user);
    }

    [Fact]
    public void Should_InvalidateCache_When_FlushOnExecute()
    {
        var userId = _usedCacheRepository.Insert(new User { UserName = "SmartSql", Status = UserStatus.Ok });
        var user = _usedCacheRepository.GetUserById(userId);
        _usedCacheRepository.UpdateUserName(userId, "SmartSql");
        var user1 = _usedCacheRepository.GetUserById(userId);
        user1.Should().NotBe(user);
    }

    [Fact]
    public void Should_ReturnCachedId_When_CacheIsHit()
    {
        var userId = _usedCacheRepository.Insert(new User { UserName = "SmartSql", Status = UserStatus.Ok });
        var id = _usedCacheRepository.GetId(userId);
        var id1 = _usedCacheRepository.GetId(userId);
        id1.Should().Be(id);
    }

    [Fact]
    public void Should_AffectRows_When_UpdatingUserName()
    {
        var userId = _usedCacheRepository.Insert(new User { UserName = "SmartSql", Status = UserStatus.Ok });
        var affected = _usedCacheRepository.UpdateUserName(userId, "SmartSql");
        (affected > 0).Should().BeTrue();
    }
}
