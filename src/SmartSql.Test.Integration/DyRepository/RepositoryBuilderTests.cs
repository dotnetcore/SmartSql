using System;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using SmartSql.DyRepository;
using SmartSql.Test.Repositories;
using Xunit;

namespace SmartSql.Test.Integration.DyRepository;

public class RepositoryBuilderTests : IntegrationTestBase
{
    private readonly IRepositoryBuilder _repositoryBuilder;
    private readonly IRepositoryFactory _repositoryFactory;

    public RepositoryBuilderTests(SmartSqlFixture fixture) : base(fixture)
    {
        _repositoryBuilder = fixture.RepositoryBuilder;
        _repositoryFactory = fixture.RepositoryFactory;
    }

    [Fact]
    public void Should_BuildRepositoryType_When_CallingBuild()
    {
        var loggerFactory = Fixture.LoggerFactory;
        var builder = new SmartSqlBuilder()
            .UseXmlConfig()
            .UseLoggerFactory(loggerFactory)
            .UseAlias("RepoBuilderTests_" + Guid.NewGuid())
            .RegisterEntity(typeof(Entities.AllPrimitive))
            .UseCUDConfigBuilder()
            .Build();
        var repoBuilder = new EmitRepositoryBuilder(null, null,
            loggerFactory.CreateLogger<EmitRepositoryBuilder>());
        var repositoryImplType = repoBuilder.Build(typeof(IAllPrimitiveRepository), builder.SmartSqlConfig);
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
