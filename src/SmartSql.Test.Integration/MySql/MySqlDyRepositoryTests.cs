using SmartSql.Test.Integration.Base;
using SmartSql.Test.Integration.Fixtures;
using Xunit;

namespace SmartSql.Test.Integration.MySql;

[Collection(MySqlFixture.CollectionName)]
public class MySqlDyRepositoryTests : DyRepositoryTestBase
{
    public MySqlDyRepositoryTests(MySqlFixture fixture) : base(fixture) { }
}
