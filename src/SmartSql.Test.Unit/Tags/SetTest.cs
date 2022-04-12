using SmartSql.Test.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using SmartSql.Configuration;
using SmartSql.Configuration.Tags;
using Xunit;

namespace SmartSql.Test.Unit.Tags
{
    [Collection("GlobalSmartSql")]
    public class SetTest
    {
        SmartSqlConfig SmartSqlConfig { get; }

        public SetTest(SmartSqlFixture smartSqlFixture)
        {
            SmartSqlConfig = smartSqlFixture.SqlMapper.SmartSqlConfig;
        }

        [Fact]
        public void Set()
        {
            var requestCtx = new RequestContext
            {
                Scope = "TagTest",
                SqlId = nameof(Set),
                Request = new { Property1 = "SmartSql", Property2 = 1, Id = 1 }
            };
            requestCtx.SetupParameters();

            var statement = SmartSqlConfig.GetStatement(requestCtx.FullSqlId);
            statement.BuildSql(requestCtx);

            Assert.Equal(@"Set   
                    Property1=?Property1
                 , 
                    Property2=?Property2", requestCtx.SqlBuilder.ToString().Trim());
        }

        [Fact]
        public void SetWhenRequestIsEmpty()
        {
            var requestCtx = new RequestContext
            {
                Scope = "TagTest",
                SqlId = nameof(Set)
            };
            requestCtx.SetupParameters();

            var statement = SmartSqlConfig.GetStatement(requestCtx.FullSqlId);
            Assert.Throws<TagMinMatchedFailException>(() => { statement.BuildSql(requestCtx); });
        }
    }
}