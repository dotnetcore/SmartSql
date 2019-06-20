using System;
using System.Collections.Generic;
using System.Text;
using SmartSql.IdGenerator;
using Xunit;

namespace SmartSql.Test.Unit.IdGenerator
{
    public class CustomSnowflakeIdTest
    {
        private CustomSnowflakeId snowflakeId;

        public CustomSnowflakeIdTest()
        {
            snowflakeId = new CustomSnowflakeId();
            snowflakeId.Initialize(new Dictionary<string, object>()
            {
                {"MachineId", 1},
                {"MachineIdBits", 5},
                {"SequenceBits", 5},
                {"EpochDate", "2019-05-10"}
            });
        }

        [Fact]
        public void NextId()
        {
            var id = snowflakeId.NextId();
            Assert.NotEqual(0,id);
        }
        [Fact]
        public void FromIdLong()
        {
            var id = snowflakeId.NextId();
            
            var idState = snowflakeId.FromId(id);
            
            var toId = snowflakeId.FromIdState(idState);

            Assert.Equal(id, toId);
        }
        [Fact]
        public void FromIdString()
        {
            
            var id = snowflakeId.NextId();
            
            var idState = snowflakeId.FromId(id);
            
            var toId = snowflakeId.FromId(idState.IdString);

            Assert.Equal(id, toId.Id);
        }
    }
}