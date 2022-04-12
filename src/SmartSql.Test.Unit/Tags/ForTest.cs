using System;
using SmartSql.Configuration;
using SmartSql.Configuration.Tags;
using SmartSql.Test.Entities;
using Xunit;

namespace SmartSql.Test.Unit.Tags
{
    [Collection("GlobalSmartSql")]
    public class ForTest
    {
        SmartSqlConfig SmartSqlConfig { get; }

        public ForTest(SmartSqlFixture smartSqlFixture)
        {
            SmartSqlConfig = smartSqlFixture.SqlMapper.SmartSqlConfig;
        }

        [Fact]
        public void BuildSqlWhenDirectValue()
        {
            var requestCtx = new RequestContext
            {
                Scope = "TagTest",
                SqlId = "ForWhenDirectValue",
                Request = new { Items = new[] { 1, 2 } }
            };
            var executionContext = new ExecutionContext
            {
                Request = requestCtx,
                SmartSqlConfig = SmartSqlConfig
            };
            requestCtx.ExecutionContext = executionContext;

            requestCtx.SetupParameters();

            var statement = SmartSqlConfig.GetStatement(requestCtx.FullSqlId);
            statement.BuildSql(requestCtx);

            Assert.Equal(@"(
                ?Item_For__0
             , 
                ?Item_For__1
            )", requestCtx.SqlBuilder.ToString().Trim());

            Assert.Equal(1, requestCtx.Parameters["Item_For__0"].Value);
            Assert.Equal(2, requestCtx.Parameters["Item_For__1"].Value);
        }

        [Fact]
        public void BuildSqlWhenNotDirectValue()
        {
            var requestCtx = new RequestContext
            {
                Scope = "TagTest",
                SqlId = "ForWhenNotDirectValue",
                Request = new { Items = new[] { new { Id = 1 }, new { Id = 2 } } }
            };
            var executionContext = new ExecutionContext
            {
                Request = requestCtx,
                SmartSqlConfig = SmartSqlConfig
            };
            requestCtx.ExecutionContext = executionContext;

            requestCtx.SetupParameters();

            var statement = SmartSqlConfig.GetStatement(requestCtx.FullSqlId);
            statement.BuildSql(requestCtx);

            Assert.Equal(@"(
                ?Item_For__Id_0
             , 
                ?Item_For__Id_1
            )", requestCtx.SqlBuilder.ToString().Trim());

            Assert.Equal(1, requestCtx.Parameters["Item_For__Id_0"].Value);
            Assert.Equal(2, requestCtx.Parameters["Item_For__Id_1"].Value);
        }

        [Fact]
        public void BuildSqlWhenNotDirectValueWithKey()
        {
            var requestCtx = new RequestContext
            {
                Scope = "TagTest",
                SqlId = "ForWhenNotDirectValueWithKey",
                Request = new { Items = new[] { new { Id = 1 }, new { Id = 2 } } }
            };
            var executionContext = new ExecutionContext
            {
                Request = requestCtx,
                SmartSqlConfig = SmartSqlConfig
            };
            requestCtx.ExecutionContext = executionContext;

            requestCtx.SetupParameters();

            var statement = SmartSqlConfig.GetStatement(requestCtx.FullSqlId);
            statement.BuildSql(requestCtx);

            Assert.Equal(@"(
                ?Item_For__Id_0
             , 
                ?Item_For__Id_1
            )", requestCtx.SqlBuilder.ToString().Trim());

            Assert.Equal(1, requestCtx.Parameters["Item_For__Id_0"].Value);
            Assert.Equal(2, requestCtx.Parameters["Item_For__Id_1"].Value);
        }

        [Fact]
        public void BuildSqlWhenNotDirectNestValueWithKey()
        {
            var requestCtx = new RequestContext
            {
                Scope = "TagTest",
                SqlId = "ForWhenNotDirectNestValueWithKey",
                Request = new
                {
                    Items = new[]
                    {
                        new { Info = new { Id = 1 } },
                        new { Info = new { Id = 2 } }
                    }
                }
            };
            var executionContext = new ExecutionContext
            {
                Request = requestCtx,
                SmartSqlConfig = SmartSqlConfig
            };
            requestCtx.ExecutionContext = executionContext;

            requestCtx.SetupParameters();

            var statement = SmartSqlConfig.GetStatement(requestCtx.FullSqlId);
            statement.BuildSql(requestCtx);

            Assert.Equal(@"(
                ?Item_For__Info_Id_0
             , 
                ?Item_For__Info_Id_1
            )", requestCtx.SqlBuilder.ToString().Trim());

            Assert.Equal(1, requestCtx.Parameters["Item_For__Info_Id_0"].Value);
            Assert.Equal(2, requestCtx.Parameters["Item_For__Info_Id_1"].Value);

        }
    }
}