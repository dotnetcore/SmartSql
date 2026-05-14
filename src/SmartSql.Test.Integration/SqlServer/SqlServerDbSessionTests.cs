using SmartSql.Test.Integration.Base;
using SmartSql.Test.Integration.Fixtures;
using Xunit;

namespace SmartSql.Test.Integration.SqlServer;

[Collection(SqlServerFixture.CollectionName)]
public class SqlServerDbSessionTests : DbSessionTestBase
{
    public SqlServerDbSessionTests(SqlServerFixture fixture) : base(fixture) { }
}
