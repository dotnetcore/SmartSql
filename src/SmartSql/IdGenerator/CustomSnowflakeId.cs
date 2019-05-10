using System;
using System.Collections.Generic;
using System.Text;

namespace SmartSql.IdGenerator
{
    public class CustomSnowflakeId : IIdGenerator
    {
        public const long DEFAULT_EPOCH = 1288834974657L;
        private const int DEFAULT_MACHINE_ID_BITS = 10;
        private const int DEFAULT_SEQUENCE_BITS = 12;
        public int MachineIdBits { get; set; }
        public int SequenceBits { get; set; }
        public long Epoch { get; set; }
        public long MachineId { get; set; }
        public long Sequence { get; set; }
        public int MaxMachineId { get; set; }
        public int SequenceMask { get; set; }
        public int TimestampShift { get; set; }
        public long LastTimestamp { get; private set; } = -1L;

        public void Initialize(IDictionary<string, object> parameters)
        {
            parameters.EnsureValue(nameof(MachineId), out long machineId);
            parameters.Value(nameof(Sequence), out long sequence);
            if (!parameters.Value(nameof(MachineIdBits), out int machineIdBits))
            {
                machineIdBits = DEFAULT_MACHINE_ID_BITS;
            }
            if (!parameters.Value(nameof(SequenceBits), out int sequenceBits))
            {
                sequenceBits = DEFAULT_SEQUENCE_BITS;
            }
            if (!parameters.Value(nameof(Epoch), out long epoch))
            {
                epoch = DEFAULT_EPOCH;
            }
            if (parameters.Value("EpochDate", out string epochDateStr))
            {
                if (!DateTimeOffset.TryParse(epochDateStr, out var epochDate))
                {
                    throw new ArgumentException($"EpochDate:[{epochDateStr}] can not convert to DateTime.");
                }
                epoch= epochDate.ToUnixTimeMilliseconds();
            }
            Init(machineId, sequence, machineIdBits, sequenceBits, epoch);
        }
        protected virtual long TilNextMillis(long lastTimestamp)
        {
            var timestamp = TimeGen();
            while (timestamp <= lastTimestamp) timestamp = TimeGen();
            return timestamp;
        }
        protected virtual long TimeGen()
        {
            return DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        }
        private void Init(long machineId, long sequence
            , int machineIdBits = DEFAULT_MACHINE_ID_BITS
            , int sequenceBits = DEFAULT_SEQUENCE_BITS
            , long epoch = DEFAULT_EPOCH)
        {
            var timestamp = TimeGen();
            if (epoch > timestamp || epoch < 0)
            {
                throw new ArgumentException($"epoch:{epoch} can't be greater than current timestamp {timestamp} or less than 0");
            }
            Epoch = epoch;
            MachineId = machineId;
            Sequence = sequence;
            MachineIdBits = machineIdBits;
            MaxMachineId = -1 ^ (-1 << MachineIdBits);
            SequenceBits = sequenceBits;
            SequenceMask = -1 ^ (-1 << SequenceBits);
            TimestampShift = SequenceBits + MachineIdBits;
            if (MachineId > MaxMachineId || MachineId < 0)
            {
                throw new ArgumentException($"worker Id can't be greater than {MaxMachineId} or less than 0");
            }
        }

        public long NextId()
        {
            lock (this)
            {
                var timestamp = TimeGen();

                if (timestamp < LastTimestamp)
                    throw new Exception(
                        $"InvalidSystemClock: Clock moved backwards, Refusing to generate id for {LastTimestamp - timestamp} milliseconds");

                if (LastTimestamp == timestamp)
                {
                    Sequence = (Sequence + 1) & SequenceMask;
                    if (Sequence == 0) timestamp = TilNextMillis(LastTimestamp);
                }
                else
                {
                    Sequence = 0;
                }

                LastTimestamp = timestamp;
                var id = ((timestamp - Epoch) << TimestampShift) |(MachineId << SequenceBits) | Sequence;

                return id;
            }
        }
    }
}
