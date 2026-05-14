using System;
using SmartSql.Configuration.Tags;
using Xunit;

namespace SmartSql.Test.Integration.Tags
{
    public class IsLessThanTest : IntegrationTestBase
    {
        public IsLessThanTest(SmartSqlFixture fixture) : base(fixture) { }

        [Fact]
        public void IsLessThan()
        {
            var requestCtx = new RequestContext
            {
                Scope = "TagTest",
                SqlId = "IsLessThan",
                Request = new { Property = 9 }
            };
            requestCtx.SetupParameters();

            var statement = SmartSqlConfig.GetStatement(requestCtx.FullSqlId);
            statement.BuildSql(requestCtx);

            Assert.Equal("Property IsLessThan 10", requestCtx.SqlBuilder.ToString().Trim());
        }

        [Fact]
        public void IsLessThanOutside()
        {
            var requestCtx = new RequestContext
            {
                Scope = "TagTest",
                SqlId = "IsLessThan",
                Request = new { Property = 10 }
            };
            requestCtx.SetupParameters();

            var statement = SmartSqlConfig.GetStatement(requestCtx.FullSqlId);
            statement.BuildSql(requestCtx);

            Assert.Equal(String.Empty, requestCtx.SqlBuilder.ToString().Trim());
        }

        [Fact]
        public void IsLessThanRequiredEmptyFail()
        {
            var requestCtx = new RequestContext
            {
                Scope = "TagTest",
                SqlId = "IsLessThanRequired"
            };
            requestCtx.SetupParameters();

            var statement = SmartSqlConfig.GetStatement(requestCtx.FullSqlId);
            Assert.Throws<TagRequiredFailException>(() => { statement.BuildSql(requestCtx); });
        }
    }
}
