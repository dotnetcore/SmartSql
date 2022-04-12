using System;
using System.Collections.Generic;
using SmartSql.Configuration;
using Xunit;

namespace SmartSql.Test.Unit.Tags
{
    [Collection("GlobalSmartSql")]
    public class OrderByTest
    {
        SmartSqlConfig SmartSqlConfig { get; }

        public OrderByTest(SmartSqlFixture smartSqlFixture)
        {
            SmartSqlConfig = smartSqlFixture.SqlMapper.SmartSqlConfig;
        }

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