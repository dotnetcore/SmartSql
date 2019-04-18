using System;
using System.Collections.Generic;
using System.Text;
using SmartSql.Test.Entities;
using Xunit;

namespace SmartSql.Test.Unit.IdGenerator
{
    public class DbSequenceTest : AbstractXmlConfigBuilderTest
    {
        [Fact]
        public void NextId()
        {
            new SmartSqlBuilder().UseXmlConfig().Build().SmartSqlConfig.IdGenerators.TryGetValue("DbSequence", out var idGen);

            var id = idGen.NextId();

            Assert.NotEqual(0, id);
        }
        [Fact]
        public void Insert()
        {
            var id = DbSession.ExecuteScalar<long>(new RequestContext
            {
                Scope = nameof(UseIdGenEntity),
                SqlId = "InsertByDbSequence",
                Request = new UseIdGenEntity()
                {
                    Name = "SmartSql"
                }
            });
            Assert.NotEqual(0, id);
        }
    }
}
