using System.Threading;
using SmartSql.Test.Entities;
using SmartSql.Test.Repositories;
using Xunit;

namespace SmartSql.Test.Integration.DyRepository
{
    public class UsedCacheRepositoryTest : IntegrationTestBase
    {
        private readonly IUsedCacheRepository _usedCacheRepository;

        public UsedCacheRepositoryTest(SmartSqlFixture fixture) : base(fixture)
        {
            _usedCacheRepository = fixture.UsedCacheRepository;
        }

        [Fact]
        public void GetNow()
        {
            var datetime = _usedCacheRepository.GetNow();
            Thread.Sleep(2000);
            var datetime1 = _usedCacheRepository.GetNow();
            Assert.Equal(datetime, datetime1);
        }

        [Fact]
        public void GetUserById()
        {
            var userId = _usedCacheRepository.Insert(new User { UserName = "SmartSql", Status = UserStatus.Ok });
            var user = _usedCacheRepository.GetUserById(userId);
            var user1 = _usedCacheRepository.GetUserById(userId);
            Assert.Equal(user, user1);
        }

        [Fact]
        public void FlushOnExecute()
        {
            var userId = _usedCacheRepository.Insert(new User { UserName = "SmartSql", Status = UserStatus.Ok });
            var user = _usedCacheRepository.GetUserById(userId);
            _usedCacheRepository.UpdateUserName(userId, "SmartSql");
            var user1 = _usedCacheRepository.GetUserById(userId);
            Assert.NotEqual(user, user1);
        }

        [Fact]
        public void GetId()
        {
            var userId = _usedCacheRepository.Insert(new User { UserName = "SmartSql", Status = UserStatus.Ok });
            var id = _usedCacheRepository.GetId(userId);
            var id1 = _usedCacheRepository.GetId(userId);
            Assert.Equal(id, id1);
        }

        [Fact]
        public void UpdateUserName()
        {
            var userId = _usedCacheRepository.Insert(new User { UserName = "SmartSql", Status = UserStatus.Ok });
            var affected = _usedCacheRepository.UpdateUserName(userId, "SmartSql");
            Assert.True(affected > 0);
        }
    }
}
