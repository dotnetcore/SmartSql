using System.Threading.Tasks;
using Xunit;

namespace SmartSql.Test.Integration.Deserializer
{
    public class DataTableDeserializerTest : IntegrationTestBase
    {
        public DataTableDeserializerTest(SmartSqlFixture fixture) : base(fixture) { }

        [Fact]
        public void GetDataTable()
        {
            var result = SqlMapper.GetDataTable(new RequestContext
            {
                Scope = "AllPrimitive",
                SqlId = "Query",
                Request = new { Taken = 10 }
            });
            Assert.NotNull(result);
        }

        [Fact]
        public async Task GetDataTableAsync()
        {
            var result = await SqlMapper.GetDataTableAsync(new RequestContext
            {
                Scope = "AllPrimitive",
                SqlId = "Query",
                Request = new { Taken = 10 }
            });
            Assert.NotNull(result);
        }
    }
}
