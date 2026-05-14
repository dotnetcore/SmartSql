using SmartSql.Test.Integration.Base;
using SmartSql.Test.Integration.Fixtures;
using Xunit;

namespace SmartSql.Test.Integration.SqlServer;

[Collection(SqlServerFixture.CollectionName)]
public class SqlServerSqlMapperTests : SqlMapperTestBase
{
    public SqlServerSqlMapperTests(SqlServerFixture fixture) : base(fixture) { }
}
