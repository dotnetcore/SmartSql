using SmartSql.Test.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace SmartSql.Test.Unit.Deserializer
{
    public class DataSetDeserializerTest : AbstractXmlConfigBuilderTest
    {
        [Fact]
        public void GetDataSet()
        {
            var result = DbSession.GetDataSet(new RequestContext
            {
                Scope = nameof(AllPrimitive),
                SqlId = "GetDataSet"
            });
        }

        [Fact]
        public async Task GetDataSetAsync()
        {
            var result = await DbSession.GetDataSetAsync(new RequestContext
            {
                Scope = nameof(AllPrimitive),
                SqlId = "GetDataSet"
            });
        }
    }
}
