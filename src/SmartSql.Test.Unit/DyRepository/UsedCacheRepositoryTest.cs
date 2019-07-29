using System.Threading;
using SmartSql.DyRepository;
using SmartSql.Test.Repositories;
using Microsoft.Extensions.Logging;
using Xunit;

namespace SmartSql.Test.Unit.DyRepository
{
    [Collection("GlobalSmartSql")]
    public class UsedCacheRepositoryTest
    {
        private IUsedCacheRepository  _usedCacheRepository;
        
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
            var user = _usedCacheRepository.GetUserById(1);
            var user1 = _usedCacheRepository.GetUserById(1);
            Assert.Equal(user, user1);
        }

        [Fact]
        public void FlushOnExecute()
        {
            var user = _usedCacheRepository.GetUserById(2);
            _usedCacheRepository.UpdateUserName(2, "SmartSql");
            var user1 = _usedCacheRepository.GetUserById(2);
            Assert.NotEqual(user, user1);
        }

        [Fact]
        public void GetId()
        {
            var id = _usedCacheRepository.GetId(1);
            var id1 = _usedCacheRepository.GetId(1);
            Assert.Equal(id, id1);
        }

        [Fact]
        public void UpdateUserName()
        {
            var affected = _usedCacheRepository.UpdateUserName(1, "SmartSql");
            Assert.True(affected > 0);
        }
    }
}