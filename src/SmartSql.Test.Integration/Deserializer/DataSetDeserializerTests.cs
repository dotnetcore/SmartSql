using FluentAssertions;
using System.Threading.Tasks;
using SmartSql.Test.Entities;
using Xunit;

namespace SmartSql.Test.Integration.Deserializer
{
    public class DataSetDeserializerTest : IntegrationTestBase
    {
        public DataSetDeserializerTest(SmartSqlFixture fixture) : base(fixture) { }

        [Fact]
        public void GetDataSet()
        {
            SqlMapper.Insert<AllPrimitive, long>(new AllPrimitive());
            var result = SqlMapper.GetDataSet(new RequestContext
            {
                Scope = nameof(AllPrimitive),
                SqlId = "GetDataSet"
            });
            Assert.NotNull(result);
            Assert.Equal(2, result.Tables.Count);
        }

        [Fact]
        public async Task GetDataSetAsync()
        {
            var result = await SqlMapper.GetDataSetAsync(new RequestContext
            {
                Scope = nameof(AllPrimitive),
                SqlId = "GetDataSet"
            });
            Assert.NotNull(result);
            Assert.Equal(2, result.Tables.Count);
        }
    }
}
