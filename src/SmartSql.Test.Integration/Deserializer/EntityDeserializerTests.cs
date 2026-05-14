using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using SmartSql.Test.Entities;
using Xunit;

namespace SmartSql.Test.Integration.Deserializer;

public class EntityDeserializerTests : IntegrationTestBase
{
    public EntityDeserializerTests(SmartSqlFixture fixture) : base(fixture) { }

    private long Insert()
    {
        return SqlMapper.Insert<AllPrimitive, long>(new AllPrimitive
        {
            String = "Insert", DateTime = DateTime.Now
        });
    }

    [Fact]
    public void Should_ReturnEntity_When_QuerySingleById()
    {
        long id = Insert();
        var entity = SqlMapper.QuerySingle<AllPrimitive>(new RequestContext
        {
            Scope = nameof(AllPrimitive), SqlId = "GetById", Request = new { Id = id }
        });
        entity.Id.Should().Be(id);
    }

    [Fact]
    public void Should_ReturnList_When_Query()
    {
        var list = SqlMapper.Query<AllPrimitive>(new RequestContext
        {
            Scope = nameof(AllPrimitive), SqlId = "Query", Request = new { Taken = 10000 }
        });
        list.Should().NotBeNull();
    }

    [Fact]
    public async Task Should_ReturnEntity_When_QuerySingleAsync()
    {
        long id = Insert();
        var entity = await SqlMapper.QuerySingleAsync<AllPrimitive>(new RequestContext
        {
            Scope = nameof(AllPrimitive), SqlId = "GetById", Request = new { Id = id }
        });
        entity.Id.Should().Be(id);
    }

    [Fact]
    public async Task Should_ReturnList_When_QueryAsync()
    {
        var list = await SqlMapper.QueryAsync<AllPrimitive>(new RequestContext
        {
            Scope = nameof(AllPrimitive), SqlId = "Query", Request = new { Taken = 10000 }
        });
        list.Should().NotBeNull();
    }

    [Fact]
    public void Should_MapNestedProperties_When_QueryNestedEntity()
    {
        var list = SqlMapper.Query<NestedEntity>(new RequestContext
        {
            Scope = nameof(AllPrimitive), SqlId = "QueryNestedPropertyResult",
            Request = new { Taken = 10000 }
        });
        list.Should().NotBeNull();
        list.First().NestedProp1.Should().NotBeNull();
        list.First().NestedProp1.NestedProp2.Should().NotBeNull();
        list.First().NestedProp1.NestedProp2.NestedProp3.Should().NotBeNull();
    }
}
