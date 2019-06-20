using SmartSql.IdGenerator;
using System;
using System.Collections.Generic;
using System.Text;
using SmartSql.Test.Entities;
using Xunit;

namespace SmartSql.Test.Unit.IdGenerator
{
    [Collection("GlobalSmartSql")]
    public class SnowflakeIdTest
    {
        protected ISqlMapper SqlMapper { get; }

        public SnowflakeIdTest(SmartSqlFixture smartSqlFixture)
        {
            SqlMapper = smartSqlFixture.SqlMapper;
        }

        [Fact]
        public void NextId()
        {
            var id = SnowflakeId.Default.NextId();
            var idState = SnowflakeId.Default.FromId(id);
            
            Assert.NotEqual(0, id);
        }

        [Fact]
        public void Insert()
        {
            var id = SqlMapper.ExecuteScalar<long>(new RequestContext
            {
                Scope = nameof(UseIdGenEntity),
                SqlId = "Insert",
                Request = new UseIdGenEntity()
                {
                    Name = "SmartSql"
                }
            });
            Assert.NotEqual(0, id);
        }
    }
}