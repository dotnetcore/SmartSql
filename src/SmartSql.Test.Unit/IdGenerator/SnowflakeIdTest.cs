using SmartSql.IdGenerator;
using System;
using System.Collections.Generic;
using System.Text;
using SmartSql.Test.Entities;
using Xunit;

namespace SmartSql.Test.Unit.IdGenerator
{
    public class SnowflakeIdTest : AbstractXmlConfigBuilderTest
    {
        protected ISqlMapper SqlMapper => BuildSqlMapper(this.GetType().FullName);

        [Fact]
        public void NextId()
        {
            var id = SnowflakeId.Default.NextId();
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
