using System;
using System.Collections.Generic;
using Xunit;

namespace SmartSql.Test.Integration.Tags
{
    public class OrderByTest : IntegrationTestBase
    {
        public OrderByTest(SmartSqlFixture fixture) : base(fixture) { }

        [Fact]
        public void BuildSql()
        {
            var requestCtx = new RequestContext
            {
                Scope = "TagTest",
                SqlId = "OrderBy",
                Request = new
                {
                    OrderBy = new KeyValuePair<String, String>("Id", "Desc")
                }
            };
            requestCtx.SetupParameters();

            var statement = SmartSqlConfig.GetStatement(requestCtx.FullSqlId);
            statement.BuildSql(requestCtx);

            Assert.Equal("Order By Id Desc", requestCtx.SqlBuilder.ToString().Trim());
        }

        [Fact]
        public void BuildSqlWhenEmpty()
        {
            var requestCtx = new RequestContext
            {
                Scope = "TagTest",
                SqlId = "OrderBy"
            };
            requestCtx.SetupParameters();

            var statement = SmartSqlConfig.GetStatement(requestCtx.FullSqlId);
            statement.BuildSql(requestCtx);

            Assert.Equal(String.Empty, requestCtx.SqlBuilder.ToString().Trim());
        }

        [Fact]
        public void BuildSqlWhenMulti()
        {
            var requestCtx = new RequestContext
            {
                Scope = "TagTest",
                SqlId = "OrderBy",
                Request = new
                {
                    OrderBy = new Dictionary<string, string>
                    {
                        { "Id", "Desc" },
                        { "Name", "Asc" },
                    }
                }
            };
            requestCtx.SetupParameters();

            var statement = SmartSqlConfig.GetStatement(requestCtx.FullSqlId);
            statement.BuildSql(requestCtx);

            Assert.Equal("Order By Id Desc,Name Asc", requestCtx.SqlBuilder.ToString().Trim());
        }
    }
}
