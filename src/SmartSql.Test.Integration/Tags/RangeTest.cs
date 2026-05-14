using System;
using SmartSql.Configuration.Tags;
using Xunit;

namespace SmartSql.Test.Integration.Tags
{
    public class RangeTest : IntegrationTestBase
    {
        public RangeTest(SmartSqlFixture fixture) : base(fixture) { }

        [Fact]
        public void Range()
        {
            var requestCtx = new RequestContext
            {
                Scope = "TagTest",
                SqlId = "Range",
                Request = new { Property = 0 }
            };
            requestCtx.SetupParameters();

            var statement = SmartSqlConfig.GetStatement(requestCtx.FullSqlId);
            statement.BuildSql(requestCtx);

            Assert.Equal("Property Between 0 And 10", requestCtx.SqlBuilder.ToString().Trim());
        }

        [Fact]
        public void RangeOutside()
        {
            var requestCtx = new RequestContext
            {
                Scope = "TagTest",
                SqlId = "Range",
                Request = new { Property = 11 }
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
