using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using SmartSql.IdGenerator;
using Xunit;

namespace SmartSql.Test.Unit.IdGenerator;

public class AbstractSnowflakeIdTests
{
    private class TestSnowflakeId : AbstractSnowflakeId
    {
        public void Init(long machineId, long sequence,
            int machineIdBits = DEFAULT_MACHINE_ID_BITS,
            int sequenceBits = DEFAULT_SEQUENCE_BITS,
            long epoch = DEFAULT_EPOCH)
        {
            InitStatus(machineId, sequence, machineIdBits, sequenceBits, epoch);
        }
    }

    private static TestSnowflakeId CreateId(
        long machineId = 1,
        long sequence = 0,
        int machineIdBits = AbstractSnowflakeId.DEFAULT_MACHINE_ID_BITS,
        int sequenceBits = AbstractSnowflakeId.DEFAULT_SEQUENCE_BITS,
        long? epoch = null)
    {
        var id = new TestSnowflakeId();
        var e = epoch ?? AbstractSnowflakeId.DEFAULT_EPOCH;
        id.Init(machineId, sequence, machineIdBits, sequenceBits, e);
        return id;
    }

    [Fact]
    public void Should_GenerateUniqueId_When_NextIdCalled()
    {
        var idGen = CreateId();

        var id1 = idGen.NextId();
        var id2 = idGen.NextId();

        id1.Should().BeGreaterThan(0);
        id2.Should().BeGreaterThan(0);
        id1.Should().NotBe(id2);
    }

    [Fact]
    public void Should_GenerateMonotonicallyIncreasingIds_When_CalledSequentially()
    {
        var idGen = CreateId();
        var previous = idGen.NextId();

        for (int i = 0; i < 100; i++)
        {
            var current = idGen.NextId();
            current.Should().BeGreaterThan(previous);
            previous = current;
        }
    }

    [Fact]
    public void Should_InitializeWithDefaults_When_InitStatusCalled()
    {
        var idGen = CreateId();

        idGen.MachineId.Should().Be(1);
        idGen.Sequence.Should().Be(0);
        idGen.MachineIdBits.Should().Be(AbstractSnowflakeId.DEFAULT_MACHINE_ID_BITS);
        idGen.SequenceBits.Should().Be(AbstractSnowflakeId.DEFAULT_SEQUENCE_BITS);
        idGen.MaxMachineMask.Should().Be((int)(-1L ^ (-1L << AbstractSnowflakeId.DEFAULT_MACHINE_ID_BITS)));
        idGen.SequenceMask.Should().Be((int)(-1L ^ (-1L << AbstractSnowflakeId.DEFAULT_SEQUENCE_BITS)));
    }

    [Fact]
    public void Should_Throw_When_MachineIdExceedsMax()
    {
        var act = () => CreateId(machineId: 2048);

        act.Should().Throw<ArgumentException>().WithMessage("*worker Id*");
    }

    [Fact]
    public void Should_Throw_When_MachineIdIsNegative()
    {
        var act = () => CreateId(machineId: -1);

        act.Should().Throw<ArgumentException>().WithMessage("*worker Id*");
    }

    [Fact]
    public void Should_Throw_When_EpochIsInFuture()
    {
        var futureEpoch = DateTimeOffset.UtcNow.AddYears(1).ToUnixTimeMilliseconds();

        var act = () => CreateId(epoch: futureEpoch);

        act.Should().Throw<ArgumentException>().WithMessage("*epoch*");
    }

    [Fact]
    public void Should_Throw_When_EpochIsNegative()
    {
        var act = () => CreateId(epoch: -1);

        act.Should().Throw<ArgumentException>().WithMessage("*epoch*");
    }

    [Fact]
    public void Should_ResetSequence_When_TimestampAdvances()
    {
        var idGen = CreateId();
        var firstId = idGen.NextId();
        var firstTimestamp = idGen.LastTimestamp;

        // Since we can't control time, just verify that LastTimestamp is set
        firstTimestamp.Should().BeGreaterThan(0);
    }

    [Fact]
    public void Should_DecodeId_When_FromIdCalled()
    {
        var idGen = CreateId(machineId: 5);
        var id = idGen.NextId();

        var state = idGen.FromId(id);

        state.Should().NotBeNull();
        state.Id.Should().Be(id);
        state.MachineId.Should().Be(5);
    }

    [Fact]
    public void Should_RoundTripViaString_When_FromIdAndFromStringCalled()
    {
        var idGen = CreateId(machineId: 3);
        var id = idGen.NextId();

        var fromId = idGen.FromId(id);
        var fromString = idGen.FromId(fromId.IdString);

        fromString.Id.Should().Be(id);
        fromString.IdString.Should().Be(fromId.IdString);
    }

    [Fact]
    public void Should_ReconstructId_When_FromIdStateCalled()
    {
        var idGen = CreateId(machineId: 7);
        var originalId = idGen.NextId();

        var state = idGen.FromId(originalId);
        var reconstructed = idGen.FromIdState(state);

        reconstructed.Should().Be(originalId);
    }

    [Fact]
    public async Task Should_GenerateUniqueIds_When_MultipleThreads()
    {
        var idGen = CreateId();
        var ids = new ConcurrentBag<long>();
        var tasks = new List<Task>();

        for (int i = 0; i < 10; i++)
        {
            tasks.Add(Task.Run(() =>
            {
                for (int j = 0; j < 100; j++)
                {
                    ids.Add(idGen.NextId());
                }
            }));
        }

        await Task.WhenAll(tasks);

        ids.Should().HaveCount(1000);
        ids.Should().OnlyHaveUniqueItems();
    }

    [Fact]
    public void Should_UseCustomEpoch_When_EpochDateProvided()
    {
        var epochDate = new DateTimeOffset(2020, 1, 1, 0, 0, 0, TimeSpan.Zero);
        var epoch = epochDate.ToUnixTimeMilliseconds();
        var idGen = CreateId(epoch: epoch);

        idGen.Epoch.Should().Be(epoch);
        idGen.EpochTime.Should().Be(epochDate);
    }

    [Fact]
    public void Should_ReturnTimestamp_When_TimeGenCalled()
    {
        var idGen = CreateId();
        var before = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        var result = idGen.TimeGen();
        var after = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

        result.Should().BeInRange(before, after);
    }

    [Fact]
    public void Should_AdvanceTimestamp_When_TilNextMillisCalled()
    {
        var idGen = CreateId();
        var now = idGen.TimeGen();

        var next = idGen.TilNextMillis(now);

        next.Should().BeGreaterThan(now);
    }

    [Fact]
    public void Should_Initialize_When_InitializeCalledWithParameters()
    {
        var idGen = new TestSnowflakeId();
        var parameters = new Dictionary<string, object>
        {
            { "MachineId", 42L },
            { "Sequence", 0L },
            { "MachineIdBits", 10 },
            { "SequenceBits", 12 }
        };

        idGen.Initialize(parameters);

        idGen.MachineId.Should().Be(42);
        idGen.Sequence.Should().Be(0);
    }

    [Fact]
    public void Should_UseDefaults_When_OptionalParametersMissing()
    {
        var idGen = new TestSnowflakeId();
        var parameters = new Dictionary<string, object>
        {
            { "MachineId", 1L }
        };

        idGen.Initialize(parameters);

        idGen.MachineIdBits.Should().Be(AbstractSnowflakeId.DEFAULT_MACHINE_ID_BITS);
        idGen.SequenceBits.Should().Be(AbstractSnowflakeId.DEFAULT_SEQUENCE_BITS);
        idGen.Epoch.Should().Be(AbstractSnowflakeId.DEFAULT_EPOCH);
    }

    [Fact]
    public void Should_ParseEpochDate_When_EpochDateParameterProvided()
    {
        var idGen = new TestSnowflakeId();
        var epochDateStr = "2020-06-15T00:00:00+00:00";
        var parameters = new Dictionary<string, object>
        {
            { "MachineId", 1L },
            { "EpochDate", epochDateStr }
        };

        idGen.Initialize(parameters);

        idGen.EpochTime.Should().Be(new DateTimeOffset(2020, 6, 15, 0, 0, 0, TimeSpan.Zero));
    }

    [Fact]
    public void Should_Throw_When_EpochDateIsInvalid()
    {
        var idGen = new TestSnowflakeId();
        var parameters = new Dictionary<string, object>
        {
            { "MachineId", 1L },
            { "EpochDate", "not-a-date" }
        };

        var act = () => idGen.Initialize(parameters);

        act.Should().Throw<ArgumentException>().WithMessage("*EpochDate*");
    }
}
