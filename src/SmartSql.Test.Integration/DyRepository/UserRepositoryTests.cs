using FluentAssertions;
using SmartSql.Data;
using SmartSql.Test.Repositories;
using Xunit;

namespace SmartSql.Test.Integration.DyRepository
{
    public class UserRepositoryTest : IntegrationTestBase
    {
        private readonly IUserRepository _userRepository;

        public UserRepositoryTest(SmartSqlFixture fixture) : base(fixture)
        {
            _userRepository = fixture.UserRepository;
        }

        [EnvironmentFact(exclude: EnvironmentFactAttribute.GITHUB_ACTION)]
        public void SP_Query()
        {
            SqlParameterCollection dbParameterCollection = new SqlParameterCollection();
            dbParameterCollection.Add(new SqlParameter("Total", null, typeof(int))
            {
                DbType = System.Data.DbType.Int32,
                Direction = System.Data.ParameterDirection.Output
            });
            var list = _userRepository.SP_Query(dbParameterCollection);
            Assert.NotNull(list);
            dbParameterCollection.TryGetParameterValue("Total", out int total);
            Assert.NotEqual(0, total);
        }
    }
}
