using SmartSql.Configuration.Tags;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace SmartSql.Test.Unit.Tags
{
    public class IsLessThanTest : AbstractXmlConfigBuilderTest
    {
        [Fact]
        public void IsLessThan()
        {
            var msg = DbSession.ExecuteScalar<String>(new RequestContext
            {
                Scope = nameof(IsLessThanTest),
                SqlId = "IsLessThan",
                Request = new { Id = -1 }
            });
            Assert.Equal("Id IsLessThan 0", msg);
        }
        [Fact]
        public void IsLessThan_Required()
        {
            var msg = DbSession.ExecuteScalar<String>(new RequestContext
            {
                Scope = nameof(IsLessThanTest),
                SqlId = "IsLessThan_Required",
                Request = new { Id = -1 }
            });
            Assert.Equal("Id IsLessThan 0", msg);
        }
        [Fact]
        public void IsGreaterThan_Required_Fail()
        {
            try
            {
                var msg = DbSession.ExecuteScalar<String>(new RequestContext
                {
                    Scope = nameof(IsLessThanTest),
                    SqlId = "IsLessThan_Required",
                    Request = new { }
                });
                Assert.Equal("Id IsLessThan 0", msg);
            }
            catch (TagRequiredFailException ex)
            {
                Assert.True(true);
            }
        }
    }
}
