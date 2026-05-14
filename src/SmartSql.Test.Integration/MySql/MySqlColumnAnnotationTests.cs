using FluentAssertions;
using SmartSql.DyRepository;
using SmartSql.Test.Entities;
using SmartSql.Test.Integration.Fixtures;
using SmartSql.Test.Repositories;
using Xunit;

namespace SmartSql.Test.Integration.MySql;

[Collection(MySqlFixture.CollectionName)]
public class MySqlColumnAnnotationTests : IntegrationTestBase
{
    private readonly IColumnAnnotationRepository _repository;

    public MySqlColumnAnnotationTests(MySqlFixture fixture) : base(fixture)
    {
        _repository = fixture.RepositoryFactory.CreateInstance(typeof(IColumnAnnotationRepository), SqlMapper)
            as IColumnAnnotationRepository;
    }

    [Fact]
    public void Should_ReturnEntity_When_GettingById()
    {
        var id = DoInsert();
        var entity = _repository.GetEntity(id);
        entity.Should().NotBeNull();
        entity.Id.Should().Be(id);
    }

    [Fact]
    public void Should_ReturnNonZeroId_When_Inserting()
    {
        var id = DoInsert();
        id.Should().BeGreaterThan(0);
    }

    private int DoInsert()
    {
        return _repository.Insert(new ColumnAnnotationEntity
        {
            Name = nameof(IColumnAnnotationRepository),
            Data = new ColumnAnnotationEntity.ExtendData
            {
                Info = nameof(IColumnAnnotationRepository)
            }
        });
    }

    [Fact]
    public void Should_ReturnNonZeroId_When_InsertingByParamAnnotations()
    {
        var id = _repository.Insert("InsertByParamAnnotations", new ColumnAnnotationEntity.ExtendData
        {
            Info = "InsertByParamAnnotations"
        });
        id.Should().BeGreaterThan(0);
    }
}
