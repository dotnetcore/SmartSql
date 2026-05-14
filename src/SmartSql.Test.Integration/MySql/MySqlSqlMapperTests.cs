using SmartSql.Test.Integration.Base;
using SmartSql.Test.Integration.Fixtures;
using Xunit;

namespace SmartSql.Test.Integration.MySql;

[Collection(MySqlFixture.CollectionName)]
public class MySqlSqlMapperTests : SqlMapperTestBase
{
    public MySqlSqlMapperTests(MySqlFixture fixture) : base(fixture) { }
}
