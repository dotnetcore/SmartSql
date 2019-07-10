using System;

namespace SmartSql.IdGenerator
{
    public class SnowflakeIdState
    {
        private const string TIME_FORMAT = "yyyyMMddHHmmssfff";
        private readonly ISnowflakeId _idGen;

        public SnowflakeIdState(long id, ISnowflakeId idGen)
        {
            Id = id;
            _idGen = idGen;
            MachineIdLength = _idGen.MaxMachineMask.ToString().Length;
            SequenceLength = _idGen.SequenceMask.ToString().Length;

            Sequence = Id & _idGen.SequenceMask;

            MachineId = (Id >> _idGen.SequenceBits) & _idGen.MaxMachineMask;
            if (MachineId != _idGen.MachineId)
            {
                throw new ArgumentException($"MachineId:[{MachineId}] not Equal IdGen.MachineId:[{_idGen.MachineId}]",
                    nameof(MachineId));
            }

            UtcTime = _idGen.EpochTime.AddMilliseconds((Id >> _idGen.TimestampShift) & _idGen.TimestampMask);
            var machineIdFormat = $"D{MachineIdLength}";
            var sequenceFormat = $"D{SequenceLength}";
            IdString =
                $"{UtcTime.ToString(TIME_FORMAT)}{MachineId.ToString(machineIdFormat)}{Sequence.ToString(sequenceFormat)}";
        }

        public SnowflakeIdState(String idString, ISnowflakeId idGen)
        {
            IdString = idString;
            _idGen = idGen;
            MachineIdLength = _idGen.MaxMachineMask.ToString().Length;
            SequenceLength = _idGen.SequenceMask.ToString().Length;

            #region UTC Convert

            var year = int.Parse(idString.Substring(0, 4));
            var month = int.Parse(idString.Substring(4, 2));
            var day = int.Parse(idString.Substring(6, 2));
            var hour = int.Parse(idString.Substring(8, 2));
            var minute = int.Parse(idString.Substring(10, 2));
            var second = int.Parse(idString.Substring(12, 2));
            var millisecond = int.Parse(idString.Substring(14, 3));
            UtcTime = new DateTimeOffset(year, month, day, hour, minute, second, millisecond, TimeSpan.Zero);

            #endregion

            var machineIdString = idString.Substring(17, MachineIdLength);
            MachineId = long.Parse(machineIdString);
            if (MachineId != _idGen.MachineId)
            {
                throw new ArgumentException($"MachineId:[{MachineId}] not Equal IdGen.MachineId:[{_idGen.MachineId}]",
                    nameof(MachineId));
            }

            var sequenceString = idString.Substring(17 + MachineIdLength, SequenceLength);
            Sequence = long.Parse(sequenceString);
            var timestamp = UtcTime.ToUnixTimeMilliseconds();
            Id = ((timestamp - _idGen.Epoch) << _idGen.TimestampShift) | (MachineId << _idGen.SequenceBits) | Sequence;
        }

        public long Id { get; }
        public String IdString { get; }
        public DateTimeOffset UtcTime { get; }
        public long MachineId { get; }
        public int MachineIdLength { get; }
        public long Sequence { get; }
        public int SequenceLength { get; }

        public override string ToString()
        {
            return IdString;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            switch (obj)
            {
                case null:
                {
                    return false;
                }

                case SnowflakeIdState compareId:
                {
                    return Id == compareId.Id;
                }

                case long compareId:
                {
                    return Id == compareId;
                }

                default:
                {
                    return false;
                }
            }
        }
    }
}