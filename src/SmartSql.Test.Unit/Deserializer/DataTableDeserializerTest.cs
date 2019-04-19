using SmartSql.Test.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace SmartSql.Test.Unit.Deserializer
{
    [Collection("GlobalSmartSql")]
    public class DataTableDeserializerTest 
    {
        protected ISqlMapper SqlMapper { get; }

        public DataTableDeserializerTest(SmartSqlFixture smartSqlFixture)
        {
            SqlMapper = smartSqlFixture.SqlMapper;
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
