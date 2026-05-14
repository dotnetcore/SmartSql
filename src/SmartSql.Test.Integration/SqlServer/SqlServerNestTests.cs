using SmartSql.Test.Integration.Base;
using SmartSql.Test.Integration.Fixtures;
using Xunit;

namespace SmartSql.Test.Integration.SqlServer;

[Collection(SqlServerFixture.CollectionName)]
public class SqlServerNestTests : NestTestBase
{
    public SqlServerNestTests(SqlServerFixture fixture) : base(fixture) { }
}
