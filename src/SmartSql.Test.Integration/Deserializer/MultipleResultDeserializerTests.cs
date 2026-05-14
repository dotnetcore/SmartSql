using FluentAssertions;
using System.Collections.Generic;
using System.Threading.Tasks;
using SmartSql.Test.DTO;
using SmartSql.Test.Entities;
using Xunit;

namespace SmartSql.Test.Integration.Deserializer;

public class MultipleResultDeserializerTests : IntegrationTestBase
{
    public MultipleResultDeserializerTests(SmartSqlFixture fixture) : base(fixture) { }

    [Fact]
    public void Should_ReturnPagedResponse_When_GetByPage()
    {
        var result = SqlMapper.QuerySingle<GetByPageResponse<AllPrimitive>>(new RequestContext
        {
            Scope = nameof(AllPrimitive),
            SqlId = "GetByPage",
            Request = new { PageSize = 10, Offset = 0 }
        });
        result.Should().NotBeNull();
    }

    [Fact]
    public async Task Should_ReturnPagedResponse_When_GetByPageAsync()
    {
        var result = await SqlMapper.QuerySingleAsync<GetByPageResponse<AllPrimitive>>(new RequestContext
        {
            Scope = nameof(AllPrimitive),
            SqlId = "GetByPage",
            Request = new { PageSize = 10, Offset = 0 }
        });
        result.Should().NotBeNull();
    }

    [Fact]
    public void Should_ReturnPagedList_When_GetMultiRoot()
    {
        var result = SqlMapper.QuerySingle<PagedList>(new RequestContext
        {
            Scope = nameof(AllPrimitive),
            SqlId = "GetMultiRoot",
            Request = new { PageSize = 10, Offset = 0 }
        });
        result.Should().NotBeNull();
        result.List.Should().NotBeNull();
    }

    public class PagedList
    {
        public long Total { get; set; }
        public IList<AllPrimitive> List { get; set; }
    }
}
