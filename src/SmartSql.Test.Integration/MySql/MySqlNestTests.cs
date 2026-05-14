using SmartSql.Test.Integration.Base;
using SmartSql.Test.Integration.Fixtures;
using Xunit;

namespace SmartSql.Test.Integration.MySql;

[Collection(MySqlFixture.CollectionName)]
public class MySqlNestTests : NestTestBase
{
    public MySqlNestTests(MySqlFixture fixture) : base(fixture) { }
}
