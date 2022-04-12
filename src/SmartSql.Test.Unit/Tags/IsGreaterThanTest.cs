using SmartSql.Configuration.Tags;
using System;
using System.Collections.Generic;
using System.Text;
using SmartSql.Configuration;
using Xunit;

namespace SmartSql.Test.Unit.Tags
{
    [Collection("GlobalSmartSql")]
    public class IsGreaterThanTest 
    {
        SmartSqlConfig SmartSqlConfig { get; }

        public IsGreaterThanTest(SmartSqlFixture smartSqlFixture)
        {
            SmartSqlConfig = smartSqlFixture.SqlMapper.SmartSqlConfig;
        }
        
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
