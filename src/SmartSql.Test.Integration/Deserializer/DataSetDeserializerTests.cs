using FluentAssertions;
using System.Threading.Tasks;
using SmartSql.Test.Entities;
using Xunit;

namespace SmartSql.Test.Integration.Deserializer;

public class DataSetDeserializerTests : IntegrationTestBase
{
    public DataSetDeserializerTests(SmartSqlFixture fixture) : base(fixture) { }

    [Fact]
    public void Should_ReturnDataSet_When_GetDataSet()
    {
        SqlMapper.Insert<AllPrimitive, long>(new AllPrimitive());
        var result = SqlMapper.GetDataSet(new RequestContext
        {
            Scope = nameof(AllPrimitive),
            SqlId = "GetDataSet"
        });
        result.Should().NotBeNull();
        result.Tables.Count.Should().Be(2);
    }

    [Fact]
    public async Task Should_ReturnDataSet_When_GetDataSetAsync()
    {
        var result = await SqlMapper.GetDataSetAsync(new RequestContext
        {
            Scope = nameof(AllPrimitive),
            SqlId = "GetDataSet"
        });
        result.Should().NotBeNull();
        result.Tables.Count.Should().Be(2);
    }
}
