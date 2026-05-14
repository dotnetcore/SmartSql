using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using SmartSql.Test.Entities;
using Xunit;

namespace SmartSql.Test.Integration.Deserializer;

public class DynamicDeserializerTests : IntegrationTestBase
{
    public DynamicDeserializerTests(SmartSqlFixture fixture) : base(fixture) { }

    [Fact]
    public void Should_ReturnDynamicEntity_When_QuerySingle()
    {
        var result = SqlMapper.QuerySingle<dynamic>(new RequestContext
        {
            Scope = nameof(AllPrimitive), SqlId = "Query", Request = new { Taken = 1 }
        });
        ((long)result.Id).Should().BeGreaterThan(0);
    }

    [Fact]
    public void Should_ReturnDynamicList_When_Query()
    {
        var result = SqlMapper.Query<dynamic>(new RequestContext
        {
            Scope = nameof(AllPrimitive), SqlId = "Query", Request = new { Taken = 10 }
        });
        ((long)result.FirstOrDefault().Id).Should().BeGreaterThan(0);
    }

    [Fact]
    public void Should_ReturnDictionaryList_When_QueryWithDictionaryType()
    {
        var result = SqlMapper.Query<IDictionary<string, object>>(new RequestContext
        {
            Scope = nameof(AllPrimitive), SqlId = "Query", Request = new { Taken = 10 }
        });
        Convert.ToInt64(result.FirstOrDefault()["Id"]).Should().BeGreaterThan(0);
    }

    [Fact]
    public void Should_ConvertToHashtable_When_DynamicResultConverted()
    {
        var result = SqlMapper.Query<dynamic>(new RequestContext
        {
            Scope = nameof(AllPrimitive), SqlId = "Query", Request = new { Taken = 10 }
        });
        var hashtableList = result.Select(item =>
        {
            var dic = (IDictionary<string, object>)item;
            var hashTable = new Hashtable(dic.Count);
            foreach (var kv in dic) hashTable.Add(kv.Key, kv.Value);
            return hashTable;
        });
        hashtableList.Should().NotBeEmpty();
    }

    [Fact]
    public async Task Should_ReturnDynamicEntity_When_QuerySingleAsync()
    {
        var result = await SqlMapper.QuerySingleAsync<dynamic>(new RequestContext
        {
            Scope = nameof(AllPrimitive), SqlId = "Query", Request = new { Taken = 1 }
        });
        ((long)result.Id).Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task Should_ReturnDynamicList_When_QueryAsync()
    {
        var result = await SqlMapper.QueryAsync<dynamic>(new RequestContext
        {
            Scope = nameof(AllPrimitive), SqlId = "Query", Request = new { Taken = 10 }
        });
        ((long)result.FirstOrDefault().Id).Should().BeGreaterThan(0);
    }
}
