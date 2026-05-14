using SmartSql.Test.Integration.Base;
using SmartSql.Test.Integration.Fixtures;
using Xunit;

namespace SmartSql.Test.Integration.MySql;

[Collection(MySqlFixture.CollectionName)]
public class MySqlDeserializerFactoryTests : DeserializerFactoryTestBase
{
    public MySqlDeserializerFactoryTests(MySqlFixture fixture) : base(fixture) { }
}
