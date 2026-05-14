using System.Collections.Generic;
using FluentAssertions;
using SmartSql.Test.Integration.Fixtures;
using Xunit;

namespace SmartSql.Test.Integration.Base;

public abstract class NestTestBase : IntegrationTestBase
{
    protected NestTestBase(IDbTestFixture fixture) : base(fixture) { }

    [Fact]
    public void Should_QueryNestObject_When_OneLevelNesting()
    {
        var requestCtx = new RequestContext
        {
            Scope = "NestTest", SqlId = "QueryNestObject1",
            Request = new { User = new { Id = 1 } }
        };
        var result = SqlMapper.ExecuteScalar<int>(requestCtx);
        result.Should().Be(1);
    }

    [Fact]
    public void Should_QueryNestObject_When_TwoLevelNesting()
    {
        var requestCtx = new RequestContext
        {
            Scope = "NestTest", SqlId = "QueryNestObject2",
            Request = new { User = new { Info = new { Id = 1 } } }
        };
        var result = SqlMapper.ExecuteScalar<int>(requestCtx);
        result.Should().Be(1);
    }

    [Fact]
    public void Should_QueryNestArray_When_ItemsIsArray()
    {
        var requestCtx = new RequestContext
        {
            Scope = "NestTest", SqlId = "QueryNestArray",
            Request = new { Order = new { Items = new[] { 1 } } }
        };
        var result = SqlMapper.ExecuteScalar<int>(requestCtx);
        result.Should().Be(1);
    }

    [Fact]
    public void Should_QueryNestDictionary_When_ItemsIsDictionary()
    {
        var requestCtx = new RequestContext
        {
            Scope = "NestTest", SqlId = "QueryNestDic",
            Request = new { Order = new { Items = new Dictionary<string, int> { { "Id", 1 } } } }
        };
        var result = SqlMapper.ExecuteScalar<int>(requestCtx);
        result.Should().Be(1);
    }

    [Fact]
    public void Should_FilterNestObject_When_OneLevelNesting()
    {
        var requestCtx = new RequestContext
        {
            Scope = "NestTest", SqlId = "FilterNestObject1",
            Request = new { User = new { Id = 1 } }
        };
        var result = SqlMapper.ExecuteScalar<int>(requestCtx);
        result.Should().Be(1);
    }

    [Fact]
    public void Should_FilterNestDictionaryMultiple_When_FieldsIsDictionary()
    {
        var requestCtx = new RequestContext
        {
            Scope = "NestTest", SqlId = "FilterNestDicMul",
            Request = new { Fields = new Dictionary<string, string>
            {
                { "Id", "Id" }, { "Name", "Name" }, { "CreateTime", "CreateTime" }
            }}
        };
        var result = SqlMapper.ExecuteScalar<string>(requestCtx);
        result.Trim().Should().Be("Id , Name , CreateTime");
    }
}
