using System.Collections.Generic;
using FluentAssertions;
using SmartSql.Configuration;
using SmartSql.Configuration.Tags;
using SmartSql.Exceptions;
using Xunit;
using CacheConfig = SmartSql.Configuration.Cache;

namespace SmartSql.Test.Unit.Configuration;

public class SqlMapTests
{
    [Fact]
    public void Should_GetStatement_When_StatementExists()
    {
        var sqlMap = CreateSqlMap("TestScope", new Dictionary<string, Statement>
        {
            ["TestScope.GetUsers"] = new Statement
            {
                Id = "GetUsers",
                SqlMap = CreateSqlMapReference("TestScope"),
                SqlTags = new List<ITag>()
            }
        });

        var statement = sqlMap.GetStatement("TestScope.GetUsers");

        statement.Should().NotBeNull();
        statement.Id.Should().Be("GetUsers");
    }

    [Fact]
    public void Should_Throw_When_GetStatementAndStatementNotFound()
    {
        var sqlMap = CreateSqlMap("TestScope", new Dictionary<string, Statement>());

        var act = () => sqlMap.GetStatement("TestScope.Unknown");

        act.Should().Throw<SmartSqlException>()
            .WithMessage("*TestScope.Unknown*");
    }

    [Fact]
    public void Should_GetCache_When_CacheExists()
    {
        var cache = new CacheConfig { Id = "TestScope.TestCache", Type = "Fifo" };
        var sqlMap = CreateSqlMap("TestScope", caches: new Dictionary<string, CacheConfig>
        {
            ["TestScope.TestCache"] = cache
        });

        var result = sqlMap.GetCache("TestScope.TestCache");

        result.Should().BeSameAs(cache);
    }

    [Fact]
    public void Should_Throw_When_GetCacheAndCacheNotFound()
    {
        var sqlMap = CreateSqlMap("TestScope");

        var act = () => sqlMap.GetCache("TestScope.UnknownCache");

        act.Should().Throw<SmartSqlException>()
            .WithMessage("*TestScope.UnknownCache*");
    }

    [Fact]
    public void Should_GetParameterMap_When_ParameterMapExists()
    {
        var parameterMap = new ParameterMap
        {
            Id = "TestScope.UserParam",
            Parameters = new Dictionary<string, Parameter>()
        };
        var sqlMap = CreateSqlMap("TestScope", parameterMaps: new Dictionary<string, ParameterMap>
        {
            ["TestScope.UserParam"] = parameterMap
        });

        var result = sqlMap.GetParameterMap("TestScope.UserParam");

        result.Should().BeSameAs(parameterMap);
    }

    [Fact]
    public void Should_Throw_When_GetParameterMapAndNotFound()
    {
        var sqlMap = CreateSqlMap("TestScope");

        var act = () => sqlMap.GetParameterMap("TestScope.Unknown");

        act.Should().Throw<SmartSqlException>()
            .WithMessage("*TestScope.Unknown*");
    }

    [Fact]
    public void Should_GetResultMap_When_ResultMapExists()
    {
        var resultMap = new ResultMap
        {
            Id = "TestScope.UserResult",
            Properties = new Dictionary<string, Property>()
        };
        var sqlMap = CreateSqlMap("TestScope", resultMaps: new Dictionary<string, ResultMap>
        {
            ["TestScope.UserResult"] = resultMap
        });

        var result = sqlMap.GetResultMap("TestScope.UserResult");

        result.Should().BeSameAs(resultMap);
    }

    [Fact]
    public void Should_Throw_When_GetResultMapAndNotFound()
    {
        var sqlMap = CreateSqlMap("TestScope");

        var act = () => sqlMap.GetResultMap("TestScope.Unknown");

        act.Should().Throw<SmartSqlException>()
            .WithMessage("*TestScope.Unknown*");
    }

    [Fact]
    public void Should_GetMultipleResultMap_When_Exists()
    {
        var multipleResultMap = new MultipleResultMap
        {
            Id = "TestScope.UserMultiple",
            Results = new List<Result>()
        };
        var sqlMap = CreateSqlMap("TestScope", multipleResultMaps: new Dictionary<string, MultipleResultMap>
        {
            ["TestScope.UserMultiple"] = multipleResultMap
        });

        var result = sqlMap.GetMultipleResultMap("TestScope.UserMultiple");

        result.Should().BeSameAs(multipleResultMap);
    }

    [Fact]
    public void Should_Throw_When_GetMultipleResultMapAndNotFound()
    {
        var sqlMap = CreateSqlMap("TestScope");

        var act = () => sqlMap.GetMultipleResultMap("TestScope.Unknown");

        act.Should().Throw<SmartSqlException>()
            .WithMessage("*TestScope.Unknown*");
    }

    private static SqlMap CreateSqlMapReference(string scope)
    {
        return new SqlMap { Scope = scope };
    }

    private static SqlMap CreateSqlMap(
        string scope,
        IDictionary<string, Statement> statements = null,
        IDictionary<string, CacheConfig> caches = null,
        IDictionary<string, ParameterMap> parameterMaps = null,
        IDictionary<string, ResultMap> resultMaps = null,
        IDictionary<string, MultipleResultMap> multipleResultMaps = null)
    {
        return new SqlMap
        {
            Scope = scope,
            Statements = statements ?? new Dictionary<string, Statement>(),
            Caches = caches ?? new Dictionary<string, CacheConfig>(),
            ParameterMaps = parameterMaps ?? new Dictionary<string, ParameterMap>(),
            ResultMaps = resultMaps ?? new Dictionary<string, ResultMap>(),
            MultipleResultMaps = multipleResultMaps ?? new Dictionary<string, MultipleResultMap>()
        };
    }
}
