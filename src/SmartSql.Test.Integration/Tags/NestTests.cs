using FluentAssertions;
using System;
using System.Collections.Generic;
using Xunit;

namespace SmartSql.Test.Integration.Tags;

public class NestTests : IntegrationTestBase
{
    public NestTests(SmartSqlFixture fixture) : base(fixture) { }

    [Fact]
    public void Should_QueryNestObject_When_OneLevelNesting()
    {
        RequestContext requestCtx = new RequestContext
        {
            Scope = "NestTest",
            SqlId = "QueryNestObject1",
            Request = new { User = new { Id = 1 } }
        };
        var result = SqlMapper.ExecuteScalar<int>(requestCtx);

        result.Should().Be(1);
        requestCtx.RealSql.Should().Be("Select ?User_Id");
    }

    [Fact]
    public void Should_QueryNestObject_When_TwoLevelNesting()
    {
        RequestContext requestCtx = new RequestContext
        {
            Scope = "NestTest",
            SqlId = "QueryNestObject2",
            Request = new
            {
                User = new
                {
                    Info = new
                    {
                        Id = 1
                    }
                }
            }
        };
        var result = SqlMapper.ExecuteScalar<int>(requestCtx);

        result.Should().Be(1);
        requestCtx.RealSql.Should().Be("Select ?User_Info_Id");
    }

    [Fact]
    public void Should_QueryNestArray_When_ItemsIsArray()
    {
        RequestContext requestCtx = new RequestContext
        {
            Scope = "NestTest",
            SqlId = "QueryNestArray",
            Request = new
            {
                Order = new
                {
                    Items = new[] { 1 }
                }
            }
        };
        var result = SqlMapper.ExecuteScalar<int>(requestCtx);

        result.Should().Be(1);
        requestCtx.RealSql.Should().Be("Select ?Order_Items_Idx_0");
    }

    [Fact]
    public void Should_QueryNestList_When_ItemsIsList()
    {
        RequestContext requestCtx = new RequestContext
        {
            Scope = "NestTest",
            SqlId = "QueryNestArray",
            Request = new
            {
                Order = new
                {
                    Items = new List<int> { 1 }
                }
            }
        };
        var result = SqlMapper.ExecuteScalar<int>(requestCtx);

        result.Should().Be(1);
        requestCtx.RealSql.Should().Be("Select ?Order_Items_Idx_0");
    }

    [Fact]
    public void Should_QueryNestDictionary_When_ItemsIsDictionary()
    {
        RequestContext requestCtx = new RequestContext
        {
            Scope = "NestTest",
            SqlId = "QueryNestDic",
            Request = new
            {
                Order = new
                {
                    Items = new Dictionary<string, int> { { "Id", 1 } }
                }
            }
        };
        var result = SqlMapper.ExecuteScalar<int>(requestCtx);

        result.Should().Be(1);
        requestCtx.RealSql.Should().Be("Select ?Order_Items_Idx_Id");
    }

    [Fact]
    public void Should_QueryNestArrayObject_When_ItemsIsAnonymousObjectArray()
    {
        RequestContext requestCtx = new RequestContext
        {
            Scope = "NestTest",
            SqlId = "QueryNestArrayObject",
            Request = new
            {
                Order = new
                {
                    Items = new[] { new { Name = "SmartSql" } }
                }
            }
        };
        var result = SqlMapper.ExecuteScalar<String>(requestCtx);

        result.Should().Be("SmartSql");
        requestCtx.RealSql.Should().Be("Select ?Order_Items_Idx_0_Name");
    }

    [Fact]
    public void Should_QueryNestArrayObject_When_ItemsIsStronglyTypedArray()
    {
        RequestContext requestCtx = new RequestContext
        {
            Scope = "NestTest",
            SqlId = "QueryNestArrayObject",
            Request = new
            {
                Order = new
                {
                    Items = new[] { new OrderItem { Name = "SmartSql" } }
                }
            }
        };
        var result = SqlMapper.ExecuteScalar<String>(requestCtx);

        result.Should().Be("SmartSql");
        requestCtx.RealSql.Should().Be("Select ?Order_Items_Idx_0_Name");
    }

    [Fact]
    public void Should_FilterNestObject_When_OneLevelNesting()
    {
        RequestContext requestCtx = new RequestContext
        {
            Scope = "NestTest",
            SqlId = "FilterNestObject1",
            Request = new { User = new { Id = 1 } }
        };
        var result = SqlMapper.ExecuteScalar<int>(requestCtx);

        result.Should().Be(1);
        requestCtx.RealSql.Should().Be("Select ?User_Id");
    }

    [Fact]
    public void Should_FilterNestObject_When_TwoLevelNesting()
    {
        RequestContext requestCtx = new RequestContext
        {
            Scope = "NestTest",
            SqlId = "FilterNestObject2",
            Request = new
            {
                User = new
                {
                    Info = new { Id = 1 }
                }
            }
        };
        var result = SqlMapper.ExecuteScalar<int>(requestCtx);

        result.Should().Be(1);
        requestCtx.RealSql.Should().Be("Select ?User_Info_Id");
    }

    [Fact]
    public void Should_FilterNestArray_When_ItemsIsArray()
    {
        RequestContext requestCtx = new RequestContext
        {
            Scope = nameof(NestTests),
            SqlId = "FilterNestArray",
            Request = new
            {
                Order = new { Items = new[] { 1 } }
            }
        };
        var result = SqlMapper.ExecuteScalar<int>(requestCtx);

        result.Should().Be(1);
        requestCtx.RealSql.Should().Be("Select ?Order_Items_Idx_0");
    }

    [Fact]
    public void Should_FilterNestDictionary_When_ItemsIsDictionary()
    {
        RequestContext requestCtx = new RequestContext
        {
            Scope = nameof(NestTests),
            SqlId = "FilterNestDic",
            Request = new
            {
                Order = new
                {
                    Items = new Dictionary<string, int> { { "Id", 1 } }
                }
            }
        };
        var result = SqlMapper.ExecuteScalar<int>(requestCtx);

        result.Should().Be(1);
        requestCtx.RealSql.Should().Be("Select ?Order_Items_Idx_Id");
    }

    [Fact]
    public void Should_FilterNestArrayObject_When_ItemsIsStronglyTypedArray()
    {
        RequestContext requestCtx = new RequestContext
        {
            Scope = nameof(NestTests),
            SqlId = "FilterNestArrayObject",
            Request = new
            {
                Order = new
                {
                    Items = new[] { new OrderItem { Name = "SmartSql" } }
                }
            }
        };
        var result = SqlMapper.ExecuteScalar<String>(requestCtx);

        result.Should().Be("SmartSql");
        requestCtx.RealSql.Should().Be("Select ?Order_Items_Idx_0_Name");
    }

    [Fact]
    public void Should_FilterNestDictionaryMultiple_When_FieldsIsDictionary()
    {
        RequestContext requestCtx = new RequestContext
        {
            Scope = nameof(NestTests),
            SqlId = "FilterNestDicMul",
            Request = new
            {
                Fields = new Dictionary<String, String>
                {
                    { "Id", "Id" },
                    { "Name", "Name" },
                    { "CreateTime", "CreateTime" },
                }
            }
        };
        var result = SqlMapper.ExecuteScalar<String>(requestCtx);

        result.Trim().Should().Be("Id , Name , CreateTime");
        requestCtx.RealSql.Trim().Should().Be(@"Select'
                Id , Name , CreateTime
            '");
    }

    public class OrderItem
    {
        public String Name { get; set; }
    }
}
