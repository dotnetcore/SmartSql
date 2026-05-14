using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using SmartSql.AutoConverter;
using SmartSql.Configuration;
using SmartSql.DataSource;
using SmartSql.DbSession;
using SmartSql.Exceptions;
using Xunit;
using CacheCfg = SmartSql.Configuration.Cache;
using StatementType = SmartSql.Configuration.StatementType;

namespace SmartSql.Test.Unit.Middlewares;

public class InitializerMiddlewareDeepTests
{
    #region InitByStatement - DataSourceChoice

    [Fact]
    public void Should_SetDataSourceChoiceToWrite_When_StatementTypeIsUpdate()
    {
        var middleware = new SmartSql.Middlewares.InitializerMiddleware();
        var config = CreateSmartSqlConfig();
        var sqlMap = CreateSqlMap();
        var statement = new Statement
        {
            SqlMap = sqlMap,
            Id = "Update",
            StatementType = StatementType.Update
        };
        sqlMap.Statements["TestScope.Update"] = statement;
        config.SqlMaps["TestScope"] = sqlMap;
        SetupMiddleware(middleware, config);

        var dbSession = CreateDbSession();
        var request = new RequestContext
        {
            Scope = "TestScope",
            SqlId = "Update"
        };
        var result = new SingleResultContext<int>();
        var context = new ExecutionContext
        {
            SmartSqlConfig = config,
            DbSession = dbSession,
            Request = request,
            Result = result
        };
        request.ExecutionContext = context;

        middleware.Invoke<int>(context);

        request.DataSourceChoice.Should().Be(DataSourceChoice.Write);
    }

    [Fact]
    public void Should_SetDataSourceChoiceToWrite_When_StatementTypeIsDelete()
    {
        var middleware = new SmartSql.Middlewares.InitializerMiddleware();
        var config = CreateSmartSqlConfig();
        var sqlMap = CreateSqlMap();
        var statement = new Statement
        {
            SqlMap = sqlMap,
            Id = "Delete",
            StatementType = StatementType.Delete
        };
        sqlMap.Statements["TestScope.Delete"] = statement;
        config.SqlMaps["TestScope"] = sqlMap;
        SetupMiddleware(middleware, config);

        var dbSession = CreateDbSession();
        var request = new RequestContext
        {
            Scope = "TestScope",
            SqlId = "Delete"
        };
        var result = new SingleResultContext<int>();
        var context = new ExecutionContext
        {
            SmartSqlConfig = config,
            DbSession = dbSession,
            Request = request,
            Result = result
        };
        request.ExecutionContext = context;

        middleware.Invoke<int>(context);

        request.DataSourceChoice.Should().Be(DataSourceChoice.Write);
    }

    [Fact]
    public void Should_UseStatementSourceChoice_When_SourceChoiceIsSet()
    {
        var middleware = new SmartSql.Middlewares.InitializerMiddleware();
        var config = CreateSmartSqlConfig();
        var sqlMap = CreateSqlMap();
        var statement = new Statement
        {
            SqlMap = sqlMap,
            Id = "GetById",
            StatementType = StatementType.Select,
            SourceChoice = DataSourceChoice.Write
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

        request.DataSourceChoice.Should().Be(DataSourceChoice.Write);
    }

    [Fact]
    public void Should_NotOverrideDataSourceChoice_When_AlreadySet()
    {
        var middleware = new SmartSql.Middlewares.InitializerMiddleware();
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
            SqlId = "GetById",
            DataSourceChoice = DataSourceChoice.Write
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

    #endregion

    #region InitByStatement - CommandType

    [Fact]
    public void Should_CopyCommandType_When_StatementHasCommandType()
    {
        var middleware = new SmartSql.Middlewares.InitializerMiddleware();
        var config = CreateSmartSqlConfig();
        var sqlMap = CreateSqlMap();
        var statement = new Statement
        {
            SqlMap = sqlMap,
            Id = "GetById",
            StatementType = StatementType.Select,
            CommandType = CommandType.StoredProcedure
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

        request.CommandType.Should().Be(CommandType.StoredProcedure);
    }

    [Fact]
    public void Should_KeepDefaultCommandType_When_StatementCommandTypeIsNull()
    {
        var middleware = new SmartSql.Middlewares.InitializerMiddleware();
        var config = CreateSmartSqlConfig();
        var sqlMap = CreateSqlMap();
        var statement = new Statement
        {
            SqlMap = sqlMap,
            Id = "GetById",
            StatementType = StatementType.Select,
            CommandType = null
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

        request.CommandType.Should().Be(CommandType.Text);
    }

    #endregion

    #region InitByStatement - Property Changed Track

    [Fact]
    public void Should_CopyEnablePropertyChangedTrack_When_RequestHasNoValue()
    {
        var middleware = new SmartSql.Middlewares.InitializerMiddleware();
        var config = CreateSmartSqlConfig();
        var sqlMap = CreateSqlMap();
        var statement = new Statement
        {
            SqlMap = sqlMap,
            Id = "GetById",
            StatementType = StatementType.Select,
            EnablePropertyChangedTrack = true
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

        request.EnablePropertyChangedTrack.Should().BeTrue();
    }

    [Fact]
    public void Should_NotOverrideEnablePropertyChangedTrack_When_RequestAlreadyHasValue()
    {
        var middleware = new SmartSql.Middlewares.InitializerMiddleware();
        var config = CreateSmartSqlConfig();
        var sqlMap = CreateSqlMap();
        var statement = new Statement
        {
            SqlMap = sqlMap,
            Id = "GetById",
            StatementType = StatementType.Select,
            EnablePropertyChangedTrack = false
        };
        sqlMap.Statements["TestScope.GetById"] = statement;
        config.SqlMaps["TestScope"] = sqlMap;
        SetupMiddleware(middleware, config);

        var dbSession = CreateDbSession();
        var request = new RequestContext
        {
            Scope = "TestScope",
            SqlId = "GetById",
            EnablePropertyChangedTrack = true
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

        request.EnablePropertyChangedTrack.Should().BeTrue();
    }

    #endregion

    #region InitByStatement - Transaction

    [Fact]
    public void Should_NotOverrideTransaction_When_RequestAlreadyHasValue()
    {
        var middleware = new SmartSql.Middlewares.InitializerMiddleware();
        var config = CreateSmartSqlConfig();
        var sqlMap = CreateSqlMap();
        var statement = new Statement
        {
            SqlMap = sqlMap,
            Id = "GetById",
            StatementType = StatementType.Select,
            Transaction = IsolationLevel.ReadCommitted
        };
        sqlMap.Statements["TestScope.GetById"] = statement;
        config.SqlMaps["TestScope"] = sqlMap;
        SetupMiddleware(middleware, config);

        var dbSession = CreateDbSession();
        var request = new RequestContext
        {
            Scope = "TestScope",
            SqlId = "GetById",
            Transaction = IsolationLevel.ReadUncommitted
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

        request.Transaction.Should().Be(IsolationLevel.ReadUncommitted);
    }

    [Fact]
    public void Should_CopyTransaction_When_RequestHasNoValue()
    {
        var middleware = new SmartSql.Middlewares.InitializerMiddleware();
        var config = CreateSmartSqlConfig();
        var sqlMap = CreateSqlMap();
        var statement = new Statement
        {
            SqlMap = sqlMap,
            Id = "GetById",
            StatementType = StatementType.Select,
            Transaction = IsolationLevel.ReadCommitted
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

        request.Transaction.Should().Be(IsolationLevel.ReadCommitted);
    }

    #endregion

    #region InitByStatement - CommandTimeout

    [Fact]
    public void Should_NotOverrideCommandTimeout_When_RequestAlreadyHasValue()
    {
        var middleware = new SmartSql.Middlewares.InitializerMiddleware();
        var config = CreateSmartSqlConfig();
        var sqlMap = CreateSqlMap();
        var statement = new Statement
        {
            SqlMap = sqlMap,
            Id = "GetById",
            StatementType = StatementType.Select,
            CommandTimeout = 60
        };
        sqlMap.Statements["TestScope.GetById"] = statement;
        config.SqlMaps["TestScope"] = sqlMap;
        SetupMiddleware(middleware, config);

        var dbSession = CreateDbSession();
        var request = new RequestContext
        {
            Scope = "TestScope",
            SqlId = "GetById",
            CommandTimeout = 30
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
    public void Should_CopyCommandTimeout_When_RequestHasNoValue()
    {
        var middleware = new SmartSql.Middlewares.InitializerMiddleware();
        var config = CreateSmartSqlConfig();
        var sqlMap = CreateSqlMap();
        var statement = new Statement
        {
            SqlMap = sqlMap,
            Id = "GetById",
            StatementType = StatementType.Select,
            CommandTimeout = 60
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

        request.CommandTimeout.Should().Be(60);
    }

    #endregion

    #region InitByStatement - ReadDb

    [Fact]
    public void Should_NotOverrideReadDb_When_RequestAlreadyHasValue()
    {
        var middleware = new SmartSql.Middlewares.InitializerMiddleware();
        var config = CreateSmartSqlConfig();
        var sqlMap = CreateSqlMap();
        var statement = new Statement
        {
            SqlMap = sqlMap,
            Id = "GetById",
            StatementType = StatementType.Select,
            ReadDb = "ReadDb2"
        };
        sqlMap.Statements["TestScope.GetById"] = statement;
        config.SqlMaps["TestScope"] = sqlMap;
        SetupMiddleware(middleware, config);

        var dbSession = CreateDbSession();
        var request = new RequestContext
        {
            Scope = "TestScope",
            SqlId = "GetById",
            ReadDb = "ReadDb1"
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

    #endregion

    #region InitByStatement - Cache

    [Fact]
    public void Should_NotOverrideCache_When_RequestAlreadyHasCacheId()
    {
        var middleware = new SmartSql.Middlewares.InitializerMiddleware();
        var config = CreateSmartSqlConfig();
        var sqlMap = CreateSqlMap();
        var cache1 = new CacheCfg { Id = "TestScope.Cache1" };
        var cache2 = new CacheCfg { Id = "TestScope.Cache2" };
        sqlMap.Caches["TestScope.Cache1"] = cache1;
        sqlMap.Caches["TestScope.Cache2"] = cache2;
        var statement = new Statement
        {
            SqlMap = sqlMap,
            Id = "GetById",
            StatementType = StatementType.Select,
            CacheId = "Cache2",
            Cache = cache2
        };
        sqlMap.Statements["TestScope.GetById"] = statement;
        config.SqlMaps["TestScope"] = sqlMap;
        SetupMiddleware(middleware, config);

        var dbSession = CreateDbSession();
        var request = new RequestContext
        {
            Scope = "TestScope",
            SqlId = "GetById",
            CacheId = "Cache1"
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

        request.CacheId.Should().Be("Cache1");
        request.Cache.Should().NotBeNull();
        request.Cache.Id.Should().Be("TestScope.Cache1");
    }

    #endregion

    #region InitByStatement - Maps

    [Fact]
    public void Should_CopyParameterMapId_When_StatementHasParameterMap()
    {
        var middleware = new SmartSql.Middlewares.InitializerMiddleware();
        var config = CreateSmartSqlConfig();
        var sqlMap = CreateSqlMap();
        var parameterMap = new ParameterMap { Id = "TestScope.UserParam" };
        sqlMap.ParameterMaps["TestScope.UserParam"] = parameterMap;
        var statement = new Statement
        {
            SqlMap = sqlMap,
            Id = "GetById",
            StatementType = StatementType.Select,
            ParameterMapId = "UserParam",
            ParameterMap = parameterMap
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

        request.ParameterMapId.Should().Be("UserParam");
        request.ParameterMap.Should().BeSameAs(parameterMap);
    }

    [Fact]
    public void Should_CopyResultMapId_When_StatementHasResultMap()
    {
        var middleware = new SmartSql.Middlewares.InitializerMiddleware();
        var config = CreateSmartSqlConfig();
        var sqlMap = CreateSqlMap();
        var resultMap = new ResultMap { Id = "TestScope.UserResult" };
        sqlMap.ResultMaps["TestScope.UserResult"] = resultMap;
        var statement = new Statement
        {
            SqlMap = sqlMap,
            Id = "GetById",
            StatementType = StatementType.Select,
            ResultMapId = "UserResult",
            ResultMap = resultMap
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

        request.ResultMapId.Should().Be("UserResult");
        request.ResultMap.Should().BeSameAs(resultMap);
    }

    [Fact]
    public void Should_CopyMultipleResultMapId_When_StatementHasMultipleResultMap()
    {
        var middleware = new SmartSql.Middlewares.InitializerMiddleware();
        var config = CreateSmartSqlConfig();
        var sqlMap = CreateSqlMap();
        var multipleResultMap = new MultipleResultMap { Id = "TestScope.UserMultiResult" };
        sqlMap.MultipleResultMaps["TestScope.UserMultiResult"] = multipleResultMap;
        var statement = new Statement
        {
            SqlMap = sqlMap,
            Id = "GetById",
            StatementType = StatementType.Select,
            MultipleResultMapId = "UserMultiResult",
            MultipleResultMap = multipleResultMap
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

        request.MultipleResultMapId.Should().Be("UserMultiResult");
        request.MultipleResultMap.Should().BeSameAs(multipleResultMap);
    }

    #endregion

    #region InitByMap - Non-Statement Path

    [Fact]
    public void Should_SetIsStatementSqlFalse_When_RealSqlProvided()
    {
        var middleware = new SmartSql.Middlewares.InitializerMiddleware();
        var config = CreateSmartSqlConfig();
        var sqlMap = CreateSqlMap();
        config.SqlMaps["TestScope"] = sqlMap;
        SetupMiddleware(middleware, config);

        var dbSession = CreateDbSession();
        var request = new RequestContext
        {
            RealSql = "SELECT * FROM Users WHERE Id = @Id",
            Scope = "TestScope"
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
    public void Should_ResolveCache_When_InitByMapWithCacheId()
    {
        var middleware = new SmartSql.Middlewares.InitializerMiddleware();
        var config = CreateSmartSqlConfig();
        var sqlMap = CreateSqlMap();
        var cache = new CacheCfg { Id = "TestScope.TestCache" };
        sqlMap.Caches["TestScope.TestCache"] = cache;
        config.SqlMaps["TestScope"] = sqlMap;
        SetupMiddleware(middleware, config);

        var dbSession = CreateDbSession();
        var request = new RequestContext
        {
            RealSql = "SELECT * FROM Users",
            Scope = "TestScope",
            CacheId = "TestCache"
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

        request.Cache.Should().NotBeNull();
        request.Cache.Id.Should().Be("TestScope.TestCache");
    }

    [Fact]
    public void Should_ResolveParameterMap_When_InitByMapWithParameterMapId()
    {
        var middleware = new SmartSql.Middlewares.InitializerMiddleware();
        var config = CreateSmartSqlConfig();
        var sqlMap = CreateSqlMap();
        var parameterMap = new ParameterMap { Id = "TestScope.UserParam" };
        sqlMap.ParameterMaps["TestScope.UserParam"] = parameterMap;
        config.SqlMaps["TestScope"] = sqlMap;
        SetupMiddleware(middleware, config);

        var dbSession = CreateDbSession();
        var request = new RequestContext
        {
            RealSql = "SELECT * FROM Users",
            Scope = "TestScope",
            ParameterMapId = "UserParam"
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

        request.ParameterMap.Should().NotBeNull();
        request.ParameterMap.Id.Should().Be("TestScope.UserParam");
    }

    [Fact]
    public void Should_ResolveResultMap_When_InitByMapWithResultMapId()
    {
        var middleware = new SmartSql.Middlewares.InitializerMiddleware();
        var config = CreateSmartSqlConfig();
        var sqlMap = CreateSqlMap();
        var resultMap = new ResultMap { Id = "TestScope.UserResult" };
        sqlMap.ResultMaps["TestScope.UserResult"] = resultMap;
        config.SqlMaps["TestScope"] = sqlMap;
        SetupMiddleware(middleware, config);

        var dbSession = CreateDbSession();
        var request = new RequestContext
        {
            RealSql = "SELECT * FROM Users",
            Scope = "TestScope",
            ResultMapId = "UserResult"
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

        request.ResultMap.Should().NotBeNull();
        request.ResultMap.Id.Should().Be("TestScope.UserResult");
    }

    [Fact]
    public void Should_ResolveMultipleResultMap_When_InitByMapWithMultipleResultMapId()
    {
        var middleware = new SmartSql.Middlewares.InitializerMiddleware();
        var config = CreateSmartSqlConfig();
        var sqlMap = CreateSqlMap();
        var multipleResultMap = new MultipleResultMap { Id = "TestScope.UserMultiResult" };
        sqlMap.MultipleResultMaps["TestScope.UserMultiResult"] = multipleResultMap;
        config.SqlMaps["TestScope"] = sqlMap;
        SetupMiddleware(middleware, config);

        var dbSession = CreateDbSession();
        var request = new RequestContext
        {
            RealSql = "SELECT * FROM Users",
            Scope = "TestScope",
            MultipleResultMapId = "UserMultiResult"
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

        request.MultipleResultMap.Should().NotBeNull();
        request.MultipleResultMap.Id.Should().Be("TestScope.UserMultiResult");
    }

    #endregion

    #region SetupSmartSql

    [Fact]
    public void Should_SetSmartSqlConfig_When_SetupSmartSqlCalled()
    {
        var middleware = new SmartSql.Middlewares.InitializerMiddleware();
        var config = CreateSmartSqlConfig();

        var builder = new SmartSqlBuilder();
        var configProp = typeof(SmartSqlBuilder).GetProperty("SmartSqlConfig");
        configProp.SetValue(builder, config);

        middleware.SetupSmartSql(builder);

        // Verify it works by testing an invoke
        var sqlMap = CreateSqlMap();
        var statement = new Statement
        {
            SqlMap = sqlMap,
            Id = "GetById",
            StatementType = StatementType.Select
        };
        sqlMap.Statements["TestScope.GetById"] = statement;
        config.SqlMaps["TestScope"] = sqlMap;

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
    }

    #endregion

    #region Helper Methods

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
            ParameterMaps = new Dictionary<string, ParameterMap>(StringComparer.OrdinalIgnoreCase),
            ResultMaps = new Dictionary<string, ResultMap>(StringComparer.OrdinalIgnoreCase),
            MultipleResultMaps = new Dictionary<string, MultipleResultMap>(StringComparer.OrdinalIgnoreCase),
            AutoConverter = NoneAutoConverter.INSTANCE
        };
    }

    private static IDbSession CreateDbSession()
    {
        var mock = new Mock<IDbSession>();
        return mock.Object;
    }

    private static void SetupMiddleware(SmartSql.Middlewares.InitializerMiddleware middleware, SmartSqlConfig config)
    {
        var builder = new SmartSqlBuilder();
        var configProp = typeof(SmartSqlBuilder).GetProperty("SmartSqlConfig");
        configProp.SetValue(builder, config);

        middleware.SetupSmartSql(builder);
    }

    #endregion
}
