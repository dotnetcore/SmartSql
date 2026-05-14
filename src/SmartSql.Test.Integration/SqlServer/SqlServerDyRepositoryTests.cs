using SmartSql.Test.Integration.Base;
using SmartSql.Test.Integration.Fixtures;
using Xunit;

namespace SmartSql.Test.Integration.SqlServer;

[Collection(SqlServerFixture.CollectionName)]
public class SqlServerDyRepositoryTests : DyRepositoryTestBase
{
    public SqlServerDyRepositoryTests(SqlServerFixture fixture) : base(fixture) { }
}
