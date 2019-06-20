using System;
using System.Collections.Generic;
using SmartSql.Configuration.Tags;

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
        int MaxMachineMask { get; }
        int SequenceMask { get; }
        int TimestampShift { get; }
        long LastTimestamp { get; }
        long TimestampMask { get; }
        long TilNextMillis(long lastTimestamp);
        long TimeGen();

        SnowflakeIdState FromId(long snowflakeId);
        SnowflakeIdState FromId(String idString);
    }
}