using SmartSql.Test.Integration.Base;
using SmartSql.Test.Integration.Fixtures;
using Xunit;

namespace SmartSql.Test.Integration.PostgreSql;

[Collection(PostgreSqlFixture.CollectionName)]
public class PgDyRepositoryTests : DyRepositoryTestBase
{
    public PgDyRepositoryTests(PostgreSqlFixture fixture) : base(fixture) { }
}
