using System;
using System.Collections.Generic;
using System.Text;
using SmartSql.Test.DTO;
using SmartSql.Test.Entities;
using Xunit;

namespace SmartSql.Test.Unit.Deserializer
{
    [Collection("GlobalSmartSql")]
    public class ValueTupleDeserializerTest 
    {
        protected ISqlMapper SqlMapper { get; }

        public ValueTupleDeserializerTest(SmartSqlFixture smartSqlFixture)
        {
            SqlMapper = smartSqlFixture.SqlMapper;
        }
        [Fact]
        public void GetByPage_ValueTuple()
        {
            var result = SqlMapper.QuerySingle<ValueTuple<IEnumerable<AllPrimitive>, int>>(new RequestContext
            {
                Scope = nameof(AllPrimitive),
                SqlId = "GetByPage_ValueTuple",
                Request = new { PageSize = 10, PageIndex = 1 }
            });
            
            Assert.NotNull(result);
        }
    }
}
