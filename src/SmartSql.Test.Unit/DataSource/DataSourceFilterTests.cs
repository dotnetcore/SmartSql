using System;
using System.Collections.Generic;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using SmartSql.Configuration;
using SmartSql.DataSource;
using SmartSql.DbSession;
using SmartSql.Exceptions;
using Xunit;

namespace SmartSql.Test.Unit.DataSource;

public class DataSourceFilterTests
{
    private readonly DataSourceFilter _filter = new(NullLoggerFactory.Instance);

    private static WriteDataSource CreateWriteDataSource(string name = "Write")
    {
        return new WriteDataSource
        {
            Name = name,
            ConnectionString = "Server=localhost;Database=test;",
            DbProvider = DbProviderManager.SQLITE_DBPROVIDER
        };
    }

    private static ReadDataSource CreateReadDataSource(string name = "Read-1", int weight = 1)
    {
        return new ReadDataSource
        {
            Name = name,
            ConnectionString = "Server=localhost;Database=test_read;",
            DbProvider = DbProviderManager.SQLITE_DBPROVIDER,
            Weight = weight
        };
    }

    private static SmartSqlConfig CreateConfig(
        WriteDataSource write = null,
        IDictionary<string, ReadDataSource> reads = null)
    {
        return new SmartSqlConfig
        {
            Database = new Database
            {
                DbProvider = DbProviderManager.SQLITE_DBPROVIDER,
                Write = write ?? CreateWriteDataSource(),
                Reads = reads
            }
        };
    }

    private static AbstractRequestContext CreateRequestContext(
        SmartSqlConfig config,
        DataSourceChoice choice = DataSourceChoice.Write,
        string readDb = null)
    {
        var request = new RequestContext<object>
        {
            DataSourceChoice = choice,
            ReadDb = readDb,
            ExecutionContext = new ExecutionContext
            {
                SmartSqlConfig = config
            }
        };
        return request;
    }

    [Fact]
    public void Should_ReturnWriteDataSource_When_ChoiceIsWrite()
    {
        var write = CreateWriteDataSource();
        var config = CreateConfig(write);
        var context = CreateRequestContext(config, DataSourceChoice.Write);

        var result = _filter.Elect(context);

        result.Should().BeSameAs(write);
    }

    [Fact]
    public void Should_ReturnWriteDataSource_When_ChoiceIsReadButNoReadSources()
    {
        var write = CreateWriteDataSource();
        var config = CreateConfig(write, reads: null);
        var context = CreateRequestContext(config, DataSourceChoice.Read);

        var result = _filter.Elect(context);

        result.Should().BeSameAs(write);
    }

    [Fact]
    public void Should_ReturnWriteDataSource_When_ChoiceIsReadButEmptyReadSources()
    {
        var write = CreateWriteDataSource();
        var config = CreateConfig(write, reads: new Dictionary<string, ReadDataSource>());
        var context = CreateRequestContext(config, DataSourceChoice.Read);

        var result = _filter.Elect(context);

        result.Should().BeSameAs(write);
    }

    [Fact]
    public void Should_ReturnWriteDataSource_When_ChoiceIsUnknown()
    {
        var write = CreateWriteDataSource();
        var config = CreateConfig(write);
        var context = CreateRequestContext(config, DataSourceChoice.Unknow);

        var result = _filter.Elect(context);

        result.Should().BeSameAs(write);
    }

    [Fact]
    public void Should_ReturnReadDataSource_When_ChoiceIsReadAndReadSourcesAvailable()
    {
        var write = CreateWriteDataSource();
        var read = CreateReadDataSource("Read-1");
        var config = CreateConfig(write,
            new Dictionary<string, ReadDataSource> { { "Read-1", read } });
        var context = CreateRequestContext(config, DataSourceChoice.Read);

        var result = _filter.Elect(context);

        result.Should().BeSameAs(read);
    }

    [Fact]
    public void Should_ReturnNamedReadDataSource_When_ReadDbSpecified()
    {
        var write = CreateWriteDataSource();
        var read1 = CreateReadDataSource("Read-1");
        var read2 = CreateReadDataSource("Read-2");
        var config = CreateConfig(write,
            new Dictionary<string, ReadDataSource>
            {
                { "Read-1", read1 },
                { "Read-2", read2 }
            });
        var context = CreateRequestContext(config, DataSourceChoice.Read, readDb: "Read-2");

        var result = _filter.Elect(context);

        result.Should().BeSameAs(read2);
    }

    [Fact]
    public void Should_Throw_When_ReadDbNotFound()
    {
        var write = CreateWriteDataSource();
        var read = CreateReadDataSource("Read-1");
        var config = CreateConfig(write,
            new Dictionary<string, ReadDataSource> { { "Read-1", read } });
        var context = CreateRequestContext(config, DataSourceChoice.Read, readDb: "NonExistent");

        var act = () => _filter.Elect(context);

        act.Should().Throw<SmartSqlException>()
            .WithMessage("*NonExistent*");
    }

    [Fact]
    public void Should_ReturnSessionDataSource_When_LocalSessionHasDataSource()
    {
        var write = CreateWriteDataSource();
        var sessionDataSource = CreateReadDataSource("SessionDS");
        var config = CreateConfig(write);

        var mockSession = new Mock<IDbSession>();
        mockSession.Setup(s => s.DataSource).Returns(sessionDataSource);

        var mockSessionStore = new Mock<IDbSessionStore>();
        mockSessionStore.Setup(s => s.LocalSession).Returns(mockSession.Object);
        config.SessionStore = mockSessionStore.Object;

        var context = CreateRequestContext(config, DataSourceChoice.Write);

        var result = _filter.Elect(context);

        result.Should().BeSameAs(sessionDataSource);
    }

    [Fact]
    public void Should_UseWeightedElection_When_MultipleReadSources()
    {
        var write = CreateWriteDataSource();
        var read1 = CreateReadDataSource("Read-1", weight: 1);
        var read2 = CreateReadDataSource("Read-2", weight: 1);
        var config = CreateConfig(write,
            new Dictionary<string, ReadDataSource>
            {
                { "Read-1", read1 },
                { "Read-2", read2 }
            });
        var context = CreateRequestContext(config, DataSourceChoice.Read);

        var result = _filter.Elect(context);

        result.Should().Match<AbstractDataSource>(ds => ds.Name == "Read-1" || ds.Name == "Read-2");
    }
}
