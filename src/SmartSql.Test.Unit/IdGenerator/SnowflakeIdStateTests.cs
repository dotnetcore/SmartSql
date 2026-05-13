using System;
using FluentAssertions;
using Moq;
using SmartSql.IdGenerator;
using Xunit;

namespace SmartSql.Test.Unit.IdGenerator;

public class SnowflakeIdStateTests
{
    private readonly Mock<ISnowflakeId> _mockIdGen;
    private readonly DateTimeOffset _epochTime;
    private readonly long _epoch;

    // Bit layout constants
    private const int MachineIdBits = 10;
    private const int SequenceBits = 12;
    private const int MaxMachineMask = 4095; // 2^12 - 1
    private const int SequenceMask = 4095;   // 2^12 - 1
    private const int TimestampShift = 22;   // MachineIdBits + SequenceBits
    private const long TimestampMask = (1L << 41) - 1; // 2^41 - 1
    private const long MachineId = 1L;

    public SnowflakeIdStateTests()
    {
        _epochTime = new DateTimeOffset(2020, 1, 1, 0, 0, 0, TimeSpan.Zero);
        _epoch = _epochTime.ToUnixTimeMilliseconds();

        _mockIdGen = new Mock<ISnowflakeId>();
        _mockIdGen.SetupGet(x => x.MachineId).Returns(MachineId);
        _mockIdGen.SetupGet(x => x.SequenceBits).Returns(SequenceBits);
        _mockIdGen.SetupGet(x => x.MaxMachineMask).Returns(MaxMachineMask);
        _mockIdGen.SetupGet(x => x.SequenceMask).Returns(SequenceMask);
        _mockIdGen.SetupGet(x => x.TimestampShift).Returns(TimestampShift);
        _mockIdGen.SetupGet(x => x.TimestampMask).Returns(TimestampMask);
        _mockIdGen.SetupGet(x => x.EpochTime).Returns(_epochTime);
        _mockIdGen.SetupGet(x => x.Epoch).Returns(_epoch);
    }

    private long BuildId(long timestamp, long machineId, long sequence)
    {
        var delta = timestamp - _epoch;
        return ((delta & TimestampMask) << TimestampShift)
               | (machineId << SequenceBits)
               | (sequence & SequenceMask);
    }

    [Fact]
    public void Should_CreateFromId_When_ValidIdProvided()
    {
        var utcNow = DateTimeOffset.UtcNow;
        var id = BuildId(utcNow.ToUnixTimeMilliseconds(), MachineId, 42);

        var state = new SnowflakeIdState(id, _mockIdGen.Object);

        state.Id.Should().Be(id);
        state.Sequence.Should().Be(42);
        state.MachineId.Should().Be(MachineId);
        state.UtcTime.Should().BeCloseTo(utcNow, TimeSpan.FromMilliseconds(1));
    }

    [Fact]
    public void Should_CreateFromString_When_ValidStringProvided()
    {
        var utcNow = new DateTimeOffset(2025, 6, 15, 10, 30, 45, 123, TimeSpan.Zero);
        var id = BuildId(utcNow.ToUnixTimeMilliseconds(), MachineId, 5);

        var fromIdState = new SnowflakeIdState(id, _mockIdGen.Object);
        var fromStringState = new SnowflakeIdState(fromIdState.IdString, _mockIdGen.Object);

        fromStringState.Id.Should().Be(id);
        fromStringState.UtcTime.Should().Be(utcNow);
        fromStringState.MachineId.Should().Be(MachineId);
        fromStringState.Sequence.Should().Be(5);
    }

    [Fact]
    public void Should_Throw_When_MachineIdMismatch()
    {
        var utcNow = DateTimeOffset.UtcNow;
        var id = BuildId(utcNow.ToUnixTimeMilliseconds(), machineId: 999, sequence: 1);

        var action = () => new SnowflakeIdState(id, _mockIdGen.Object);

        action.Should().Throw<ArgumentException>()
            .WithMessage("*MachineId*");
    }

    [Fact]
    public void Should_BeEqual_When_SameId()
    {
        var utcNow = DateTimeOffset.UtcNow;
        var id = BuildId(utcNow.ToUnixTimeMilliseconds(), MachineId, 1);

        var state1 = new SnowflakeIdState(id, _mockIdGen.Object);
        var state2 = new SnowflakeIdState(id, _mockIdGen.Object);

        state1.Equals(state2).Should().BeTrue();
        state1.GetHashCode().Should().Be(state2.GetHashCode());
    }

    [Fact]
    public void Should_RoundTrip_When_ConvertingBetweenIdAndString()
    {
        var utcNow = new DateTimeOffset(2025, 3, 20, 12, 0, 0, 500, TimeSpan.Zero);
        var id = BuildId(utcNow.ToUnixTimeMilliseconds(), MachineId, 100);

        var fromId = new SnowflakeIdState(id, _mockIdGen.Object);
        var fromString = new SnowflakeIdState(fromId.IdString, _mockIdGen.Object);

        fromString.Id.Should().Be(fromId.Id);
        fromString.IdString.Should().Be(fromId.IdString);
        fromString.UtcTime.Should().Be(fromId.UtcTime);
        fromString.Sequence.Should().Be(fromId.Sequence);
        fromString.MachineId.Should().Be(fromId.MachineId);
    }
}
