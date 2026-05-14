using FluentAssertions;
using System.Threading.Tasks;
using SmartSql.Reflection.EntityProxy;
using SmartSql.Test.Entities;
using Xunit;


namespace SmartSql.Test.Integration.DbSession;

public class SqlMapperTests : IntegrationTestBase
{
    public SqlMapperTests(SmartSqlFixture fixture) : base(fixture) { }

    [Fact]
    public async Task Should_ReturnList_When_QueryAsync()
    {
        var list = await SqlMapper.QueryAsync<dynamic>(new RequestContext
        {
            RealSql = "SELECT T.* From T_AllPrimitive T limit 5"
        });
        list.Should().NotBeNull();
    }

    [Fact]
    public void Should_ReturnSingle_When_QuerySingleDynamic()
    {
        var list = SqlMapper.QuerySingleDynamic(new RequestContext
        {
            RealSql = "SELECT T.* From T_AllPrimitive T limit 1"
        });
        list.Should().NotBeNull();
    }

    [Fact]
    public void Should_ReturnList_When_QueryDynamic()
    {
        var list = SqlMapper.QueryDynamic(new RequestContext
        {
            RealSql = "SELECT T.* From T_AllPrimitive T limit 5"
        });
        list.Should().NotBeNull();
    }

    [Fact]
    public void Should_ReturnList_When_QueryDictionary()
    {
        var list = SqlMapper.QueryDictionary(new RequestContext
        {
            RealSql = "SELECT T.* From T_AllPrimitive T limit 5"
        });
        list.Should().NotBeNull();
    }

    [Fact]
    public void Should_ReturnSingle_When_QuerySingleDictionary()
    {
        var list = SqlMapper.QuerySingleDictionary(new RequestContext
        {
            RealSql = "SELECT T.* From T_AllPrimitive T limit 1"
        });
        list.Should().NotBeNull();
    }

    [Fact]
    public void Should_ReturnList_When_Query()
    {
        var list = SqlMapper.Query<AllPrimitive>(new RequestContext
        {
            RealSql = "SELECT T.* From T_AllPrimitive T limit 5"
        });
        list.Should().NotBeNull();
    }

    [Fact]
    public void Should_TrackChanges_When_QueryEnableTrack()
    {
        var entity = SqlMapper.QuerySingle<AllPrimitive>(new RequestContext
        {
            EnablePropertyChangedTrack = true,
            RealSql = "SELECT T.* From T_AllPrimitive T limit 1"
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
            RealSql = "SELECT T.* From T_AllPrimitive T limit 1"
        });
        entity.DbNullId.Should().Be(0);
    }
}
