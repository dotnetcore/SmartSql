using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using SmartSql.Configuration;
using SmartSql.DataSource;
using SmartSql.DbSession;
using Xunit;
using InitializerMiddleware = SmartSql.Middlewares.InitializerMiddleware;
using CacheCfg = SmartSql.Configuration.Cache;

namespace SmartSql.Test.Unit.Middlewares;

public class InitializerMiddlewareTests
{
    [Fact]
    public void Should_HaveOrderZero()
    {
        var middleware = new InitializerMiddleware();

        middleware.Order.Should().Be(0);
    }

    [Fact]
    public void Should_SetIsStatementSqlFalse_When_RealSqlIsProvided()
    {
        var middleware = new InitializerMiddleware();
        var config = CreateSmartSqlConfig();
        SetupMiddleware(middleware, config);
        var dbSession = CreateDbSession();
        var request = new RequestContext
        {
            RealSql = "SELECT * FROM Users WHERE Id=@Id"
        };
        var result = new SingleResultContext<string>();
        var context = new ExecutionContext
        {
            SmartSqlConfig = config,
            DbSession = dbSession,
            Request = request,
            Result = result
        };
        request.ExecutionContext = context;

        middleware.Invoke<string>(context);

        request.IsStatementSql.Should().BeFalse();
    }

    [Fact]
    public void Should_ResolveStatementFromConfig_When_ScopeAndSqlIdProvided()
    {
        var middleware = new InitializerMiddleware();
        var config = CreateSmartSqlConfig();
        var sqlMap = CreateSqlMap();
        var statement = new Statement
        {
            SqlMap = sqlMap,
            Id = "GetById",
            StatementType = StatementType.Select
        };
        sqlMap.Statements["TestScope.GetById"] = statement;
        config.SqlMaps["TestScope"] = sqlMap;
        SetupMiddleware(middleware, config);

        var dbSession = CreateDbSession();
        var request = new RequestContext
        {
            Scope = "TestScope",
            SqlId = "GetById"
        };
        var result = new SingleResultContext<string>();
        var context = new ExecutionContext
        {
            SmartSqlConfig = config,
            DbSession = dbSession,
            Request = request,
            Result = result
        };
        request.ExecutionContext = context;

        middleware.Invoke<string>(context);

        request.Statement.Should().NotBeNull();
        request.Statement.Id.Should().Be("GetById");
    }

    [Fact]
    public void Should_SetDataSourceChoiceToRead_When_StatementTypeIsSelect()
    {
        var middleware = new InitializerMiddleware();
        var config = CreateSmartSqlConfig();
        var sqlMap = CreateSqlMap();
        var statement = new Statement
        {
            SqlMap = sqlMap,
            Id = "GetById",
            StatementType = StatementType.Select
        };
        sqlMap.Statements["TestScope.GetById"] = statement;
        config.SqlMaps["TestScope"] = sqlMap;
        SetupMiddleware(middleware, config);

        var dbSession = CreateDbSession();
        var request = new RequestContext
        {
            Scope = "TestScope",
            SqlId = "GetById"
        };
        var result = new SingleResultContext<string>();
        var context = new ExecutionContext
        {
            SmartSqlConfig = config,
            DbSession = dbSession,
            Request = request,
            Result = result
        };
        request.ExecutionContext = context;

        middleware.Invoke<string>(context);

        request.DataSourceChoice.Should().Be(DataSourceChoice.Read);
    }

    [Fact]
    public void Should_SetDataSourceChoiceToWrite_When_StatementTypeIsInsert()
    {
        var middleware = new InitializerMiddleware();
        var config = CreateSmartSqlConfig();
        var sqlMap = CreateSqlMap();
        var statement = new Statement
        {
            SqlMap = sqlMap,
            Id = "Insert",
            StatementType = StatementType.Insert
        };
        sqlMap.Statements["TestScope.Insert"] = statement;
        config.SqlMaps["TestScope"] = sqlMap;
        SetupMiddleware(middleware, config);

        var dbSession = CreateDbSession();
        var request = new RequestContext
        {
            Scope = "TestScope",
            SqlId = "Insert"
        };
        var result = new SingleResultContext<string>();
        var context = new ExecutionContext
        {
            SmartSqlConfig = config,
            DbSession = dbSession,
            Request = request,
            Result = result
        };
        request.ExecutionContext = context;

        middleware.Invoke<string>(context);

        request.DataSourceChoice.Should().Be(DataSourceChoice.Write);
    }

    [Fact]
    public void Should_CopyCommandType_FromStatement()
    {
        var middleware = new InitializerMiddleware();
        var config = CreateSmartSqlConfig();
        var sqlMap = CreateSqlMap();
        var statement = new Statement
        {
            SqlMap = sqlMap,
            Id = "GetById",
            StatementType = StatementType.Select,
            CommandType = System.Data.CommandType.StoredProcedure
        };
        sqlMap.Statements["TestScope.GetById"] = statement;
        config.SqlMaps["TestScope"] = sqlMap;
        SetupMiddleware(middleware, config);
        var dbSession = CreateDbSession();
        var request = new RequestContext
        {
            Scope = "TestScope",
            SqlId = "GetById"
        };
        var result = new SingleResultContext<string>();
        var context = new ExecutionContext
        {
            SmartSqlConfig = config,
            DbSession = dbSession,
            Request = request,
            Result = result
        };
        request.ExecutionContext = context;

        middleware.Invoke<string>(context);

        request.CommandType.Should().Be(System.Data.CommandType.StoredProcedure);
    }

    [Fact]
    public void Should_CopyCacheIdAndCache_FromStatement()
    {
        var middleware = new InitializerMiddleware();
        var config = CreateSmartSqlConfig();
        var sqlMap = CreateSqlMap();
        var cache = new CacheCfg { Id = "TestScope.UserCache" };
        sqlMap.Caches["TestScope.UserCache"] = cache;
        var statement = new Statement
        {
            SqlMap = sqlMap,
            Id = "GetById",
            StatementType = StatementType.Select,
            CacheId = "UserCache",
            Cache = cache
        };
        sqlMap.Statements["TestScope.GetById"] = statement;
        config.SqlMaps["TestScope"] = sqlMap;
        SetupMiddleware(middleware, config);
        var dbSession = CreateDbSession();
        var request = new RequestContext
        {
            Scope = "TestScope",
            SqlId = "GetById"
        };
        var result = new SingleResultContext<string>();
        var context = new ExecutionContext
        {
            SmartSqlConfig = config,
            DbSession = dbSession,
            Request = request,
            Result = result
        };
        request.ExecutionContext = context;

        middleware.Invoke<string>(context);

        request.CacheId.Should().Be("UserCache");
        request.Cache.Should().NotBeNull();
        request.Cache.Id.Should().Be("TestScope.UserCache");
    }

    [Fact]
    public void Should_CopyTransaction_FromStatement()
    {
        var middleware = new InitializerMiddleware();
        var config = CreateSmartSqlConfig();
        var sqlMap = CreateSqlMap();
        var statement = new Statement
        {
            SqlMap = sqlMap,
            Id = "GetById",
            StatementType = StatementType.Select,
            Transaction = System.Data.IsolationLevel.ReadUncommitted
        };
        sqlMap.Statements["TestScope.GetById"] = statement;
        config.SqlMaps["TestScope"] = sqlMap;
        SetupMiddleware(middleware, config);
        var dbSession = CreateDbSession();
        var request = new RequestContext
        {
            Scope = "TestScope",
            SqlId = "GetById"
        };
        var result = new SingleResultContext<string>();
        var context = new ExecutionContext
        {
            SmartSqlConfig = config,
            DbSession = dbSession,
            Request = request,
            Result = result
        };
        request.ExecutionContext = context;

        middleware.Invoke<string>(context);

        request.Transaction.Should().Be(System.Data.IsolationLevel.ReadUncommitted);
    }

    [Fact]
    public void Should_CopyCommandTimeout_FromStatement()
    {
        var middleware = new InitializerMiddleware();
        var config = CreateSmartSqlConfig();
        var sqlMap = CreateSqlMap();
        var statement = new Statement
        {
            SqlMap = sqlMap,
            Id = "GetById",
            StatementType = StatementType.Select,
            CommandTimeout = 30
        };
        sqlMap.Statements["TestScope.GetById"] = statement;
        config.SqlMaps["TestScope"] = sqlMap;
        SetupMiddleware(middleware, config);
        var dbSession = CreateDbSession();
        var request = new RequestContext
        {
            Scope = "TestScope",
            SqlId = "GetById"
        };
        var result = new SingleResultContext<string>();
        var context = new ExecutionContext
        {
            SmartSqlConfig = config,
            DbSession = dbSession,
            Request = request,
            Result = result
        };
        request.ExecutionContext = context;

        middleware.Invoke<string>(context);

        request.CommandTimeout.Should().Be(30);
    }

    [Fact]
    public void Should_CopyReadDb_FromStatement()
    {
        var middleware = new InitializerMiddleware();
        var config = CreateSmartSqlConfig();
        var sqlMap = CreateSqlMap();
        var statement = new Statement
        {
            SqlMap = sqlMap,
            Id = "GetById",
            StatementType = StatementType.Select,
            ReadDb = "ReadDb1"
        };
        sqlMap.Statements["TestScope.GetById"] = statement;
        config.SqlMaps["TestScope"] = sqlMap;
        SetupMiddleware(middleware, config);
        var dbSession = CreateDbSession();
        var request = new RequestContext
        {
            Scope = "TestScope",
            SqlId = "GetById"
        };
        var result = new SingleResultContext<string>();
        var context = new ExecutionContext
        {
            SmartSqlConfig = config,
            DbSession = dbSession,
            Request = request,
            Result = result
        };
        request.ExecutionContext = context;

        middleware.Invoke<string>(context);

        request.ReadDb.Should().Be("ReadDb1");
    }

    [Fact]
    public async Task Should_WorkWithInvokeAsync()
    {
        var middleware = new InitializerMiddleware();
        var config = CreateSmartSqlConfig();
        var sqlMap = CreateSqlMap();
        var statement = new Statement
        {
            SqlMap = sqlMap,
            Id = "GetById",
            StatementType = StatementType.Select
        };
        sqlMap.Statements["TestScope.GetById"] = statement;
        config.SqlMaps["TestScope"] = sqlMap;
        SetupMiddleware(middleware, config);
        var dbSession = CreateDbSession();
        var request = new RequestContext
        {
            Scope = "TestScope",
            SqlId = "GetById"
        };
        var result = new SingleResultContext<string>();
        var context = new ExecutionContext
        {
            SmartSqlConfig = config,
            DbSession = dbSession,
            Request = request,
            Result = result
        };
        request.ExecutionContext = context;

        await middleware.InvokeAsync<string>(context);

        request.Statement.Should().NotBeNull();
        request.Statement.Id.Should().Be("GetById");
    }

    private static SmartSqlConfig CreateSmartSqlConfig()
    {
        return new SmartSqlConfig();
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

    private static void SetupMiddleware(InitializerMiddleware middleware, SmartSqlConfig config)
    {
        var builder = new SmartSqlBuilder();
        var configProp = typeof(SmartSqlBuilder).GetProperty("SmartSqlConfig");
        configProp.SetValue(builder, config);

        middleware.SetupSmartSql(builder);
    }
}
