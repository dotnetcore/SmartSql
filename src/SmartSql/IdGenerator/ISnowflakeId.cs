using System;
using System.Collections.Generic;

namespace SmartSql.IdGenerator
{
    public interface ISnowflakeId : IIdGenerator
    {
        int MachineIdBits { get; }
        int SequenceBits { get; }
        long Epoch { get; }
        DateTimeOffset EpochTime { get; }
        long MachineId { get; }
        long Sequence { get; }
        int MaxMachineId { get; }
        int SequenceMask { get; }
        int TimestampShift { get; }
        long LastTimestamp { get; }
        long TilNextMillis(long lastTimestamp);
        long TimeGen();

        SnowflakeIdState FromId(long snowflakeId);
    }

    public class SnowflakeIdState
    {
        public DateTime Time { get; set; }
        public long MachineId { get;set;  }
        public long Sequence { get;set;  }
    }
}