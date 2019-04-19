using System;
using System.Collections.Generic;
using System.Text;
using SmartSql.Test.Entities;
using Xunit;

namespace SmartSql.Test.Unit.IdGenerator
{
    [Collection("GlobalSmartSql")]
    public class DbSequenceTest
    {
        protected ISqlMapper SqlMapper { get; }

        public DbSequenceTest(SmartSqlFixture smartSqlFixture)
        {
            SqlMapper = smartSqlFixture.SqlMapper;
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
