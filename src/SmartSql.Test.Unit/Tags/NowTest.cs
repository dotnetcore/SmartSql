using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace SmartSql.Test.Unit.Tags
{
    [Collection("GlobalSmartSql")]
    public class NowTest
    {
        protected ISqlMapper SqlMapper { get; }

        public NowTest(SmartSqlFixture smartSqlFixture)
        {
            SqlMapper = smartSqlFixture.SqlMapper;
        }

        [Fact]
        public void GetNow()
        {
            var msg = SqlMapper.ExecuteScalar<DateTime>(new RequestContext
            {
                Scope = nameof(NowTest),
                SqlId = "GetNow"
            });
            Assert.True(true);
        }
        
        public void GetExpNow()
        {
            var msg = SqlMapper.ExecuteScalar<DateTime>(new RequestContext
            {
                Scope = nameof(NowTest),
                SqlId = "GetExpNow"
            });
            Assert.True(true);
        }
        

        [Fact]
        public void UpdateDateTime()
        {
            var msg = SqlMapper.ExecuteScalar<DateTime>(new RequestContext
            {
                Scope = nameof(NowTest),
                SqlId = "UpdateDateTime",
                Request = new {Id = 86088}
            });
            Assert.True(true);
        }
    }
}