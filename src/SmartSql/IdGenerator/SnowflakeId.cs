// Copyright 2010-2012 Twitter, Inc.
// An object that generates IDs. This is broken into a separate class in case we ever want to support multiple worker threads per process

using System;
using System.Collections.Generic;

namespace SmartSql.IdGenerator
{
    public class SnowflakeId : AbstractSnowflakeId
    {
        private const int DEFAULT_WORKER_ID_BITS = 5;
        private const int DEFAULT_DATACENTER_ID_BITS = 5;
        private const long DEFAULT_MAX_WORKER_ID = -1L ^ (-1L << DEFAULT_WORKER_ID_BITS);
        private const long DEFAULT_MAX_DATACENTER_ID = -1L ^ (-1L << DEFAULT_DATACENTER_ID_BITS);

        private static SnowflakeId _snowflakeId;

        private static readonly object S_LOCK = new object();

        public SnowflakeId()
        {
        }

        public SnowflakeId(long workerId, long datacenterId, long sequence = 0L,
            int workIdBits = DEFAULT_WORKER_ID_BITS)
        {
            Init(workerId, datacenterId, sequence, workIdBits);
        }

        public int WorkerIdBits { get; private set; }
        public int DatacenterIdBits { get; private set; }
        public int DatacenterIdShift { get; private set; }
        public long MaxWorkerId { get; private set; }
        public long MaxDatacenterId { get; private set; }
        public long WorkerId { get; private set; }
        public long DatacenterId { get; private set; }

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
                    var workerId = random.Next((int) DEFAULT_MAX_WORKER_ID);
                    var datacenterId = random.Next((int) DEFAULT_MAX_DATACENTER_ID);
                    return _snowflakeId = new SnowflakeId(workerId, datacenterId);
                }
            }
        }


        public override void Initialize(IDictionary<string, object> parameters)
        {
            parameters.EnsureValue(nameof(WorkerId), out long workerId);
            parameters.Value(nameof(DatacenterId), out long datacenterId);
            parameters.Value(nameof(Sequence), out long sequence);
            if (!parameters.Value(nameof(WorkerIdBits), out int workIdBits))
            {
                workIdBits = DEFAULT_WORKER_ID_BITS;
            }

            if (!parameters.Value(nameof(Epoch), out long epoch))
            {
                epoch = DEFAULT_EPOCH;
            }

            Init(workerId, datacenterId, sequence, workIdBits, epoch);
        }

        private void Init(long workerId, long datacenterId, long sequence, int workIdBits = DEFAULT_WORKER_ID_BITS,
            long epoch = DEFAULT_EPOCH)
        {
            WorkerId = workerId;
            DatacenterId = datacenterId;
            WorkerIdBits = workIdBits;
            if (workIdBits > DEFAULT_MACHINE_ID_BITS)
            {
                throw new ArgumentException(
                    $"worker Id bits:{workIdBits} can't be greater than {DEFAULT_MACHINE_ID_BITS} or less than 0");
            }

            DatacenterIdBits = DEFAULT_MACHINE_ID_BITS - workIdBits;
            if (DatacenterIdBits < 1)
            {
                throw new ArgumentException(
                    $"Datacenter Id bits:{DatacenterIdBits} can't be less than 1");
            }

            MaxWorkerId = -1L ^ (-1L << WorkerIdBits);
            MaxDatacenterId = -1L ^ (-1L << DatacenterIdBits);
            DatacenterIdShift = DEFAULT_SEQUENCE_BITS + workIdBits;

            if (workerId > MaxWorkerId || workerId < 0)
            {
                throw new ArgumentException($"worker Id can't be greater than {MaxWorkerId} or less than 0");
            }

            if (datacenterId > MaxDatacenterId || datacenterId < 0)
            {
                throw new ArgumentException($"datacenter Id can't be greater than {MaxDatacenterId} or less than 0");
            }

            var machineId = (DatacenterId << DatacenterIdBits) | WorkerId;
            InitStatus(machineId, sequence, DEFAULT_MACHINE_ID_BITS, DEFAULT_SEQUENCE_BITS, epoch);
        }
    }
}