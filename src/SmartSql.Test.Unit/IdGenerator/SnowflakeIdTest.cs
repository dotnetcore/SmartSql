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
        [Fact]
        public void NextId()
        {
            var id = SnowflakeId.Default.NextId();
            Assert.NotEqual(0, id);
        }
        [Fact]
        public void Insert()
        {
            var id = DbSession.ExecuteScalar<long>(new RequestContext
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
