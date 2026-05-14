using FluentAssertions;
using SmartSql.Data;
using SmartSql.Test.Repositories;
using Xunit;

namespace SmartSql.Test.Integration.DyRepository;

public class UserRepositoryTests : IntegrationTestBase
{
    private readonly IUserRepository _userRepository;

    public UserRepositoryTests(SmartSqlFixture fixture) : base(fixture)
    {
        _userRepository = fixture.UserRepository;
    }

    [EnvironmentFact(exclude: EnvironmentFactAttribute.GITHUB_ACTION)]
    public void Should_ReturnResults_When_CallingStoredProcedure()
    {
        SqlParameterCollection dbParameterCollection = new SqlParameterCollection();
        dbParameterCollection.Add(new SqlParameter("Total", null, typeof(int))
        {
            DbType = System.Data.DbType.Int32,
            Direction = System.Data.ParameterDirection.Output
        });
        var list = _userRepository.SP_Query(dbParameterCollection);
        list.Should().NotBeNull();
        dbParameterCollection.TryGetParameterValue("Total", out int total);
        total.Should().BeGreaterThan(0);
    }
}
