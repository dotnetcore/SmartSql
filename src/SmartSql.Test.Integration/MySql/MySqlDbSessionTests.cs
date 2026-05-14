using SmartSql.Test.Integration.Base;
using SmartSql.Test.Integration.Fixtures;
using Xunit;

namespace SmartSql.Test.Integration.MySql;

[Collection(MySqlFixture.CollectionName)]
public class MySqlDbSessionTests : DbSessionTestBase
{
    public MySqlDbSessionTests(MySqlFixture fixture) : base(fixture) { }
}
