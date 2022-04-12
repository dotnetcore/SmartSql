using System;
using System.Collections.Generic;
using System.Text;
using SmartSql.Configuration;
using Xunit;

namespace SmartSql.Test.Unit.Tags
{
    [Collection("GlobalSmartSql")]
    public class NowTest
    {
        SmartSqlConfig SmartSqlConfig { get; }

        public NowTest(SmartSqlFixture smartSqlFixture)
        {
            SmartSqlConfig = smartSqlFixture.SqlMapper.SmartSqlConfig;
        }
        
        [Fact]
        public void AppendNowTime()
        {
            var requestCtx = new RequestContext
            {
                Scope = "TagTest",
                SqlId = "Now"
            };
            requestCtx.SetupParameters();

            var statement = SmartSqlConfig.GetStatement(requestCtx.FullSqlId);
            statement.BuildSql(requestCtx);

            Assert.True(requestCtx.Parameters.ContainsKey("NowTime"));
            
            Assert.Equal(@"Select ?NowTime;", requestCtx.SqlBuilder.ToString().Trim());
        }
    }
}