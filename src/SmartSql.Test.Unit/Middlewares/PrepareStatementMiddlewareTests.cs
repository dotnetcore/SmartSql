using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using SmartSql.Command;
using SmartSql.Configuration;
using SmartSql.DataSource;
using SmartSql.DbSession;
using SmartSql.Utils;
using SmartSql.TypeHandlers;
using Xunit;
using Microsoft.Data.Sqlite;
using PrepareStatementMiddleware = SmartSql.Middlewares.PrepareStatementMiddleware;
using CacheCfg = SmartSql.Configuration.Cache;
using StatementTypeCfg = SmartSql.Configuration.StatementType;

namespace SmartSql.Test.Unit.Middlewares;

public class PrepareStatementMiddlewareTests
{
    [Fact]
    public void Should_HaveOrderOneHundred()
    {
        var middleware = new PrepareStatementMiddleware();

        middleware.Order.Should().Be(100);
    }

    [Fact]
    public void Should_SkipSqlBuild_When_RealSqlIsProvidedDirectly()
    {
        var middleware = new PrepareStatementMiddleware();
        var dbProvider = CreateSqliteDbProvider();
        var config = CreateSmartSqlConfig(dbProvider);
        SetupMiddleware(middleware, config);

        var dbSession = CreateDbSession();
        var request = new RequestContext
        {
            RealSql = "SELECT * FROM Users"
        };
        // Mark as non-statement SQL (RealSql already set means InitializerMiddleware would set this)
        SetIsStatementSql(request, false);
        request.CommandType = CommandType.Text;

        var result = new SingleResultContext<string>();
        var context = new ExecutionContext
        {
            SmartSqlConfig = config,
            DbSession = dbSession,
            Request = request,
            Result = result
        };
        request.ExecutionContext = context;
        request.SetupParameters();

        var originalSql = request.RealSql;
        middleware.Invoke<string>(context);

        request.RealSql.Should().Be(originalSql);
    }

    [Fact]
    public async Task Should_WorkWithInvokeAsync_When_RealSqlProvided()
    {
        var middleware = new PrepareStatementMiddleware();
        var dbProvider = CreateSqliteDbProvider();
        var config = CreateSmartSqlConfig(dbProvider);
        SetupMiddleware(middleware, config);

        var dbSession = CreateDbSession();
        var request = new RequestContext
        {
            RealSql = "SELECT 1"
        };
        SetIsStatementSql(request, false);
        request.CommandType = CommandType.Text;

        var result = new SingleResultContext<string>();
        var context = new ExecutionContext
        {
            SmartSqlConfig = config,
            DbSession = dbSession,
            Request = request,
            Result = result
        };
        request.ExecutionContext = context;
        request.SetupParameters();

        var originalSql = request.RealSql;
        await middleware.InvokeAsync<string>(context);

        request.RealSql.Should().Be(originalSql);
    }

    [Fact]
    public void Should_BuildSqlFromStatement_When_StatementSqlWithTags()
    {
        var middleware = new PrepareStatementMiddleware();
        var dbProvider = CreateSqliteDbProvider();
        var config = CreateSmartSqlConfig(dbProvider);
        SetupMiddleware(middleware, config);

        var sqlMap = CreateSqlMap();
        config.SqlMaps["TestScope"] = sqlMap;
        var statement = new Statement
        {
            SqlMap = sqlMap,
            Id = "GetById",
            StatementType = StatementTypeCfg.Select,
            SqlTags = new List<SmartSql.Configuration.Tags.ITag>
            {
                new SmartSql.Configuration.Tags.SqlText("SELECT * FROM Users WHERE Id = @Id", "@")
            }
        };
        sqlMap.Statements["TestScope.GetById"] = statement;

        var dbSession = CreateDbSession();
        var request = new RequestContext
        {
            Scope = "TestScope",
            SqlId = "GetById"
        };
        SetStatement(request, statement);
        SetIsStatementSql(request, true);
        request.CommandType = CommandType.Text;

        var result = new SingleResultContext<string>();
        var context = new ExecutionContext
        {
            SmartSqlConfig = config,
            DbSession = dbSession,
            Request = request,
            Result = result
        };
        request.ExecutionContext = context;
        request.SetupParameters();

        middleware.Invoke<string>(context);

        request.RealSql.Should().Be("SELECT * FROM Users WHERE Id = @Id");
    }

    [Fact]
    public void Should_BuildEmptySql_When_StatementHasNoTags()
    {
        var middleware = new PrepareStatementMiddleware();
        var dbProvider = CreateSqliteDbProvider();
        var config = CreateSmartSqlConfig(dbProvider);
        SetupMiddleware(middleware, config);

        var sqlMap = CreateSqlMap();
        config.SqlMaps["TestScope"] = sqlMap;
        var statement = new Statement
        {
            SqlMap = sqlMap,
            Id = "GetAll",
            StatementType = StatementTypeCfg.Select,
            SqlTags = new List<SmartSql.Configuration.Tags.ITag>()
        };
        sqlMap.Statements["TestScope.GetAll"] = statement;

        var dbSession = CreateDbSession();
        var request = new RequestContext
        {
            Scope = "TestScope",
            SqlId = "GetAll"
        };
        SetStatement(request, statement);
        SetIsStatementSql(request, true);
        request.CommandType = CommandType.Text;

        var result = new SingleResultContext<string>();
        var context = new ExecutionContext
        {
            SmartSqlConfig = config,
            DbSession = dbSession,
            Request = request,
            Result = result
        };
        request.ExecutionContext = context;
        request.SetupParameters();

        middleware.Invoke<string>(context);

        request.RealSql.Should().BeEmpty();
    }

    private static void SetStatement(AbstractRequestContext request, Statement statement)
    {
        var prop = typeof(AbstractRequestContext).GetProperty("Statement",
            System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        prop.SetValue(request, statement);
    }

    private static void SetIsStatementSql(AbstractRequestContext request, bool value)
    {
        var prop = typeof(AbstractRequestContext).GetProperty("IsStatementSql",
            System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        prop.SetValue(request, value);
    }

    private static SmartSqlConfig CreateSmartSqlConfig(DbProvider dbProvider)
    {
        var config = new SmartSqlConfig
        {
            Database = new Database { DbProvider = dbProvider }
        };
        config.SqlParamAnalyzer = new SqlParamAnalyzer(
            config.Settings.IgnoreParameterCase,
            dbProvider.ParameterPrefix);
        return config;
    }

    private static DbProvider CreateSqliteDbProvider()
    {
        return new DbProvider
        {
            Name = "SQLite",
            ParameterPrefix = "@",
            Factory = SqliteFactory.Instance
        };
    }

    private static SqlMap CreateSqlMap()
    {
        return new SqlMap
        {
            Scope = "TestScope",
            Statements = new Dictionary<string, Statement>(StringComparer.OrdinalIgnoreCase),
            Caches = new Dictionary<string, CacheCfg>(StringComparer.OrdinalIgnoreCase),
            AutoConverter = SmartSql.AutoConverter.NoneAutoConverter.INSTANCE
        };
    }

    private static IDbSession CreateDbSession()
    {
        var mock = new Mock<IDbSession>();
        return mock.Object;
    }

    private static void SetupMiddleware(
        PrepareStatementMiddleware middleware,
        SmartSqlConfig config)
    {
        var builder = new SmartSqlBuilder();
        var configProp = typeof(SmartSqlBuilder).GetProperty("SmartSqlConfig");
        configProp.SetValue(builder, config);

        middleware.SetupSmartSql(builder);
    }
}
