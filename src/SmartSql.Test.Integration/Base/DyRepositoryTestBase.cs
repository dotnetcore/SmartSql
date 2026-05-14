using System;
using FluentAssertions;
using SmartSql.DyRepository;
using SmartSql.Test.Entities;
using SmartSql.Test.Integration.Fixtures;
using SmartSql.Test.Repositories;
using Xunit;

namespace SmartSql.Test.Integration.Base;

public abstract class DyRepositoryTestBase : IntegrationTestBase
{
    protected readonly IAllPrimitiveRepository _repository;
    protected readonly IRepositoryFactory _repositoryFactory;

    protected DyRepositoryTestBase(IDbTestFixture fixture) : base(fixture)
    {
        _repository = fixture.AllPrimitiveRepository;
        _repositoryFactory = fixture.RepositoryFactory;
    }

    [Fact]
    public void Should_ReturnDictionary_When_QueryingDictionary()
    {
        var result = _repository.QueryDictionary(10);
        result.Should().NotBeNull();
    }

    [Fact]
    public void Should_CreateAndUseInstance_When_CallingCreateInstance()
    {
        var repository = _repositoryFactory.CreateInstance(typeof(IAllPrimitiveRepository), SqlMapper) as IAllPrimitiveRepository;
        repository.IsDyRepository().Should().BeTrue();
        var list = repository.Query(10);
        var id = repository.Insert(new AllPrimitive { String = "", DateTime = DateTime.Now });
    }

    [Fact]
    public void Should_InsertEntity_When_UsingAnnotationTransaction()
    {
        var repository = _repositoryFactory.CreateInstance(typeof(IAllPrimitiveRepository), SqlMapper) as IAllPrimitiveRepository;
        var id = repository.InsertByAnnotationTransaction(new AllPrimitive { String = "", DateTime = DateTime.Now });
    }

    [Fact]
    public void Should_InsertEntity_When_UsingAnnotationAOPTransaction()
    {
        var repository = _repositoryFactory.CreateInstance(typeof(IAllPrimitiveRepository), SqlMapper) as IAllPrimitiveRepository;
        var id = repository.InsertByAnnotationAOPTransaction(new AllPrimitive { String = "", DateTime = DateTime.Now });
    }
}
