using FluentAssertions;
using SmartSql.IdGenerator;
using SmartSql.Test.Entities;
using SmartSql.Test.Integration.Fixtures;
using Xunit;

namespace SmartSql.Test.Integration.MySql;

[Collection(MySqlFixture.CollectionName)]
public class MySqlJsonTypeTests : IntegrationTestBase
{
    public MySqlJsonTypeTests(MySqlFixture fixture) : base(fixture) { }

    [Fact]
    public void Should_ReturnId_When_Insert()
    {
        var id = InsertImpl();
        id.Should().BeGreaterThan(0);
    }

    private long InsertImpl(UserExtendedInfo userExtendedInfo = null)
    {
        userExtendedInfo ??= NewUserExtendedInfo();
        SqlMapper.Execute(new RequestContext
        {
            Scope = nameof(UserExtendedInfo),
            SqlId = "Insert",
            Request = userExtendedInfo
        });
        return userExtendedInfo.UserId;
    }

    private UserExtendedInfo NewUserExtendedInfo()
    {
        var id = SnowflakeId.Default.NextId();
        return new UserExtendedInfo
        {
            UserId = id,
            Data = new UserInfo
            {
                Height = 188,
                Weight = 168
            }
        };
    }

    [Fact]
    public void Should_ReturnEntity_When_GetById()
    {
        var insertUserExtendedInfo = NewUserExtendedInfo();
        var userId = InsertImpl(insertUserExtendedInfo);
        var userExtendedInfo = SqlMapper.QuerySingle<UserExtendedInfo>(new RequestContext
        {
            Scope = nameof(UserExtendedInfo),
            SqlId = "GetEntity",
            Request = new { UserId = userId }
        });
        insertUserExtendedInfo.UserId.Should().Be(userId);
        userExtendedInfo.Data.Height.Should().Be(insertUserExtendedInfo.Data.Height);
        userExtendedInfo.Data.Weight.Should().Be(insertUserExtendedInfo.Data.Weight);
    }
}
