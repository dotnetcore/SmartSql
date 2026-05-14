using FluentAssertions;
using System;
using Xunit;

namespace SmartSql.Test.Integration.Tags
{
    public class DynamicTest : IntegrationTestBase
    {
        public DynamicTest(SmartSqlFixture fixture) : base(fixture) { }

        [Fact]
        public void BuildSql()
        {
            var requestCtx = new RequestContext
            {
                Scope = "TagTest",
                SqlId = "Dynamic",
                Request = new { Property = "Property" }
            };
            requestCtx.SetupParameters();

            var statement = SmartSqlConfig.GetStatement(requestCtx.FullSqlId);
            statement.BuildSql(requestCtx);

            Assert.Equal(@"Where
                    T.Property=?Property", requestCtx.SqlBuilder.ToString().Trim());
        }

        [Fact]
        public void BuildSqlWhenPropertyIsEmpty()
        {
            var requestCtx = new RequestContext
            {
                Scope = "TagTest",
                SqlId = "Dynamic",
                Request = new { Property = "" }
            };
            requestCtx.SetupParameters();

            var statement = SmartSqlConfig.GetStatement(requestCtx.FullSqlId);
            statement.BuildSql(requestCtx);

            Assert.Equal(String.Empty, requestCtx.SqlBuilder.ToString().Trim());
        }
    }
}
