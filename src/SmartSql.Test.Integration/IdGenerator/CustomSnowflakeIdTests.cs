using System.Collections.Generic;
using FluentAssertions;
using SmartSql.IdGenerator;
using Xunit;

namespace SmartSql.Test.Integration.IdGenerator;

public class CustomSnowflakeIdTests
{
    private readonly CustomSnowflakeId _snowflakeId;

    public CustomSnowflakeIdTests()
    {
        _snowflakeId = new CustomSnowflakeId();
        _snowflakeId.Initialize(new Dictionary<string, object>()
        {
            {"MachineId", 1},
            {"MachineIdBits", 5},
            {"SequenceBits", 5},
            {"EpochDate", "2019-05-10"}
        });
    }

    [Fact]
    public void Should_GenerateNonZeroId_When_CallingNextId()
    {
        var id = _snowflakeId.NextId();
        id.Should().BeGreaterThan(0);
    }

    [Fact]
    public void Should_RoundTripId_When_UsingFromIdLong()
    {
        var id = _snowflakeId.NextId();

        var idState = _snowflakeId.FromId(id);

        var toId = _snowflakeId.FromIdState(idState);

        toId.Should().Be(id);
    }

    [Fact]
    public void Should_RoundTripId_When_UsingFromIdString()
    {
        var id = _snowflakeId.NextId();

        var idState = _snowflakeId.FromId(id);

        var toId = _snowflakeId.FromId(idState.IdString);

        toId.Id.Should().Be(id);
    }
}
