using SmartSql.Configuration.Tags;
using System;
using SmartSql.Configuration;
using Xunit;

namespace SmartSql.Test.Unit.Tags
{
    [Collection("GlobalSmartSql")]
    public class WhereTest 
    {
        SmartSqlConfig SmartSqlConfig { get; }

        public WhereTest(SmartSqlFixture smartSqlFixture)
        {
            SmartSqlConfig = smartSqlFixture.SqlMapper.SmartSqlConfig;
        }
        
        [Fact]
        public void BuildSql()
        {
            var requestCtx = new RequestContext
            {
                Scope = "TagTest",
                SqlId = nameof(Where),
                Request = new { Property = "Property" }
            };
            requestCtx.SetupParameters();

            var statement = SmartSqlConfig.GetStatement(requestCtx.FullSqlId);
            statement.BuildSql(requestCtx);

            Assert.Equal(@"Where   
                    T.Property=?Property", requestCtx.SqlBuilder.ToString().Trim());
        }
        [Fact]
        public void BuildSqlWhenRequestIsEmpty()
        {
            var requestCtx = new RequestContext
            {
                Scope = "TagTest",
                SqlId = nameof(Where)
            };
            requestCtx.SetupParameters();

            var statement = SmartSqlConfig.GetStatement(requestCtx.FullSqlId);
            statement.BuildSql(requestCtx);

            Assert.Equal(String.Empty, requestCtx.SqlBuilder.ToString().Trim());
        }
        
        
        [Fact]
        public void BuildSqlMin()
        {
            var requestCtx = new RequestContext
            {
                Scope = "TagTest",
                SqlId = "WhereMin",
                Request = new { Property = "Property" }
            };
            requestCtx.SetupParameters();

            var statement = SmartSqlConfig.GetStatement(requestCtx.FullSqlId);
            statement.BuildSql(requestCtx);

            Assert.Equal(@"Where   
                    T.Property=?Property", requestCtx.SqlBuilder.ToString().Trim());
        }

        [Fact]
        public void BuildSqlMinWhenRequestIsEmpty()
        {
            var requestCtx = new RequestContext
            {
                Scope = "TagTest",
                SqlId = "WhereMin"
            };
            requestCtx.SetupParameters();

            var statement = SmartSqlConfig.GetStatement(requestCtx.FullSqlId);

            Assert.Throws<TagMinMatchedFailException>(() => { statement.BuildSql(requestCtx); });
        }


    }
}
