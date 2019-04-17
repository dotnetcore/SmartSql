// Copyright 2010-2012 Twitter, Inc.
// An object that generates IDs. This is broken into a separate class in case we ever want to support multiple worker threads per process

using System;
using System.Collections.Generic;

namespace SmartSql.IdGenerator
{
    public class SnowflakeId : IIdGenerator
    {
        public const long DEFAULT_TWEPOCH = 1288834974657L;
        private const int DEFAULT_WORKER_ID_BITS = 5;
        private const int DEFAULT_DATACENTER_ID_BITS = 5;
        private const long DEFAULT_MAX_WORKER_ID = -1L ^ (-1L << DEFAULT_WORKER_ID_BITS);
        private const long DEFAULT_MAX_DATACENTER_ID = -1L ^ (-1L << DEFAULT_DATACENTER_ID_BITS);

        private const int MACHINE_ID_BITS = 10;
        private const int SEQUENCE_BITS = 12;
        private const long SEQUENCE_MASK = -1L ^ (-1L << SEQUENCE_BITS);
        private const int TIMESTAMP_SHIFT = SEQUENCE_BITS + MACHINE_ID_BITS;
        private static SnowflakeId _snowflakeId;

        private readonly object _lock = new object();
        private static readonly object S_LOCK = new object();
        private long _lastTimestamp = -1L;
        public SnowflakeId()
        {

        }
        public SnowflakeId(long workerId, long datacenterId, long sequence = 0L, int workIdBits = DEFAULT_WORKER_ID_BITS)
        {
            Init(workerId, datacenterId, sequence, workIdBits);
        }
        public long Twepoch { get; private set; }
        public int WorkerIdBits { get; private set; }
        public int DatacenterIdBits { get; private set; }
        public int DatacenterIdShift { get; private set; }
        public long MaxWorkerId { get; private set; }
        public long MaxDatacenterId { get; private set; }
        public long WorkerId { get; private set; }
        public long DatacenterId { get; private set; }
        public long Sequence { get; private set; }
        public static SnowflakeId Default
        {
            get
            {
                lock (S_LOCK)
                {
                    if (_snowflakeId != null)
                    {
                        return _snowflakeId;
                    }
                    var random = new Random();
                    var workerId = random.Next((int)DEFAULT_MAX_WORKER_ID);
                    var datacenterId = random.Next((int)DEFAULT_MAX_DATACENTER_ID);
                    return _snowflakeId = new SnowflakeId(workerId, datacenterId);
                }
            }
        }

        public virtual long NextId()
        {
            lock (_lock)
            {
                var timestamp = TimeGen();

                if (timestamp < _lastTimestamp)
                    throw new Exception(
                        $"InvalidSystemClock: Clock moved backwards, Refusing to generate id for {_lastTimestamp - timestamp} milliseconds");

                if (_lastTimestamp == timestamp)
                {
                    Sequence = (Sequence + 1) & SEQUENCE_MASK;
                    if (Sequence == 0) timestamp = TilNextMillis(_lastTimestamp);
                }
                else
                {
                    Sequence = 0;
                }

                _lastTimestamp = timestamp;
                var id = ((timestamp - Twepoch) << TIMESTAMP_SHIFT) |
                         (DatacenterId << DatacenterIdShift) |
                         (WorkerId << SEQUENCE_BITS) | Sequence;

                return id;
            }
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

        public void Initialize(IDictionary<string, object> paramters)
        {
            paramters.EnsureValue(nameof(WorkerId), out long workerId);
            paramters.Value(nameof(DatacenterId), out long datacenterId);
            paramters.Value(nameof(Sequence), out long sequence);
            if (!paramters.Value(nameof(WorkerIdBits), out int workIdBits))
            {
                workIdBits = DEFAULT_WORKER_ID_BITS;
            }
            if (!paramters.Value(nameof(Twepoch), out long twepoch))
            {
                twepoch = DEFAULT_TWEPOCH;
            }
            Init(workerId, datacenterId, sequence, workIdBits, twepoch);
        }

        private void Init(long workerId, long datacenterId, long sequence, int workIdBits = DEFAULT_WORKER_ID_BITS, long twepoch = DEFAULT_TWEPOCH)
        {
            var timestamp = TimeGen();
            if (twepoch > timestamp || twepoch < 0)
            {
                throw new ArgumentException($"twepoch:{twepoch} can't be greater than current timestamp {timestamp} or less than 0");
            }
            Twepoch = twepoch;
            WorkerId = workerId;
            DatacenterId = datacenterId;
            Sequence = sequence;
            WorkerIdBits = workIdBits;
            if (workIdBits > MACHINE_ID_BITS)
            {
                throw new ArgumentException($"worker Id bits:{workIdBits} can't be greater than {MACHINE_ID_BITS} or less than 0");
            }
            DatacenterIdBits = MACHINE_ID_BITS - workIdBits;
            MaxWorkerId = -1L ^ (-1L << WorkerIdBits);
            MaxDatacenterId = -1L ^ (-1L << DatacenterIdBits);
            DatacenterIdShift = SEQUENCE_BITS + workIdBits;

            if (workerId > MaxWorkerId || workerId < 0)
            {
                throw new ArgumentException($"worker Id can't be greater than {MaxWorkerId} or less than 0");
            }

            if (datacenterId > MaxDatacenterId || datacenterId < 0)
            {
                throw new ArgumentException($"datacenter Id can't be greater than {MaxDatacenterId} or less than 0");
            }
        }
    }
}