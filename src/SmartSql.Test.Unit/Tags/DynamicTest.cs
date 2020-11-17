using SmartSql.Configuration;
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
        public void Dynamic_Test()
        {
            var requestCtx = new RequestContext
            {
                Scope = nameof(DynamicTest),
                SqlId = "GetUser",
                Request = new {UserName = "SmartSql"}
            };
            requestCtx.SetupParameters();

            var statement = SmartSqlConfig.GetStatement(requestCtx.FullSqlId);
            statement.BuildSql(requestCtx);

            Assert.Equal(@"Select * From T_User T
       Where   
          T.UserName=@UserName", requestCtx.SqlBuilder.ToString().Trim());
        }

        [Fact]
        public void Dynamic_Empty_Test()
        {
            var requestCtx = new RequestContext
            {
                Scope = nameof(DynamicTest),
                SqlId = "GetUser",
                Request = new {UserName = ""}
            };
            requestCtx.SetupParameters();

            var statement = SmartSqlConfig.GetStatement(requestCtx.FullSqlId);
            statement.BuildSql(requestCtx);

            Assert.Equal(@"Select * From T_User T", requestCtx.SqlBuilder.ToString().Trim());
        }
    }
}