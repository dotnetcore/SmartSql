using System;
using System.Collections.Generic;
using FluentAssertions;
using SmartSql.Exceptions;
using SmartSql.IdGenerator;
using Xunit;

namespace SmartSql.Test.Unit.IdGenerator;

public class IdGeneratorBuilderTests
{
    private readonly IdGeneratorBuilder _builder = new IdGeneratorBuilder();

    [Fact]
    public void Should_BuildSnowflakeId_When_TypeIsSnowflakeId()
    {
        var parameters = new Dictionary<string, object>
        {
            { "WorkerId", 1L },
            { "DatacenterId", 1L }
        };

        var result = _builder.Build("SnowflakeId", parameters);

        result.Should().BeOfType<SnowflakeId>();
    }

    [Fact]
    public void Should_ReturnDefaultSnowflakeId_When_SnowflakeIdWithNullParameters()
    {
        var result = _builder.Build("SnowflakeId", null);

        result.Should().BeOfType<SnowflakeId>();
        result.Should().BeSameAs(SnowflakeId.Default);
    }

    [Fact]
    public void Should_BuildCustomSnowflakeId_When_TypeIsCustomSnowflakeId()
    {
        var parameters = new Dictionary<string, object>
        {
            { "MachineId", 1L },
            { "Sequence", 0L }
        };

        var result = _builder.Build("CustomSnowflakeId", parameters);

        result.Should().BeOfType<CustomSnowflakeId>();
    }

    [Fact]
    public void Should_BuildDbSequence_When_TypeIsDbSequence()
    {
        var parameters = new Dictionary<string, object>
        {
            { "Step", 100 },
            { "SequenceSql", "SELECT NEXT VALUE FOR TestSeq" }
        };

        var result = _builder.Build("DbSequence", parameters);

        result.Should().BeOfType<DbSequence>();
    }

    [Fact]
    public void Should_InitializeSnowflakeId_When_ParametersProvided()
    {
        var parameters = new Dictionary<string, object>
        {
            { "WorkerId", 5L },
            { "DatacenterId", 3L },
            { "Sequence", 10L }
        };

        var result = _builder.Build("SnowflakeId", parameters);

        var snowflakeId = result as SnowflakeId;
        snowflakeId.Should().NotBeNull();
        snowflakeId.WorkerId.Should().Be(5);
        snowflakeId.DatacenterId.Should().Be(3);
    }

    [Fact]
    public void Should_GenerateUniqueIds_When_MultipleCalls()
    {
        var parameters = new Dictionary<string, object>
        {
            { "WorkerId", 1L },
            { "DatacenterId", 1L }
        };

        var idGenerator = _builder.Build("SnowflakeId", parameters);

        var id1 = idGenerator.NextId();
        var id2 = idGenerator.NextId();
        var id3 = idGenerator.NextId();

        id1.Should().NotBe(id2);
        id2.Should().NotBe(id3);
        id1.Should().NotBe(id3);
    }

    [Fact]
    public void Should_Throw_When_TypeIsUnknown()
    {
        var parameters = new Dictionary<string, object>();

        var act = () => _builder.Build("UnknownIdGenerator", parameters);

        act.Should().Throw<Exception>();
    }

    [Fact]
    public void Should_BuildCustomSnowflakeId_WithDefaultSequence()
    {
        var parameters = new Dictionary<string, object>
        {
            { "MachineId", 10L }
        };

        var result = _builder.Build("CustomSnowflakeId", parameters);

        result.Should().BeOfType<CustomSnowflakeId>();
        var id = result.NextId();
        id.Should().BeGreaterThan(0);
    }

    [Fact]
    public void Should_BuildDbSequence_WithDefaultParameters()
    {
        var parameters = new Dictionary<string, object>
        {
            { "Step", 1000 },
            { "SequenceSql", "SELECT SEQ.NEXTVAL FROM DUAL" }
        };

        var result = _builder.Build("DbSequence", parameters);

        result.Should().BeOfType<DbSequence>();
    }

    [Fact]
    public void Should_InitializeIdGenerator_WithEmptyParameters()
    {
        var parameters = new Dictionary<string, object>();

        var act = () => _builder.Build("DbSequence", parameters);

        act.Should().Throw<SmartSqlException>();
    }

    [Fact]
    public void Should_HandleNullParameters_ForSnowflakeId()
    {
        var result = _builder.Build("SnowflakeId", null);

        result.Should().BeOfType<SnowflakeId>();
        result.Should().BeSameAs(SnowflakeId.Default);
    }

    [Fact]
    public void Should_BuildCustomSnowflakeId_WithEpoch()
    {
        var parameters = new Dictionary<string, object>
        {
            { "MachineId", 5L },
            { "Sequence", 0L },
            { "Epoch", 1609459200000L } // 2021-01-01
        };

        var result = _builder.Build("CustomSnowflakeId", parameters);

        result.Should().BeOfType<CustomSnowflakeId>();
        var id = result.NextId();
        id.Should().BeGreaterThan(0);
    }
}
