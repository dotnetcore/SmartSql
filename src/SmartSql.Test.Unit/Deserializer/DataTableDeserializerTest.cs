using SmartSql.Test.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace SmartSql.Test.Unit.Deserializer
{
    public class DataTableDeserializerTest : AbstractXmlConfigBuilderTest
    {
        protected ISqlMapper SqlMapper { get; }
        public DataTableDeserializerTest()
        {
            SqlMapper = BuildSqlMapper(this.GetType().FullName);
        }
        [Fact]
        public void GetDataTable()
        {
            var result = SqlMapper.GetDataTable(new RequestContext
            {
                Scope = nameof(AllPrimitive),
                SqlId = "Query",
                Request = new { Taken = 100 }
            });

        }
        [Fact]
        public async Task GetDataTableAsync()
        {
            var result = await SqlMapper.GetDataTableAsync(new RequestContext
            {
                Scope = nameof(AllPrimitive),
                SqlId = "Query",
                Request = new { Taken = 100 }
            });
        }
    }
}
