using System;
using System.Collections.Generic;
using System.Text;
using SmartSql.IdGenerator;
using Xunit;

namespace SmartSql.Test.Unit.IdGenerator
{
    public class CustomSnowflakeIdTest
    {
        [Fact]
        public void Test()
        {
            CustomSnowflakeId snowflakeId = new CustomSnowflakeId();
            snowflakeId.Initialize(new Dictionary<string, object>()
            {
                { "MachineId",1},
                { "MachineIdBits",1},
                { "SequenceBits",5},
                { "EpochDate","2019-05-10"}
            });
            var id = snowflakeId.NextId();
        }
    }
}
