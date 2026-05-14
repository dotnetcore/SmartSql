using System;
using System.Collections.Generic;
using SmartSql.Test.Entities;
using Xunit;

namespace SmartSql.Test.Integration.Deserializer
{
    public class ValueTupleDeserializerTest : IntegrationTestBase
    {
        public ValueTupleDeserializerTest(SmartSqlFixture fixture) : base(fixture) { }

        [Fact]
        public void GetByPage_ValueTuple()
        {
            var result = SqlMapper.QuerySingle<ValueTuple<IEnumerable<AllPrimitive>, int>>(new RequestContext
            {
                Scope = nameof(AllPrimitive),
                SqlId = "GetByPage_ValueTuple",
                Request = new { PageSize = 10, Offset = 0 }
            });
            Assert.NotNull(result);
        }
    }
}
