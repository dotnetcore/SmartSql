using SmartSql.Test.Integration.Base;
using SmartSql.Test.Integration.Fixtures;
using Xunit;

namespace SmartSql.Test.Integration.PostgreSql;

[Collection(PostgreSqlFixture.CollectionName)]
public class PgCUDTests : CUDTestBase
{
    public PgCUDTests(PostgreSqlFixture fixture) : base(fixture) { }
}
