using System.Threading.Tasks;
using FluentAssertions;
using SmartSql.Reflection.EntityProxy;
using SmartSql.Test.Entities;
using SmartSql.Test.Integration.Fixtures;
using Xunit;

namespace SmartSql.Test.Integration.Base;

public abstract class SqlMapperTestBase : IntegrationTestBase
{
    protected SqlMapperTestBase(IDbTestFixture fixture) : base(fixture) { }

    [Fact]
    public async Task Should_ReturnList_When_QueryAsync()
    {
        var list = await SqlMapper.QueryAsync<dynamic>(new RequestContext
        {
            RealSql = SelectTopAllPrimitive(5)
        });
        list.Should().NotBeNull();
    }

    [Fact]
    public void Should_ReturnSingle_When_QuerySingleDynamic()
    {
        var result = SqlMapper.QuerySingleDynamic(new RequestContext
        {
            RealSql = SelectTopAllPrimitive(1)
        });
        ((object)result).Should().NotBeNull();
    }

    [Fact]
    public void Should_ReturnList_When_QueryDynamic()
    {
        var list = SqlMapper.QueryDynamic(new RequestContext
        {
            RealSql = SelectTopAllPrimitive(5)
        });
        list.Should().NotBeNull();
    }

    [Fact]
    public void Should_ReturnList_When_QueryDictionary()
    {
        var list = SqlMapper.QueryDictionary(new RequestContext
        {
            RealSql = SelectTopAllPrimitive(5)
        });
        list.Should().NotBeNull();
    }

    [Fact]
    public void Should_ReturnSingle_When_QuerySingleDictionary()
    {
        var list = SqlMapper.QuerySingleDictionary(new RequestContext
        {
            RealSql = SelectTopAllPrimitive(1)
        });
        list.Should().NotBeNull();
    }

    [Fact]
    public void Should_ReturnList_When_Query()
    {
        var list = SqlMapper.Query<AllPrimitive>(new RequestContext
        {
            RealSql = SelectTopAllPrimitive(5)
        });
        list.Should().NotBeNull();
    }

    [Fact]
    public void Should_TrackChanges_When_QueryEnableTrack()
    {
        var entity = SqlMapper.QuerySingle<AllPrimitive>(new RequestContext
        {
            EnablePropertyChangedTrack = true,
            RealSql = SelectTopAllPrimitive(1)
        });
        var entityProxy = entity as IEntityPropertyChangedTrackProxy;
        entityProxy.Should().NotBeNull();

        var state = entityProxy.GetPropertyVersion(nameof(AllPrimitive.String));
        state.Should().Be(0);
        entity.String = "Updated";
        state = entityProxy.GetPropertyVersion(nameof(AllPrimitive.String));
        state.Should().Be(1);

        SqlMapper.Update(entity);
    }

    [Fact]
    public void Should_ReturnDefault_When_DbNullToDefaultEntity()
    {
        var entity = SqlMapper.QuerySingle<IgnoreDbNullEntity>(new RequestContext
        {
            RealSql = SelectTopAllPrimitive(1)
        });
        entity.DbNullId.Should().Be(0);
    }
}
