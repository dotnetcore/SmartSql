using System;
using SmartSql.Configuration;
using Xunit;

namespace SmartSql.Test.Unit.Tags
{
    [Collection("GlobalSmartSql")]
    public class UUIDTest
    {
        SmartSqlConfig SmartSqlConfig { get; }

        public UUIDTest(SmartSqlFixture smartSqlFixture)
        {
            SmartSqlConfig = smartSqlFixture.SqlMapper.SmartSqlConfig;
        }

        [Fact]
        public void UUID()
        {
            var requestCtx = new RequestContext
            {
                Scope = "TagTest",
                SqlId = "UUID"
            };
            requestCtx.SetupParameters();

            var statement = SmartSqlConfig.GetStatement(requestCtx.FullSqlId);
            statement.BuildSql(requestCtx);

            Assert.True(requestCtx.Parameters.ContainsKey("UUID"));
            Assert.True(requestCtx.Parameters["UUID"].Value.ToString().Contains("-"));
            Assert.Equal(@"Select ?UUID;", requestCtx.SqlBuilder.ToString().Trim());
        }

        [Fact]
        public void UUIDToN()
        {
            var requestCtx = new RequestContext
            {
                Scope = "TagTest",
                SqlId = "UUIDToN"
            };
            requestCtx.SetupParameters();

            var statement = SmartSqlConfig.GetStatement(requestCtx.FullSqlId);
            statement.BuildSql(requestCtx);

            Assert.True(requestCtx.Parameters.ContainsKey("UUID"));
            Assert.False(requestCtx.Parameters["UUID"].Value.ToString().Contains("-"));
            Assert.Equal(@"Select ?UUID;", requestCtx.SqlBuilder.ToString().Trim());
        }
    }
}