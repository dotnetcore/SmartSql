using SmartSql.Configuration.Tags;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace SmartSql.Test.Unit.Tags
{
    [Collection("GlobalSmartSql")]
    public class IsLessThanTest 
    {
        protected ISqlMapper SqlMapper { get; }

        public IsLessThanTest(SmartSqlFixture smartSqlFixture)
        {
            SqlMapper = smartSqlFixture.SqlMapper;
        }

        [Fact]
        public void IsLessThan()
        {
            var msg = SqlMapper.ExecuteScalar<String>(new RequestContext
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
            var msg = SqlMapper.ExecuteScalar<String>(new RequestContext
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
                var msg = SqlMapper.ExecuteScalar<String>(new RequestContext
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
