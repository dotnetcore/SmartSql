using SmartSql.IdGenerator;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace SmartSql.Test.Unit.IdGenerator
{
    public class SnowflakeIdTest
    {
        [Fact]
        public void NextId()
        {
            var id = SnowflakeId.Default.NextId();
            Assert.NotEqual(0, id);
        }
    }
}
