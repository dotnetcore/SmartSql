using SmartSql.Configuration.Tags;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace SmartSql.Test.Unit.Tags
{
    [Collection("GlobalSmartSql")]
    public class WhereTest 
    {
        protected ISqlMapper SqlMapper { get; }

        public WhereTest(SmartSqlFixture smartSqlFixture)
        {
            SqlMapper = smartSqlFixture.SqlMapper;
        }
        [Fact]
        public void Where_Min()
        {
            var msg = SqlMapper.ExecuteScalar<String>(new RequestContext
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
                var msg = SqlMapper.ExecuteScalar<String>(new RequestContext
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
