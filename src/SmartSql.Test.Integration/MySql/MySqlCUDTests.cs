using SmartSql.Test.Integration.Base;
using SmartSql.Test.Integration.Fixtures;
using Xunit;

namespace SmartSql.Test.Integration.MySql;

[Collection(MySqlFixture.CollectionName)]
public class MySqlCUDTests : CUDTestBase
{
    public MySqlCUDTests(MySqlFixture fixture) : base(fixture) { }
}
