﻿using SmartSql.Configuration;
using Xunit;

namespace SmartSql.Test.Unit.Tags
{
    [Collection("GlobalSmartSql")]
    public class DynamicTest
    {
        SmartSqlConfig SmartSqlConfig { get; }

        public DynamicTest(SmartSqlFixture smartSqlFixture)
        {
            SmartSqlConfig = smartSqlFixture.SqlMapper.SmartSqlConfig;
        }

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

            Assert.Equal(@"Select * From T_Table T
             Where   
                    T.Property=?Property", requestCtx.SqlBuilder.ToString().Trim());
        }

        [Fact]
        public void DynamicWhenPropertyIsEmpty()
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

            Assert.Equal(@"Select * From T_Table T", requestCtx.SqlBuilder.ToString().Trim());
        }
    }
}