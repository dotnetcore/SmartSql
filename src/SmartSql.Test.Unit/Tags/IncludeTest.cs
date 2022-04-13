using System;
using SmartSql.Configuration;
using SmartSql.Configuration.Tags;
using Xunit;

namespace SmartSql.Test.Unit.Tags
{
    [Collection("GlobalSmartSql")]
    public class IncludeTest
    {
        SmartSqlConfig SmartSqlConfig { get; }

        public IncludeTest(SmartSqlFixture smartSqlFixture)
        {
            SmartSqlConfig = smartSqlFixture.SqlMapper.SmartSqlConfig;
        }

        [Fact]
        public void BuildSql()
        {
            var requestCtx = new RequestContext
            {
                Scope = "TagTest",
                SqlId = "Include",
                Request = new { Property = "Property" }
            };
            requestCtx.SetupParameters();

            var statement = SmartSqlConfig.GetStatement(requestCtx.FullSqlId);
            statement.BuildSql(requestCtx);

            Assert.Equal(@"Where     
                Property=?Property", requestCtx.SqlBuilder.ToString().Trim());
        }

        [Fact]
        public void BuildSqlWhenEmpty()
        {
            var requestCtx = new RequestContext
            {
                Scope = "TagTest",
                SqlId = "Include",
                Request = new { Property = "" }
            };
            requestCtx.SetupParameters();

            var statement = SmartSqlConfig.GetStatement(requestCtx.FullSqlId);
            statement.BuildSql(requestCtx);
            Assert.Equal(String.Empty, requestCtx.SqlBuilder.ToString().Trim());
        }

        [Fact]
        public void IncludeRequired()
        {
            var requestCtx = new RequestContext
            {
                Scope = "TagTest",
                SqlId = "IncludeRequired",
                Request = new { Property = "" }
            };
            requestCtx.SetupParameters();

            var statement = SmartSqlConfig.GetStatement(requestCtx.FullSqlId);

            Assert.Throws<TagRequiredFailException>(() => { statement.BuildSql(requestCtx); });
        }
    }
}