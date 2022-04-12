using SmartSql.Test.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using SmartSql.Configuration;
using Xunit;

namespace SmartSql.Test.Unit.Tags
{
    [Collection("GlobalSmartSql")]
    public class PlaceholderTest
    {
        SmartSqlConfig SmartSqlConfig { get; }

        public PlaceholderTest(SmartSqlFixture smartSqlFixture)
        {
            SmartSqlConfig = smartSqlFixture.SqlMapper.SmartSqlConfig;
        }

        [Fact]
        public void Placeholder()
        {
            var requestCtx = new RequestContext
            {
                Scope = "TagTest",
                SqlId = nameof(Placeholder),
                Request = new { Placeholder = "Placeholder" }
            };
            requestCtx.SetupParameters();

            var statement = SmartSqlConfig.GetStatement(requestCtx.FullSqlId);
            statement.BuildSql(requestCtx);

            Assert.Equal("Placeholder", requestCtx.SqlBuilder.ToString().Trim());
        }

        [Fact]
        public void NestPlaceholder()
        {
            var reqParams = new { Nest = new { Placeholder = "Placeholder" } };
            var requestCtx = new RequestContext
            {
                Scope = "TagTest",
                SqlId = nameof(NestPlaceholder),
                Request = reqParams
            };
            requestCtx.SetupParameters();

            var statement = SmartSqlConfig.GetStatement(requestCtx.FullSqlId);
            statement.BuildSql(requestCtx);
            Assert.Equal("Placeholder", requestCtx.SqlBuilder.ToString().Trim());
        }
    }
}