using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace SmartSql.Test.Unit.IdGenerator
{
    public class DbSequenceTest
    {
        [Fact]
        public void NextId()
        {
            new SmartSqlBuilder().UseXmlConfig().Build().SmartSqlConfig.IdGenerators.TryGetValue("DbSequence", out var idGen);

            var id = idGen.NextId();

            Assert.NotEqual(0, id);
        }
    }
}
