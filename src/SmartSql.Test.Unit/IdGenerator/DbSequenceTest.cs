using System;
using System.Collections.Generic;
using System.Text;
using SmartSql.Test.Entities;
using Xunit;

namespace SmartSql.Test.Unit.IdGenerator
{
    public class DbSequenceTest : AbstractXmlConfigBuilderTest
    {
        protected ISqlMapper SqlMapper { get; }

        public DbSequenceTest()
        {
            SqlMapper = BuildSqlMapper(this.GetType().FullName);
        }

        [Fact]
        public void NextId()
        {
            SqlMapper.SmartSqlConfig.IdGenerators.TryGetValue("DbSequence", out var idGen);

            var id = idGen.NextId();

            Assert.NotEqual(0, id);
        }
        [Fact]
        public void Insert()
        {
            var id = SqlMapper.ExecuteScalar<long>(new RequestContext
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
