using FluentAssertions;
using System;
using System.Collections.Generic;
using SmartSql.Configuration.Tags;
using Xunit;

namespace SmartSql.Test.Integration.Tags
{
    public class IsNotEmptyTest : IntegrationTestBase
    {
        public IsNotEmptyTest(SmartSqlFixture fixture) : base(fixture) { }

        [Fact]
        public void BuildSql()
        {
            var requestCtx = new RequestContext
            {
                Scope = "TagTest",
                SqlId = "IsNotEmpty",
                Request = new { Property = true }
            };
            requestCtx.SetupParameters();

            var statement = SmartSqlConfig.GetStatement(requestCtx.FullSqlId);
            statement.BuildSql(requestCtx);

            Assert.Equal("Property IsNotEmpty", requestCtx.SqlBuilder.ToString().Trim());
        }

        [Fact]
        public void BuildSqlWhenStringIsEmpty()
        {
            var requestCtx = new RequestContext
            {
                Scope = "TagTest",
                SqlId = "IsNotEmpty",
                Request = new { Property = "" }
            };
            requestCtx.SetupParameters();

            var statement = SmartSqlConfig.GetStatement(requestCtx.FullSqlId);
            statement.BuildSql(requestCtx);

            Assert.Equal(String.Empty, requestCtx.SqlBuilder.ToString().Trim());
        }

        [Fact]
        public void BuildSqlWhenListIsEmpty()
        {
            var requestCtx = new RequestContext
            {
                Scope = "TagTest",
                SqlId = "IsNotEmpty",
                Request = new { Property = new List<String>() }
            };
            requestCtx.SetupParameters();

            var statement = SmartSqlConfig.GetStatement(requestCtx.FullSqlId);
            statement.BuildSql(requestCtx);

            Assert.Equal(String.Empty, requestCtx.SqlBuilder.ToString().Trim());
        }

        [Fact]
        public void BuildSqlWhenIsEmpty()
        {
            var requestCtx = new RequestContext
            {
                Scope = "TagTest",
                SqlId = "IsNotEmpty"
            };
            requestCtx.SetupParameters();

            var statement = SmartSqlConfig.GetStatement(requestCtx.FullSqlId);
            statement.BuildSql(requestCtx);

            Assert.Equal(String.Empty, requestCtx.SqlBuilder.ToString().Trim());
        }

        [Fact]
        public void BuildSqlRequiredFail()
        {
            var requestCtx = new RequestContext
            {
                Scope = "TagTest",
                SqlId = "IsNotEmptyRequired"
            };
            requestCtx.SetupParameters();

            var statement = SmartSqlConfig.GetStatement(requestCtx.FullSqlId);
            Assert.Throws<TagRequiredFailException>(() => statement.BuildSql(requestCtx));
        }
    }
}
