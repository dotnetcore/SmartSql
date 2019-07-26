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
        private IUsedCacheRepository _userRepository;

        public UsedCacheRepositoryTest(SmartSqlFixture smartSqlFixture)
        {
            var repositoryBuilder = new EmitRepositoryBuilder(null, null,
                smartSqlFixture.LoggerFactory.CreateLogger<EmitRepositoryBuilder>());
            var repositoryFactory = new RepositoryFactory(repositoryBuilder,
                smartSqlFixture.LoggerFactory.CreateLogger<RepositoryFactory>());
            _userRepository =
                repositoryFactory.CreateInstance(typeof(IUsedCacheRepository), smartSqlFixture.SqlMapper) as
                    IUsedCacheRepository;
        }

        [Fact]
        public void GetNow()
        {
            var datetime = _userRepository.GetNow();
            Thread.Sleep(1000);
            var datetime1 = _userRepository.GetNow();
            Assert.Equal(datetime, datetime1);
        }

        [Fact]
        public void GetId()
        {
            var id = _userRepository.GetId(1);
            var id1 = _userRepository.GetId(1);
            Assert.Equal(id, id1);
        }

        [Fact]
        public void GetUserById()
        {
            var id = _userRepository.GetUserById(1);
            var id1 = _userRepository.GetUserById(1);
            Assert.Equal(id, id1);
        }

        [Fact]
        public void UpdateUserName()
        {
            var affected = _userRepository.UpdateUserName(1, "SmartSql");
            Assert.True(affected > 0);
        }
    }
}