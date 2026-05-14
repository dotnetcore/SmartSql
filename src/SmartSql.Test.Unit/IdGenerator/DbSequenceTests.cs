using System;
using System.Collections.Generic;
using System.Reflection;
using FluentAssertions;
using Moq;
using SmartSql.Configuration;
using SmartSql.DataSource;
using SmartSql.DbSession;
using SmartSql.Exceptions;
using SmartSql.IdGenerator;
using Xunit;

namespace SmartSql.Test.Unit.IdGenerator;

public class DbSequenceTests
{
    private static DbSequence CreateDbSequence(
        int step = 100,
        string sequenceSql = "SELECT NEXT VALUE FOR TestSeq",
        ISqlMapper sqlMapper = null)
    {
        var dbSequence = new DbSequence();
        var parameters = new Dictionary<string, object>
        {
            { "Step", step },
            { "SequenceSql", sequenceSql }
        };
        dbSequence.Initialize(parameters);

        if (sqlMapper != null)
        {
            SetSqlMapper(dbSequence, sqlMapper);
        }

        return dbSequence;
    }

    private static void SetSqlMapper(DbSequence dbSequence, ISqlMapper sqlMapper)
    {
        var field = typeof(DbSequence).GetField("_sqlMapper",
            BindingFlags.NonPublic | BindingFlags.Instance);
        field.SetValue(dbSequence, sqlMapper);
    }

    [Fact]
    public void Should_Initialize_WithStep_When_ParametersProvided()
    {
        var parameters = new Dictionary<string, object>
        {
            { "Step", 50 },
            { "SequenceSql", "SELECT NEXT VALUE FOR TestSeq" }
        };

        var dbSequence = new DbSequence();
        dbSequence.Initialize(parameters);

        dbSequence.Should().NotBeNull();
    }

    [Fact]
    public void Should_Throw_When_StepNotProvided()
    {
        var parameters = new Dictionary<string, object>
        {
            { "SequenceSql", "SELECT NEXT VALUE FOR TestSeq" }
        };

        var dbSequence = new DbSequence();
        var act = () => dbSequence.Initialize(parameters);

        act.Should().Throw<SmartSqlException>();
    }

    [Fact]
    public void Should_Throw_When_SequenceSqlNotProvided()
    {
        var parameters = new Dictionary<string, object>
        {
            { "Step", 100 }
        };

        var dbSequence = new DbSequence();
        var act = () => dbSequence.Initialize(parameters);

        act.Should().Throw<SmartSqlException>();
    }

    [Fact]
    public void Should_SetupSqlMapper_When_SetupSmartSqlCalled()
    {
        var mockSqlMapper = new Mock<ISqlMapper>();
        var config = new SmartSqlConfig();
        var builder = new SmartSqlBuilder();
        var configProp = typeof(SmartSqlBuilder).GetProperty("SmartSqlConfig");
        configProp.SetValue(builder, config);
        var sqlMapperProp = typeof(SmartSqlBuilder).GetProperty("SqlMapper");
        sqlMapperProp.SetValue(builder, mockSqlMapper.Object);

        var dbSequence = CreateDbSequence();

        dbSequence.SetupSmartSql(builder);

        var field = typeof(DbSequence).GetField("_sqlMapper",
            BindingFlags.NonPublic | BindingFlags.Instance);
        field.GetValue(dbSequence).Should().BeSameAs(mockSqlMapper.Object);
    }

    [Fact]
    public void Should_ReturnIdFromSequence_When_NextIdCalled()
    {
        var mockSqlMapper = new Mock<ISqlMapper>();
        mockSqlMapper.Setup(m => m.ExecuteScalar<long>(It.IsAny<RequestContext>()))
            .Returns(1000L);

        var dbSequence = CreateDbSequence(100, "SELECT NEXT VALUE FOR TestSeq", mockSqlMapper.Object);

        var nextId = dbSequence.NextId();

        nextId.Should().Be(1000);
    }

    [Fact]
    public void Should_IncrementWithinBatch_When_NextIdCalledMultipleTimes()
    {
        var callCount = 0;
        var mockSqlMapper = new Mock<ISqlMapper>();
        mockSqlMapper.Setup(m => m.ExecuteScalar<long>(It.IsAny<RequestContext>()))
            .Returns(() => 1000L + (callCount++ * 100));

        var dbSequence = CreateDbSequence(100, "SELECT NEXT VALUE FOR TestSeq", mockSqlMapper.Object);

        var id1 = dbSequence.NextId();
        var id2 = dbSequence.NextId();
        var id3 = dbSequence.NextId();

        id1.Should().Be(1000);
        id2.Should().Be(1001);
        id3.Should().Be(1002);
    }

    [Fact]
    public void Should_FetchNewBatch_When_ExhaustingCurrentBatch()
    {
        var callCount = 0;
        var mockSqlMapper = new Mock<ISqlMapper>();
        mockSqlMapper.Setup(m => m.ExecuteScalar<long>(It.IsAny<RequestContext>()))
            .Returns(() => 1000L + (callCount++ * 100));

        var dbSequence = CreateDbSequence(100, "SELECT NEXT VALUE FOR TestSeq", mockSqlMapper.Object);

        // First call triggers initial fetch (batch 1: 1000-1099)
        var firstId = dbSequence.NextId();
        firstId.Should().Be(1000);

        // Exhaust the rest of batch 1
        for (int i = 1; i < 100; i++)
        {
            dbSequence.NextId();
        }

        // Next call should trigger a new batch fetch (batch 2: 1100-1199)
        var nextId = dbSequence.NextId();

        nextId.Should().Be(1100);
        mockSqlMapper.Verify(m => m.ExecuteScalar<long>(It.IsAny<RequestContext>()), Times.Exactly(2));
    }

    [Fact]
    public void Should_BeThreadSafe_When_NextIdCalledConcurrently()
    {
        var mockSqlMapper = new Mock<ISqlMapper>();
        var callCount = 0;
        var lockObj = new object();

        mockSqlMapper.Setup(m => m.ExecuteScalar<long>(It.IsAny<RequestContext>()))
            .Returns(() =>
            {
                lock (lockObj)
                {
                    return 1000L + (callCount++ * 100);
                }
            });

        var dbSequence = CreateDbSequence(100, "SELECT NEXT VALUE FOR TestSeq", mockSqlMapper.Object);

        var ids = new System.Collections.Concurrent.ConcurrentBag<long>();
        var tasks = new List<System.Threading.Tasks.Task>();

        for (int i = 0; i < 10; i++)
        {
            tasks.Add(System.Threading.Tasks.Task.Run(() =>
            {
                for (int j = 0; j < 10; j++)
                {
                    ids.Add(dbSequence.NextId());
                }
            }));
        }

        System.Threading.Tasks.Task.WaitAll(tasks.ToArray());

        ids.Should().HaveCount(100);
        ids.Should().OnlyHaveUniqueItems();
    }

    [Fact]
    public void Should_HandleZeroStep_When_Initialize()
    {
        var parameters = new Dictionary<string, object>
        {
            { "Step", 0 },
            { "SequenceSql", "SELECT NEXT VALUE FOR TestSeq" }
        };

        var dbSequence = new DbSequence();
        dbSequence.Initialize(parameters);

        dbSequence.Should().NotBeNull();
    }

    [Fact]
    public void Should_UseCorrectSequenceSql_When_ExecuteScalarCalled()
    {
        RequestContext capturedContext = null;
        var mockSqlMapper = new Mock<ISqlMapper>();

        mockSqlMapper.Setup(m => m.ExecuteScalar<long>(It.IsAny<AbstractRequestContext>()))
            .Callback<AbstractRequestContext>(ctx => capturedContext = ctx as RequestContext)
            .Returns(500L);

        var expectedSql = "SELECT NEXT VALUE FOR TestSeq";
        var dbSequence = CreateDbSequence(100, expectedSql, mockSqlMapper.Object);

        dbSequence.NextId();

        capturedContext.Should().NotBeNull();
        capturedContext.RealSql.Should().Be(expectedSql);
    }
}
