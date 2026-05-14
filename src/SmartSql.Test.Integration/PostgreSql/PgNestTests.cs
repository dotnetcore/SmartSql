using SmartSql.Test.Integration.Base;
using SmartSql.Test.Integration.Fixtures;
using Xunit;

namespace SmartSql.Test.Integration.PostgreSql;

[Collection(PostgreSqlFixture.CollectionName)]
public class PgNestTests : NestTestBase
{
    public PgNestTests(PostgreSqlFixture fixture) : base(fixture) { }
}
