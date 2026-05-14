using SmartSql.Test.Integration.Base;
using SmartSql.Test.Integration.Fixtures;
using Xunit;

namespace SmartSql.Test.Integration.PostgreSql;

[Collection(PostgreSqlFixture.CollectionName)]
public class PgDbSessionTests : DbSessionTestBase
{
    public PgDbSessionTests(PostgreSqlFixture fixture) : base(fixture) { }
}
