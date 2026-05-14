using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using FluentAssertions;
using Microsoft.Data.Sqlite;
using Moq;
using SmartSql.Cache;
using SmartSql.Configuration;
using SmartSql.Data;
using SmartSql.DataSource;
using Xunit;

namespace SmartSql.Test.Unit.Cache;

public class CacheKeyRequestContextTests
{
    private static RequestContext<object> CreateRequestContext(
        string realSql = "SELECT * FROM Users",
        Dictionary<string, DbParameter> dbParameters = null,
        bool isStatementSql = true,
        string scope = "TestScope",
        string sqlId = "GetAll",
        Type resultType = null)
    {
        var config = new SmartSqlConfig
        {
            Database = new Database
            {
                DbProvider = new DbProvider { ParameterPrefix = "@" }
            }
        };

        var request = new RequestContext<object>
        {
            RealSql = realSql,
            Scope = scope,
            SqlId = sqlId
        };

        var result = new SingleResultContext<string>();

        var executionContext = new ExecutionContext
        {
            SmartSqlConfig = config,
            Request = request,
            Result = result
        };
        request.ExecutionContext = executionContext;

        // Set IsStatementSql via reflection
        var isStatementSqlProp = typeof(AbstractRequestContext).GetProperty("IsStatementSql",
            System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        isStatementSqlProp.SetValue(request, isStatementSql);

        // Set up parameters with DbParameters if provided
        var sqlParams = new SqlParameterCollection();
        if (dbParameters != null)
        {
            foreach (var kv in dbParameters)
            {
                sqlParams.TryAdd(kv.Key, kv.Value.Value);
                sqlParams.DbParameters[kv.Key] = kv.Value;
            }
        }

        var paramsProp = typeof(AbstractRequestContext).GetProperty("Parameters",
            System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        paramsProp.SetValue(request, sqlParams);

        return request;
    }

    [Fact]
    public void Should_GenerateHashedKey_When_NoParameters()
    {
        var request = CreateRequestContext(realSql: "SELECT 1");

        var cacheKey = new CacheKey(request);

        cacheKey.Key.Should().NotBeNullOrEmpty();
        cacheKey.Key.Should().NotBe("SELECT 1");
    }

    [Fact]
    public void Should_IncludeFullSqlId_When_IsStatementSql()
    {
        var request = CreateRequestContext(
            realSql: "SELECT 1",
            isStatementSql: true,
            scope: "MyScope",
            sqlId: "GetUser");

        var cacheKey = new CacheKey(request);

        cacheKey.Key.Should().StartWith("MyScope.GetUser:");
    }

    [Fact]
    public void Should_NotIncludeFullSqlId_When_IsNotStatementSql()
    {
        var request = CreateRequestContext(
            realSql: "SELECT 1",
            isStatementSql: false,
            scope: "MyScope",
            sqlId: "GetUser");

        var cacheKey = new CacheKey(request);

        cacheKey.Key.Should().NotStartWith("MyScope.GetUser:");
    }

    [Fact]
    public void Should_IncludeParametersInKey_When_HasDbParameters()
    {
        var param1 = new SqliteParameter("@Id", 42);
        var params1 = new Dictionary<string, DbParameter> { { "Id", param1 } };
        var request1 = CreateRequestContext(realSql: "SELECT * FROM Users WHERE Id=@Id", dbParameters: params1);

        var param2 = new SqliteParameter("@Id", 99);
        var params2 = new Dictionary<string, DbParameter> { { "Id", param2 } };
        var request2 = CreateRequestContext(realSql: "SELECT * FROM Users WHERE Id=@Id", dbParameters: params2);

        var key1 = new CacheKey(request1);
        var key2 = new CacheKey(request2);

        key1.Key.Should().NotBe(key2.Key);
    }

    [Fact]
    public void Should_GenerateSameKey_When_SameSqlAndParameters()
    {
        var param1 = new SqliteParameter("@Id", 42);
        var params1 = new Dictionary<string, DbParameter> { { "Id", param1 } };
        var request1 = CreateRequestContext(realSql: "SELECT * FROM Users WHERE Id=@Id", dbParameters: params1);

        var param1b = new SqliteParameter("@Id", 42);
        var params1b = new Dictionary<string, DbParameter> { { "Id", param1b } };
        var request1b = CreateRequestContext(realSql: "SELECT * FROM Users WHERE Id=@Id", dbParameters: params1b);

        var key1 = new CacheKey(request1);
        var key1b = new CacheKey(request1b);

        key1.Key.Should().Be(key1b.Key);
    }

    [Fact]
    public void Should_GenerateDifferentKey_When_DifferentSql()
    {
        var request1 = CreateRequestContext(realSql: "SELECT 1");
        var request2 = CreateRequestContext(realSql: "SELECT 2");

        var key1 = new CacheKey(request1);
        var key2 = new CacheKey(request2);

        key1.Key.Should().NotBe(key2.Key);
    }

    [Fact]
    public void Should_SetResultType_When_CreatedFromRequestContext()
    {
        var request = CreateRequestContext();

        var cacheKey = new CacheKey(request);

        cacheKey.ResultType.Should().Be(typeof(string));
    }

    [Fact]
    public void Should_HandleNullParameterValue()
    {
        var param = new SqliteParameter("@Name", DBNull.Value);
        var parameters = new Dictionary<string, DbParameter> { { "Name", param } };
        var request = CreateRequestContext(realSql: "SELECT * FROM Users WHERE Name=@Name", dbParameters: parameters);

        var act = () => new CacheKey(request);

        act.Should().NotThrow();
    }

    [Fact]
    public void Should_GenerateKeyWithMultipleParameters()
    {
        var param1 = new SqliteParameter("@Id", 42);
        var param2 = new SqliteParameter("@Name", "Test");
        var parameters = new Dictionary<string, DbParameter> { { "Id", param1 }, { "Name", param2 } };

        var request = CreateRequestContext(
            realSql: "SELECT * FROM Users WHERE Id=@Id AND Name=@Name",
            dbParameters: parameters);

        var act = () => new CacheKey(request);

        act.Should().NotThrow();
    }

    [Fact]
    public void Should_EqualOtherCacheKey_When_SameKey()
    {
        var request1 = CreateRequestContext(realSql: "SELECT 1", isStatementSql: false);
        var request2 = CreateRequestContext(realSql: "SELECT 1", isStatementSql: false);

        var key1 = new CacheKey(request1);
        var key2 = new CacheKey(request2);

        key1.Equals(key2).Should().BeTrue();
    }

    [Fact]
    public void Should_ReturnKey_When_ToString()
    {
        var request = CreateRequestContext();

        var cacheKey = new CacheKey(request);

        cacheKey.ToString().Should().Be(cacheKey.Key);
    }

    [Fact]
    public void Should_ReturnKeyHashCode_When_GetHashCode()
    {
        var request = CreateRequestContext();

        var cacheKey = new CacheKey(request);

        cacheKey.GetHashCode().Should().Be(cacheKey.Key.GetHashCode());
    }
}
