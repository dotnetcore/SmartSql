using SmartSql.Configuration.Tags;
using System;
using System.Collections.Generic;
using System.Text;
using SmartSql.Configuration;
using Xunit;

namespace SmartSql.Test.Unit.Tags
{
    [Collection("GlobalSmartSql")]
    public class RangeTest
    {
        SmartSqlConfig SmartSqlConfig { get; }

        public RangeTest(SmartSqlFixture smartSqlFixture)
        {
            SmartSqlConfig = smartSqlFixture.SqlMapper.SmartSqlConfig;
        }

        [Fact]
        public void Range()
        {
            var requestCtx = new RequestContext
            {
                Scope = "TagTest",
                SqlId = "Range",
                Request = new { Range = 0 }
            };
            requestCtx.SetupParameters();

            var statement = SmartSqlConfig.GetStatement(requestCtx.FullSqlId);
            statement.BuildSql(requestCtx);

            Assert.Equal("Range Between 0 And 10", requestCtx.SqlBuilder.ToString().Trim());
        }
        [Fact]
        public void RangeOutside()
        {
            var requestCtx = new RequestContext
            {
                Scope = "TagTest",
                SqlId = "Range",
                Request = new { Range = 11 }
            };
            requestCtx.SetupParameters();

            var statement = SmartSqlConfig.GetStatement(requestCtx.FullSqlId);
            statement.BuildSql(requestCtx);

            Assert.Equal(String.Empty, requestCtx.SqlBuilder.ToString().Trim());
        }

        [Fact]
        public void RangeRequiredEmptyFail()
        {
            var requestCtx = new RequestContext
            {
                Scope = "TagTest",
                SqlId = "RangeRequired"
            };
            requestCtx.SetupParameters();

            var statement = SmartSqlConfig.GetStatement(requestCtx.FullSqlId);
            Assert.Throws<TagRequiredFailException>(() => { statement.BuildSql(requestCtx); });
        }
    }
}