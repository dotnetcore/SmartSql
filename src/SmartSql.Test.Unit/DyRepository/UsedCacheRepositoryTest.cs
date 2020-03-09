using System.Threading;
using SmartSql.DyRepository;
using SmartSql.Test.Repositories;
using Microsoft.Extensions.Logging;
using SmartSql.Test.Entities;
using Xunit;

namespace SmartSql.Test.Unit.DyRepository
{
    [Collection("GlobalSmartSql")]
    public class UsedCacheRepositoryTest
    {
        private IUsedCacheRepository _usedCacheRepository;

        public UsedCacheRepositoryTest(SmartSqlFixture smartSqlFixture)
        {
            _usedCacheRepository = smartSqlFixture.UsedCacheRepository;
        }

        [Fact]
        public void GetNow()
        {
            
            var datetime = _usedCacheRepository.GetNow();
            Thread.Sleep(1000);
            var datetime1 = _usedCacheRepository.GetNow();
            Assert.Equal(datetime, datetime1);
        }

        [Fact]
        public void GetUserById()
        {
            var userId = _usedCacheRepository.Insert(new User {UserName = "SmartSql", Status = UserStatus.Ok});
            var user = _usedCacheRepository.GetUserById(userId);
            var user1 = _usedCacheRepository.GetUserById(userId);
            Assert.Equal(user, user1);
        }

        [Fact]
        public void FlushOnExecute()
        {
            var userId = _usedCacheRepository.Insert(new User {UserName = "SmartSql", Status = UserStatus.Ok});
            var user = _usedCacheRepository.GetUserById(userId);
            _usedCacheRepository.UpdateUserName(userId, "SmartSql");
            var user1 = _usedCacheRepository.GetUserById(userId);
            Assert.NotEqual(user, user1);
        }

        [Fact]
        public void GetId()
        {
            var userId = _usedCacheRepository.Insert(new User {UserName = "SmartSql", Status = UserStatus.Ok});
            var id = _usedCacheRepository.GetId(userId);
            var id1 = _usedCacheRepository.GetId(userId);
            Assert.Equal(id, id1);
        }

        [Fact]
        public void UpdateUserName()
        {
            var userId = _usedCacheRepository.Insert(new User {UserName = "SmartSql", Status = UserStatus.Ok});
            var affected = _usedCacheRepository.UpdateUserName(userId, "SmartSql");
            Assert.True(affected > 0);
        }
    }
}