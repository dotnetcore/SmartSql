using SmartSql.Test.Integration.Base;
using SmartSql.Test.Integration.Fixtures;
using Xunit;

namespace SmartSql.Test.Integration.Sqlite;

[Collection(SqliteFixture.CollectionName)]
public class SqliteDeserializerFactoryTests : DeserializerFactoryTestBase
{
    public SqliteDeserializerFactoryTests(SqliteFixture fixture) : base(fixture) { }
}
