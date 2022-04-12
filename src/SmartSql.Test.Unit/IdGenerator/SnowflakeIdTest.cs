using SmartSql.IdGenerator;
using System;
using System.Collections.Generic;
using System.Text;
using SmartSql.Test.Entities;
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
            var idState = SnowflakeId.Default.FromId(id);
            Assert.Equal(id, idState.Id);
            Assert.Equal(DateTime.UtcNow.Date, idState.UtcTime.Date);
        }
    }
}