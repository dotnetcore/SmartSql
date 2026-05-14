using System;
using System.IO;
using System.Linq;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using SmartSql.DataSource;
using SmartSql.DyRepository;
using SmartSql.Test.Repositories;
using Xunit;

namespace SmartSql.Test.Integration.DyRepository;

public class RepositoryBuilderTests
{
    protected ISqlMapper SqlMapper { get; }

    IRepositoryBuilder _repositoryBuilder;
    IRepositoryFactory _repositoryFactory;

    public RepositoryBuilderTests()
    {
        var loggerFactory = new LoggerFactory(Enumerable.Empty<ILoggerProvider>(),
            new LoggerFilterOptions { MinLevel = LogLevel.Debug });
        var logPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs",
            "SmartSql-RepositoryBuilderTests.log");
        loggerFactory.AddFile(logPath, LogLevel.Trace);

        var smartSqlBuilder = new SmartSqlBuilder()
            .UseXmlConfig()
            .UseLoggerFactory(loggerFactory)
            .UseAlias(nameof(RepositoryBuilderTests) + Guid.NewGuid())
            .AddFilter<TestPrepareStatementFilter>()
            .Build();
        SqlMapper = smartSqlBuilder.SqlMapper;
        _repositoryBuilder = new EmitRepositoryBuilder(null, null,
            loggerFactory.CreateLogger<EmitRepositoryBuilder>());
        _repositoryFactory = new RepositoryFactory(_repositoryBuilder,
            loggerFactory.CreateLogger<RepositoryFactory>());
    }

    [Fact]
    public void Should_BuildRepositoryType_When_CallingBuild()
    {
        var repositoryImplType =
            _repositoryBuilder.Build(typeof(IAllPrimitiveRepository), SqlMapper.SmartSqlConfig);
    }

    [Fact]
    public void Should_CreateAndUseInstance_When_CallingCreateInstance()
    {
        var repository =
            _repositoryFactory.CreateInstance(typeof(IAllPrimitiveRepository),
                SqlMapper) as IAllPrimitiveRepository;
        repository.IsDyRepository().Should().BeTrue();
        var list = repository.Query(10);
        var id = repository.Insert(new Entities.AllPrimitive
        {
            String = "",
            DateTime = DateTime.Now
        });
    }

    [Fact]
    public void Should_InsertEntity_When_UsingAnnotationTransaction()
    {
        var repository =
            _repositoryFactory.CreateInstance(typeof(IAllPrimitiveRepository),
                SqlMapper) as IAllPrimitiveRepository;
        var id = repository.InsertByAnnotationTransaction(new Entities.AllPrimitive
        {
            String = "",
            DateTime = DateTime.Now
        });
    }

    [Fact]
    public void Should_InsertEntity_When_UsingAnnotationAOPTransaction()
    {
        var repository =
            _repositoryFactory.CreateInstance(typeof(IAllPrimitiveRepository),
                SqlMapper) as IAllPrimitiveRepository;
        var id = repository.InsertByAnnotationAOPTransaction(new Entities.AllPrimitive
        {
            String = "",
            DateTime = DateTime.Now
        });
    }
}
