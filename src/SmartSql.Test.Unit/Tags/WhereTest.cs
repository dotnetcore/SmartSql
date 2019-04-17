using SmartSql.Configuration.Tags;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace SmartSql.Test.Unit.Tags
{
    public class WhereTest : AbstractXmlConfigBuilderTest
    {
        [Fact]
        public void Where_Min()
        {
            var msg = DbSession.ExecuteScalar<String>(new RequestContext
            {
                Scope = nameof(WhereTest),
                SqlId = "Where_Min",
                Request = new { Msg = "SmartSql" }
            });
            Assert.Equal("Where", msg);
        }
        [Fact]
        public void Where_Min_Fail()
        {
            try
            {
                var msg = DbSession.ExecuteScalar<String>(new RequestContext
                {
                    Scope = nameof(WhereTest),
                    SqlId = "Where_Min",
                    Request = new { }
                });
                Assert.True(false);
            }
            catch (TagMinMatchedFailException ex)
            {
                Assert.True(true);
            }
        }
    }
}
