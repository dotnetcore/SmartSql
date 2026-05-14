using FluentAssertions;
using System.Threading.Tasks;
using Xunit;

namespace SmartSql.Test.Integration.Deserializer;

public class DataTableDeserializerTests : IntegrationTestBase
{
    public DataTableDeserializerTests(SmartSqlFixture fixture) : base(fixture) { }

    [Fact]
    public void Should_ReturnDataTable_When_GetDataTable()
    {
        var result = SqlMapper.GetDataTable(new RequestContext
        {
            Scope = "AllPrimitive",
            SqlId = "Query",
            Request = new { Taken = 10 }
        });
        result.Should().NotBeNull();
    }

    [Fact]
    public async Task Should_ReturnDataTable_When_GetDataTableAsync()
    {
        var result = await SqlMapper.GetDataTableAsync(new RequestContext
        {
            Scope = "AllPrimitive",
            SqlId = "Query",
            Request = new { Taken = 10 }
        });
        result.Should().NotBeNull();
    }
}
