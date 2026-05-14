using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Reflection;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using SmartSql.Configuration;
using SmartSql.Data;
using SmartSql.DataSource;
using SmartSql.DbSession;
using SmartSql.Utils;
using Xunit;
using Microsoft.Data.Sqlite;
using PrepareStatementMiddleware = SmartSql.Middlewares.PrepareStatementMiddleware;
using CacheCfg = SmartSql.Configuration.Cache;
using StatementTypeCfg = SmartSql.Configuration.StatementType;

namespace SmartSql.Test.Unit.Middlewares;

public class PrepareStatementMiddlewareDeepTests
{
    [Fact]
    public void Should_HaveOrderOneHundred()
    {
        var middleware = new PrepareStatementMiddleware();

        middleware.Order.Should().Be(100);
    }

    [Fact]
    public void Should_SkipSqlBuild_When_IsStatementSqlIsFalse()
    {
        var (middleware, config) = CreateMiddlewareWithConfig();
        var dbSession = CreateDbSession();
        var request = new RequestContext { RealSql = "SELECT * FROM Users WHERE Id = @Id" };
        SetIsStatementSql(request, false);
        request.CommandType = CommandType.Text;

        var context = CreateExecutionContext(config, request, dbSession);
        request.ExecutionContext = context;
        request.SetupParameters();

        var originalSql = request.RealSql;
        middleware.Invoke<string>(context);

        request.RealSql.Should().Be(originalSql);
    }

    [Fact]
    public async Task Should_SkipSqlBuildAsync_When_IsStatementSqlIsFalse()
    {
        var (middleware, config) = CreateMiddlewareWithConfig();
        var dbSession = CreateDbSession();
        var request = new RequestContext { RealSql = "SELECT 1" };
        SetIsStatementSql(request, false);
        request.CommandType = CommandType.Text;

        var context = CreateExecutionContext(config, request, dbSession);
        request.ExecutionContext = context;
        request.SetupParameters();

        var originalSql = request.RealSql;
        await middleware.InvokeAsync<string>(context);

        request.RealSql.Should().Be(originalSql);
    }

    [Fact]
    public void Should_BuildSqlFromStatementTags_When_IsStatementSqlIsTrue()
    {
        var (middleware, config) = CreateMiddlewareWithConfig();
        var dbSession = CreateDbSession();

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

        var request = new RequestContext { Scope = "TestScope", SqlId = "GetById" };
        SetStatement(request, statement);
        SetIsStatementSql(request, true);
        request.CommandType = CommandType.Text;

        var context = CreateExecutionContext(config, request, dbSession);
        request.ExecutionContext = context;
        request.SetupParameters();

        middleware.Invoke<string>(context);

        request.RealSql.Should().Be("SELECT * FROM Users WHERE Id = @Id");
    }

    [Fact]
    public void Should_BuildEmptyString_When_StatementHasNoTags()
    {
        var (middleware, config) = CreateMiddlewareWithConfig();
        var dbSession = CreateDbSession();

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

        var request = new RequestContext { Scope = "TestScope", SqlId = "GetAll" };
        SetStatement(request, statement);
        SetIsStatementSql(request, true);
        request.CommandType = CommandType.Text;

        var context = CreateExecutionContext(config, request, dbSession);
        request.ExecutionContext = context;
        request.SetupParameters();

        middleware.Invoke<string>(context);

        request.RealSql.Should().BeEmpty();
    }

    [Fact]
    public void Should_ReplaceSqlParameters_When_RealSqlHasParams()
    {
        var (middleware, config) = CreateMiddlewareWithConfig();
        var dbSession = CreateDbSession();
        var request = new RequestContext { RealSql = "SELECT * FROM Users WHERE Id = @Id" };
        SetIsStatementSql(request, false);
        request.CommandType = CommandType.Text;

        var sqlParams = new SqlParameterCollection();
        sqlParams.Add(new SqlParameter("Id", 42, typeof(int)));
        SetParameters(request, sqlParams);

        var context = CreateExecutionContext(config, request, dbSession);
        request.ExecutionContext = context;

        middleware.Invoke<string>(context);

        request.Parameters["Id"].SourceParameter.Should().NotBeNull();
        request.Parameters["Id"].SourceParameter.ParameterName.Should().Be("Id");
        request.Parameters["Id"].SourceParameter.Value.Should().Be(42);
    }

    [Fact]
    public void Should_BuildStoredProcedureParameters_When_CommandTypeIsStoredProcedure()
    {
        var (middleware, config) = CreateMiddlewareWithConfig();
        var dbSession = CreateDbSession();
        var request = new RequestContext { RealSql = "sp_GetUser", CommandType = CommandType.StoredProcedure };
        SetIsStatementSql(request, false);

        var sqlParams = new SqlParameterCollection();
        sqlParams.Add(new SqlParameter("Id", 42, typeof(int)));
        SetParameters(request, sqlParams);

        var context = CreateExecutionContext(config, request, dbSession);
        request.ExecutionContext = context;

        middleware.Invoke<string>(context);

        request.Parameters["Id"].SourceParameter.Should().NotBeNull();
        request.Parameters["Id"].SourceParameter.ParameterName.Should().Be("Id");
    }

    [Fact]
    public void Should_SetDbTypeAndDirection_When_ParameterHasThem()
    {
        var (middleware, config) = CreateMiddlewareWithConfig();
        var dbSession = CreateDbSession();
        var request = new RequestContext { RealSql = "SELECT * FROM Users WHERE Name = @Name" };
        SetIsStatementSql(request, false);
        request.CommandType = CommandType.Text;

        var sqlParams = new SqlParameterCollection();
        var sqlParameter = new SqlParameter("Name", "Test", typeof(string))
        {
            DbType = DbType.AnsiString,
            Direction = ParameterDirection.Input,
            Size = 50
        };
        sqlParams.Add(sqlParameter);
        SetParameters(request, sqlParams);

        var context = CreateExecutionContext(config, request, dbSession);
        request.ExecutionContext = context;

        middleware.Invoke<string>(context);

        sqlParameter.SourceParameter.Should().NotBeNull();
        sqlParameter.SourceParameter.DbType.Should().Be(DbType.AnsiString);
        sqlParameter.SourceParameter.Direction.Should().Be(ParameterDirection.Input);
        sqlParameter.SourceParameter.Size.Should().Be(50);
    }

    [Fact]
    public void Should_SkipSourceParameterCreation_When_StoredProcedureWithExistingSourceParam()
    {
        var (middleware, config) = CreateMiddlewareWithConfig();
        var dbSession = CreateDbSession();
        var request = new RequestContext { RealSql = "sp_GetUser", CommandType = CommandType.StoredProcedure };
        SetIsStatementSql(request, false);

        var sqlParams = new SqlParameterCollection();
        var existingSourceParam = SqliteFactory.Instance.CreateParameter();
        existingSourceParam.ParameterName = "Id";
        existingSourceParam.Value = 42;
        var sqlParameter = new SqlParameter("Id", 42, typeof(int));
        sqlParameter.SourceParameter = existingSourceParam;
        sqlParams.Add(sqlParameter);
        SetParameters(request, sqlParams);

        var context = CreateExecutionContext(config, request, dbSession);
        request.ExecutionContext = context;

        middleware.Invoke<string>(context);

        sqlParameter.SourceParameter.Should().BeSameAs(existingSourceParam);
    }

    [Fact]
    public void Should_SetParameterWithDefaultValues_When_NoOptionalPropertiesSet()
    {
        var (middleware, config) = CreateMiddlewareWithConfig();
        var dbSession = CreateDbSession();
        var request = new RequestContext { RealSql = "SELECT * FROM Users WHERE Name = @Name" };
        SetIsStatementSql(request, false);
        request.CommandType = CommandType.Text;

        var sqlParams = new SqlParameterCollection();
        var sqlParameter = new SqlParameter("Name", "Test", typeof(string));
        sqlParams.Add(sqlParameter);
        SetParameters(request, sqlParams);

        var context = CreateExecutionContext(config, request, dbSession);
        request.ExecutionContext = context;

        middleware.Invoke<string>(context);

        sqlParameter.SourceParameter.Should().NotBeNull();
        sqlParameter.SourceParameter.ParameterName.Should().Be("Name");
    }

    [Fact]
    public void Should_HandleStoredProcedureWithNullValue_When_ParameterValueIsNull()
    {
        var (middleware, config) = CreateMiddlewareWithConfig();
        var dbSession = CreateDbSession();
        var request = new RequestContext { RealSql = "sp_GetUser", CommandType = CommandType.StoredProcedure };
        SetIsStatementSql(request, false);

        var sqlParams = new SqlParameterCollection();
        var sqlParameter = new SqlParameter("Name", null, typeof(string));
        sqlParams.Add(sqlParameter);
        SetParameters(request, sqlParams);

        var context = CreateExecutionContext(config, request, dbSession);
        request.ExecutionContext = context;

        middleware.Invoke<string>(context);

        sqlParameter.SourceParameter.Should().NotBeNull();
        sqlParameter.SourceParameter.Value.Should().BeNull();
    }

    [Fact]
    public void Should_HandleMultipleParameters_When_MultipleParamsInSql()
    {
        var (middleware, config) = CreateMiddlewareWithConfig();
        var dbSession = CreateDbSession();
        var request = new RequestContext
        {
            RealSql = "SELECT * FROM Users WHERE Id = @Id AND Name = @Name"
        };
        SetIsStatementSql(request, false);
        request.CommandType = CommandType.Text;

        var sqlParams = new SqlParameterCollection();
        sqlParams.Add(new SqlParameter("Id", 1, typeof(int)));
        sqlParams.Add(new SqlParameter("Name", "Test", typeof(string)));
        SetParameters(request, sqlParams);

        var context = CreateExecutionContext(config, request, dbSession);
        request.ExecutionContext = context;

        middleware.Invoke<string>(context);

        request.Parameters["Id"].SourceParameter.Should().NotBeNull();
        request.Parameters["Name"].SourceParameter.Should().NotBeNull();
    }

    [Fact]
    public void Should_UseParamMapHandler_When_MappedWithReflection()
    {
        var (middleware, config) = CreateMiddlewareWithConfig();
        var dbSession = CreateDbSession();
        var request = new RequestContext { RealSql = "SELECT * FROM Users WHERE Id = @Id" };
        SetIsStatementSql(request, false);
        request.CommandType = CommandType.Text;

        var paramMap = new ParameterMap();
        paramMap.Parameters = new Dictionary<string, Parameter>(StringComparer.OrdinalIgnoreCase)
        {
            { "Id", new Parameter { Property = "Id", DbType = DbType.Int64 } }
        };
        SetParameterMap(request, paramMap);

        var sqlParams = new SqlParameterCollection();
        var sqlParameter = new SqlParameter("Id", 42, typeof(int));
        sqlParams.Add(sqlParameter);
        SetParameters(request, sqlParams);

        var context = CreateExecutionContext(config, request, dbSession);
        request.ExecutionContext = context;

        middleware.Invoke<string>(context);

        sqlParameter.SourceParameter.Should().NotBeNull();
        sqlParameter.SourceParameter.ParameterName.Should().Be("Id");
    }

    [Fact]
    public void Should_BuildSqlWithConditionalTag_When_StatementHasIsNotEmptyTag()
    {
        var (middleware, config) = CreateMiddlewareWithConfig();
        var dbSession = CreateDbSession();

        var sqlMap = CreateSqlMap();
        config.SqlMaps["TestScope"] = sqlMap;

        var isNotEmpty = new SmartSql.Configuration.Tags.IsNotEmpty { Property = "Name" };
        isNotEmpty.ChildTags = new List<SmartSql.Configuration.Tags.ITag>
        {
            new SmartSql.Configuration.Tags.SqlText("AND Name = @Name", "@")
        };
        var statement = new Statement
        {
            SqlMap = sqlMap,
            Id = "FindByName",
            StatementType = StatementTypeCfg.Select,
            SqlTags = new List<SmartSql.Configuration.Tags.ITag>
            {
                new SmartSql.Configuration.Tags.SqlText("SELECT * FROM Users WHERE 1=1", "@"),
                isNotEmpty
            }
        };
        sqlMap.Statements["TestScope.FindByName"] = statement;

        var request = new RequestContext { Scope = "TestScope", SqlId = "FindByName" };
        SetStatement(request, statement);
        SetIsStatementSql(request, true);
        request.CommandType = CommandType.Text;

        var sqlParams = new SqlParameterCollection();
        sqlParams.TryAdd("Name", "TestName");
        SetParameters(request, sqlParams);

        var context = new ExecutionContext
        {
            SmartSqlConfig = config,
            DbSession = dbSession,
            Request = request,
            Result = new SingleResultContext<string>()
        };
        request.ExecutionContext = context;
        // Don't call SetupParameters - it would overwrite our manually set Parameters

        middleware.Invoke<string>(context);

        request.RealSql.Should().Contain("SELECT * FROM Users WHERE 1=1");
        request.RealSql.Should().Contain("AND Name = @Name");
    }

    [Fact]
    public void Should_BuildSqlWithoutConditionalTag_When_IsNotEmptyConditionFails()
    {
        var (middleware, config) = CreateMiddlewareWithConfig();
        var dbSession = CreateDbSession();

        var sqlMap = CreateSqlMap();
        config.SqlMaps["TestScope"] = sqlMap;

        var isNotEmpty = new SmartSql.Configuration.Tags.IsNotEmpty { Property = "Name" };
        isNotEmpty.ChildTags = new List<SmartSql.Configuration.Tags.ITag>
        {
            new SmartSql.Configuration.Tags.SqlText("AND Name = @Name", "@")
        };
        var statement = new Statement
        {
            SqlMap = sqlMap,
            Id = "FindByName",
            StatementType = StatementTypeCfg.Select,
            SqlTags = new List<SmartSql.Configuration.Tags.ITag>
            {
                new SmartSql.Configuration.Tags.SqlText("SELECT * FROM Users WHERE 1=1", "@"),
                isNotEmpty
            }
        };
        sqlMap.Statements["TestScope.FindByName"] = statement;

        var request = new RequestContext { Scope = "TestScope", SqlId = "FindByName" };
        SetStatement(request, statement);
        SetIsStatementSql(request, true);
        request.CommandType = CommandType.Text;

        // Don't add Name parameter - IsNotEmpty should fail
        var sqlParams = new SqlParameterCollection();
        SetParameters(request, sqlParams);

        var context = new ExecutionContext
        {
            SmartSqlConfig = config,
            DbSession = dbSession,
            Request = request,
            Result = new SingleResultContext<string>()
        };
        request.ExecutionContext = context;

        middleware.Invoke<string>(context);

        request.RealSql.Should().Contain("SELECT * FROM Users WHERE 1=1");
        request.RealSql.Should().NotContain("AND Name = @Name");
    }

    [Fact]
    public async Task Should_ReplaceSqlParametersAsync_When_RealSqlHasParams()
    {
        var (middleware, config) = CreateMiddlewareWithConfig();
        var dbSession = CreateDbSession();
        var request = new RequestContext { RealSql = "SELECT * FROM Users WHERE Id = @Id" };
        SetIsStatementSql(request, false);
        request.CommandType = CommandType.Text;

        var sqlParams = new SqlParameterCollection();
        sqlParams.Add(new SqlParameter("Id", 42, typeof(int)));
        SetParameters(request, sqlParams);

        var context = CreateExecutionContext(config, request, dbSession);
        request.ExecutionContext = context;

        await middleware.InvokeAsync<string>(context);

        request.Parameters["Id"].SourceParameter.Should().NotBeNull();
        request.Parameters["Id"].SourceParameter.ParameterName.Should().Be("Id");
    }

    #region Helper Methods

    private static void SetStatement(AbstractRequestContext request, Statement statement)
    {
        typeof(AbstractRequestContext).GetProperty("Statement",
            BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
            .SetValue(request, statement);
    }

    private static void SetIsStatementSql(AbstractRequestContext request, bool value)
    {
        typeof(AbstractRequestContext).GetProperty("IsStatementSql",
            BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
            .SetValue(request, value);
    }

    private static void SetParameters(AbstractRequestContext request, SqlParameterCollection parameters)
    {
        typeof(AbstractRequestContext).GetProperty("Parameters",
            BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
            .SetValue(request, parameters);
    }

    private static void SetParameterMap(AbstractRequestContext request, ParameterMap parameterMap)
    {
        typeof(AbstractRequestContext).GetProperty("ParameterMap",
            BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
            .SetValue(request, parameterMap);
    }

    private static SmartSqlConfig CreateConfig()
    {
        var dbProvider = new DbProvider
        {
            Name = "SQLite",
            ParameterPrefix = "@",
            Factory = SqliteFactory.Instance
        };
        var config = new SmartSqlConfig
        {
            Database = new Database { DbProvider = dbProvider }
        };
        config.SqlParamAnalyzer = new SqlParamAnalyzer(
            config.Settings.IgnoreParameterCase,
            dbProvider.ParameterPrefix);
        return config;
    }

    private static PrepareStatementMiddleware CreateMiddleware()
    {
        return CreateMiddlewareWithConfig().middleware;
    }

    private static (PrepareStatementMiddleware middleware, SmartSqlConfig config) CreateMiddlewareWithConfig()
    {
        var config = CreateConfig();
        var builder = new SmartSqlBuilder();
        typeof(SmartSqlBuilder).GetProperty("SmartSqlConfig").SetValue(builder, config);

        var middleware = new PrepareStatementMiddleware();
        middleware.SetupSmartSql(builder);
        return (middleware, config);
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
        return new Mock<IDbSession>().Object;
    }

    private static ExecutionContext CreateExecutionContext(
        SmartSqlConfig config,
        AbstractRequestContext request,
        IDbSession dbSession)
    {
        return new ExecutionContext
        {
            SmartSqlConfig = config,
            DbSession = dbSession,
            Request = request,
            Result = new SingleResultContext<string>()
        };
    }

    #endregion
}
