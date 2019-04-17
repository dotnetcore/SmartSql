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
        [Fact]
        public void GetDataTable()
        {
            var result = DbSession.GetDataTable(new RequestContext
            {
                Scope = nameof(AllPrimitive),
                SqlId = "Query",
                Request = new { Taken = 100 }
            });

        }
        [Fact]
        public async Task GetDataTableAsync()
        {
            var result = await DbSession.GetDataTableAsync(new RequestContext
            {
                Scope = nameof(AllPrimitive),
                SqlId = "Query",
                Request = new { Taken = 100 }
            });
        }
    }
}
