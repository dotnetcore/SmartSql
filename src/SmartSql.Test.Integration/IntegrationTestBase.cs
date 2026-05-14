using FluentAssertions;
using SmartSql.Configuration;
using SmartSql.Test.Integration.Fixtures;
using Xunit;

namespace SmartSql.Test.Integration;

public abstract class IntegrationTestBase
{
    protected ISqlMapper SqlMapper { get; }
    protected IDbTestFixture Fixture { get; }
    protected SmartSqlConfig SmartSqlConfig => SqlMapper.SmartSqlConfig;
    protected string DbProvider => Fixture.DbProvider;

    protected IntegrationTestBase(IDbTestFixture fixture)
    {
        Fixture = fixture;
        SqlMapper = fixture.SqlMapper;
    }

    protected string SelectTopAllPrimitive(int count)
    {
        var top = DbProvider is "SqlServer" or "MsSqlServer"
            ? $"TOP {count}"
            : "";
        var limit = DbProvider is not ("SqlServer" or "MsSqlServer")
            ? $" limit {count}"
            : "";
        return $"SELECT {top} T.* From T_AllPrimitive T{limit}";
    }
}
