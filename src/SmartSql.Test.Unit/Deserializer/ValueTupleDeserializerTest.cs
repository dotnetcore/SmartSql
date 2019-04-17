using System;
using System.Collections.Generic;
using System.Text;
using SmartSql.Test.DTO;
using SmartSql.Test.Entities;
using Xunit;

namespace SmartSql.Test.Unit.Deserializer
{
    public class ValueTupleDeserializerTest : AbstractXmlConfigBuilderTest
    {
        [Fact]
        public void GetByPage_ValueTuple()
        {
            var result = DbSession.QuerySingle<ValueTuple<IEnumerable<AllPrimitive>, int>>(new RequestContext
            {
                Scope = nameof(AllPrimitive),
                SqlId = "GetByPage_ValueTuple",
                Request = new { PageSize = 10, PageIndex = 1 }
            });
            
            Assert.NotNull(result);
        }
    }
}
