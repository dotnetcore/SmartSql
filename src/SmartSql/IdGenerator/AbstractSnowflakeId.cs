using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace SmartSql.IdGenerator
{
    public abstract class AbstractSnowflakeId : ISnowflakeId
    {
        public const long TWEPOCH = 1288834974657L;
        public const long DEFAULT_EPOCH = TWEPOCH;
        public const int DEFAULT_TOTAL_BITS = 63;
        public const int DEFAULT_MACHINE_ID_BITS = 10;
        public const int DEFAULT_SEQUENCE_BITS = 12;
        public int MachineIdBits { get; protected set; }
        public int SequenceBits { get; protected set; }
        public long Epoch { get; protected set; }
        public DateTimeOffset EpochTime { get; protected set; }
        public long MachineId { get; protected set; }
        public long Sequence { get; protected set; }
        public int MaxMachineId { get; protected set; }
        public int SequenceMask { get; protected set; }
        public int TimestampShift { get; protected set; }
        public long TimestampMask { get; protected set; }
        public long LastTimestamp { get; protected set; } = -1L;

        public virtual void Initialize(IDictionary<string, object> parameters)
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
                if (!DateTime.TryParse(epochDateStr, out var epochDate))
                {
                    throw new ArgumentException($"EpochDate:[{epochDateStr}] can not convert to DateTime.");
                }
                
                epoch = new DateTimeOffset(epochDate).ToUnixTimeMilliseconds();
            }

            InitStatus(machineId, sequence, machineIdBits, sequenceBits, epoch);
        }

        protected void InitStatus(long machineId, long sequence
            , int machineIdBits = DEFAULT_MACHINE_ID_BITS
            , int sequenceBits = DEFAULT_SEQUENCE_BITS
            , long epoch = DEFAULT_EPOCH)
        {
            var timestamp = TimeGen();
            if (epoch > timestamp || epoch < 0)
            {
                throw new ArgumentException(
                    $"epoch:{epoch} can't be greater than current timestamp {timestamp} or less than 0");
            }

            Epoch = epoch;

            EpochTime = DateTimeOffset.FromUnixTimeMilliseconds(epoch);
            MachineId = machineId;
            Sequence = sequence;
            MachineIdBits = machineIdBits;
            MaxMachineId = (int) GetMask(MachineIdBits);
            SequenceBits = sequenceBits;
            SequenceMask = (int) GetMask(sequenceBits);
            TimestampShift = SequenceBits + MachineIdBits;
            TimestampMask = GetMask(DEFAULT_TOTAL_BITS - TimestampShift);
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
                var id = ((timestamp - Epoch) << TimestampShift) | (MachineId << SequenceBits) | Sequence;

                return id;
            }
        }


        public long TilNextMillis(long lastTimestamp)
        {
            var timestamp = TimeGen();
            while (timestamp <= lastTimestamp) timestamp = TimeGen();
            return timestamp;
        }

        public long TimeGen()
        {
            return DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        }

        public SnowflakeIdState FromId(long snowflakeId)
        {
            return new SnowflakeIdState
            {
                Sequence = snowflakeId & SequenceMask,
                MachineId = (snowflakeId >> SequenceBits) & MaxMachineId,
                Time = EpochTime.AddMilliseconds((snowflakeId >> TimestampShift) & TimestampMask).DateTime
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static long GetMask(int bits)
        {
            return -1L ^ (-1L << bits);
        }
    }
}