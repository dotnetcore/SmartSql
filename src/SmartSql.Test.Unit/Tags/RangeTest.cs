using SmartSql.Configuration.Tags;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace SmartSql.Test.Unit.Tags
{
    [Collection("GlobalSmartSql")]
    public class RangeTest
    {
        protected ISqlMapper SqlMapper { get; }

        public RangeTest(SmartSqlFixture smartSqlFixture)
        {
            SqlMapper = smartSqlFixture.SqlMapper;
        }

        [Fact]
        public void Range()
        {
            var msg = SqlMapper.ExecuteScalar<String>(new RequestContext
            {
                Scope = nameof(RangeTest),
                SqlId = "Range",
                Request = new { Id = 0 }
            });
            Assert.Equal("Id Between 0 And 10", msg);
        }
        [Fact]
        public void Range_Required()
        {
            var msg = SqlMapper.ExecuteScalar<String>(new RequestContext
            {
                Scope = nameof(RangeTest),
                SqlId = "Range_Required",
                Request = new { Id = 1 }
            });
            Assert.Equal("Id Between 0 And 10", msg);
        }
        [Fact]
        public void Range_Required_Fail()
        {
            try
            {
                var msg = SqlMapper.ExecuteScalar<String>(new RequestContext
                {
                    Scope = nameof(RangeTest),
                    SqlId = "Range_Required",
                    Request = new { }
                });
                Assert.Equal("Id Between 0 And 10", msg);
            }
            catch (TagRequiredFailException ex)
            {
                Assert.True(true);
            }
        }
    }
}
