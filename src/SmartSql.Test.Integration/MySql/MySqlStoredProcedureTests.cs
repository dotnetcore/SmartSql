using System.Data;
using FluentAssertions;
using SmartSql.Data;
using SmartSql.Test.Entities;
using SmartSql.Test.Integration.Fixtures;
using Xunit;
using MySqlDataClient = MySql.Data.MySqlClient;

namespace SmartSql.Test.Integration.MySql;

[Collection(MySqlFixture.CollectionName)]
public class MySqlStoredProcedureTests : IntegrationTestBase
{
    public MySqlStoredProcedureTests(MySqlFixture fixture) : base(fixture) { }

    [EnvironmentFact(exclude: EnvironmentFactAttribute.GITHUB_ACTION)]
    public void Should_ExecuteStoredProcedure()
    {
        SqlParameterCollection dbParams = new SqlParameterCollection();
        dbParams.Add(new SqlParameter
        {
            Name = "Total", DbType = DbType.Int32, Direction = ParameterDirection.Output
        });
        var context = new RequestContext
        {
            CommandType = CommandType.StoredProcedure, RealSql = "SP_Query", Request = dbParams
        };
        var list = SqlMapper.Query<AllPrimitive>(context);
        dbParams.TryGetParameterValue("Total", out int total);
    }

    [EnvironmentFact(exclude: EnvironmentFactAttribute.GITHUB_ACTION)]
    public void Should_ExecuteStoredProcedure_When_SourceParameterProvided()
    {
        SqlParameterCollection dbParams = new SqlParameterCollection();
        dbParams.Add(new SqlParameter("Total", null)
        {
            SourceParameter = new MySqlDataClient.MySqlParameter("Total", DbType.Int32)
            {
                Direction = ParameterDirection.Output
            }
        });
        var context = new RequestContext
        {
            CommandType = CommandType.StoredProcedure, RealSql = "SP_Query", Request = dbParams
        };
        var list = SqlMapper.Query<AllPrimitive>(context);
        list.Should().NotBeNull();
        dbParams.TryGetParameterValue("Total", out int total);
    }
}
