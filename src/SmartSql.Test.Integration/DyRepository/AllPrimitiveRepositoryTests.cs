using System.Linq;
using FluentAssertions;
using SmartSql.Test.Entities;
using SmartSql.Test.Repositories;
using Xunit;

namespace SmartSql.Test.Integration.DyRepository;

public class AllPrimitiveRepositoryTests : IntegrationTestBase
{
    private readonly IAllPrimitiveRepository _repository;

    public AllPrimitiveRepositoryTests(SmartSqlFixture fixture) : base(fixture)
    {
        _repository = fixture.AllPrimitiveRepository;
    }

    [Fact]
    public void Should_ReturnResult_When_QueryingByPageValueTuple()
    {
        var result = _repository.GetByPage_ValueTuple();
        result.Should().NotBeNull();
    }

    [Fact]
    public void Should_ReturnDictionary_When_QueryingDictionary()
    {
        var result = _repository.QueryDictionary(10);
        result.Should().NotBeNull();
    }

    [Theory]
    [InlineData(1, NumericalEnum11.One)]
    [InlineData(2, NumericalEnum11.Two)]
    public void Should_ReturnMatchingEnums_When_FilteringByValue(int value, NumericalEnum11 numericalEnum)
    {
        var list = SqlMapper.Query<NumericalEnum11?>(new RequestContext
        {
            RealSql = "SELECT NumericalEnum FROM T_AllPrimitive WHERE NumericalEnum = ?value",
            Request = new { value }
        });
        list.Should().NotBeNull();
        list.All(t => t == numericalEnum).Should().BeTrue();

        var result = _repository.GetNumericalEnums(value);
        result.Should().NotBeNull();
        result.All(t => t == numericalEnum).Should().BeTrue();
    }
}
