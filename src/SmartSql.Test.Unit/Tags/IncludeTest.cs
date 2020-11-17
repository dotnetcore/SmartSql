using SmartSql.Configuration;
using SmartSql.Configuration.Tags;
using Xunit;

namespace SmartSql.Test.Unit.Tags
{
    [Collection("GlobalSmartSql")]
    public class IncludeTest
    {
        SmartSqlConfig SmartSqlConfig { get; }

        public IncludeTest(SmartSqlFixture smartSqlFixture)
        {
            SmartSqlConfig = smartSqlFixture.SqlMapper.SmartSqlConfig;
        }


        [Fact]
        public void Include_Test()
        {
            var requestCtx = new RequestContext
            {
                Scope = nameof(IncludeTest),
                SqlId = "Query",
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
        public void Include_Empty_Test()
        {
            Assert.Throws<TagRequiredFailException>(() =>
            {
                var requestCtx = new RequestContext
                {
                    Scope = nameof(IncludeTest),
                    SqlId = "Query",
                    Request = new {UserName = ""}
                };
                requestCtx.SetupParameters();

                var statement = SmartSqlConfig.GetStatement(requestCtx.FullSqlId);
                statement.BuildSql(requestCtx);
            });

        }

        [Fact]
        public void Include_Empty_Not_Required_Test()
        {
            var requestCtx = new RequestContext
            {
                Scope = nameof(IncludeTest),
                SqlId = "Query_Not_Required",
                Request = new {UserName = ""}
            };
            requestCtx.SetupParameters();

            var statement = SmartSqlConfig.GetStatement(requestCtx.FullSqlId);
            statement.BuildSql(requestCtx);


            Assert.Equal(@"Select * From T_User T", requestCtx.SqlBuilder.ToString().Trim());
        }
    }
}