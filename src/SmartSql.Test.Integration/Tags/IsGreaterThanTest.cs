using System;
using SmartSql.Configuration.Tags;
using Xunit;

namespace SmartSql.Test.Integration.Tags
{
    public class IsGreaterThanTest : IntegrationTestBase
    {
        public IsGreaterThanTest(SmartSqlFixture fixture) : base(fixture) { }

        [Fact]
        public void IsGreaterThan()
        {
            var requestCtx = new RequestContext
            {
                Scope = "TagTest",
                SqlId = "IsGreaterThan",
                Request = new { Property = 11 }
            };
            requestCtx.SetupParameters();

            var statement = SmartSqlConfig.GetStatement(requestCtx.FullSqlId);
            statement.BuildSql(requestCtx);

            Assert.Equal("Property IsGreaterThan 10", requestCtx.SqlBuilder.ToString().Trim());
        }

        [Fact]
        public void IsGreaterThanOutside()
        {
            var requestCtx = new RequestContext
            {
                Scope = "TagTest",
                SqlId = "IsGreaterThan",
                Request = new { Property = 10 }
            };
            requestCtx.SetupParameters();

            var statement = SmartSqlConfig.GetStatement(requestCtx.FullSqlId);
            statement.BuildSql(requestCtx);

            Assert.Equal(String.Empty, requestCtx.SqlBuilder.ToString().Trim());
        }

        [Fact]
        public void IsGreaterThanRequiredEmptyFail()
        {
            var requestCtx = new RequestContext
            {
                Scope = "TagTest",
                SqlId = "IsGreaterThanRequired"
            };
            requestCtx.SetupParameters();

            var statement = SmartSqlConfig.GetStatement(requestCtx.FullSqlId);
            Assert.Throws<TagRequiredFailException>(() => { statement.BuildSql(requestCtx); });
        }
    }
}
