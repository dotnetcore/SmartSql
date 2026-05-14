using FluentAssertions;
using SmartSql.Configuration;
using Xunit;

namespace SmartSql.Test.Integration;

[Collection("GlobalSmartSql")]
public abstract class IntegrationTestBase
{
    protected ISqlMapper SqlMapper { get; }
    protected SmartSqlFixture Fixture { get; }
    protected SmartSqlConfig SmartSqlConfig => SqlMapper.SmartSqlConfig;

    protected IntegrationTestBase(SmartSqlFixture fixture)
    {
        Fixture = fixture;
        SqlMapper = fixture.SqlMapper;
    }
}
