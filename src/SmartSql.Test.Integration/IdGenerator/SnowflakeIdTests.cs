using System;
using FluentAssertions;
using SmartSql.IdGenerator;
using Xunit;

namespace SmartSql.Test.Integration.IdGenerator;

public class SnowflakeIdTests
{
    [Fact]
    public void Should_GenerateValidId_When_CallingNextId()
    {
        var id = SnowflakeId.Default.NextId();
        id.Should().BeGreaterThan(0);
        var idState = SnowflakeId.Default.FromId(id);
        idState.Id.Should().Be(id);
        idState.UtcTime.Date.Should().Be(DateTime.UtcNow.Date);
    }
}
