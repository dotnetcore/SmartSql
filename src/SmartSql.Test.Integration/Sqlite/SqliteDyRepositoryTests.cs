using SmartSql.Test.Integration.Base;
using SmartSql.Test.Integration.Fixtures;
using Xunit;

namespace SmartSql.Test.Integration.Sqlite;

[Collection(SqliteFixture.CollectionName)]
public class SqliteDyRepositoryTests : DyRepositoryTestBase
{
    public SqliteDyRepositoryTests(SqliteFixture fixture) : base(fixture) { }
}
