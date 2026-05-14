using System;
using System.Collections.Generic;
using System.Data;
using FluentAssertions;
using Microsoft.Data.Sqlite;
using SmartSql.Configuration;
using SmartSql.Data;
using SmartSql.DataSource;
using SmartSql.DbSession;
using SmartSql.Utils;
using Xunit;

namespace SmartSql.Test.Unit.Utils;

public class ExecutionContextExtensionsTests
{
    private static ExecutionContext CreateExecutionContext(
        string realSql = "SELECT * FROM T_User WHERE Id=@Id",
        Action<SqlParameterCollection> configureParameters = null)
    {
        var dbProvider = DbProviderManager.SQLITE_DBPROVIDER;
        var config = new SmartSqlConfig
        {
            Database = new Database { DbProvider = dbProvider },
            SqlParamAnalyzer = new SqlParamAnalyzer(false, "@")
        };

        var sqlParams = new SqlParameterCollection();
        configureParameters?.Invoke(sqlParams);

        var requestContext = new RequestContext
        {
            Scope = "TestScope",
            SqlId = "GetById",
            RealSql = realSql,
            Request = new { }
        };

        var executionContext = new ExecutionContext
        {
            SmartSqlConfig = config,
            Request = requestContext
        };
        requestContext.ExecutionContext = executionContext;

        // Initialize the Parameters collection via SetupParameters
        requestContext.SetupParameters();

        // Populate DbParameters from the configured sqlParams
        foreach (var dbParam in sqlParams.DbParameters.Values)
        {
            requestContext.Parameters.DbParameters.Add(dbParam.ParameterName, dbParam);
        }

        return executionContext;
    }

    private static SqliteParameter CreateDbParameter(string name, object value, DbType dbType = DbType.String)
    {
        return new SqliteParameter(name, value) { DbType = dbType };
    }

    #region FormatSql

    [Fact]
    public void Should_FormatSqlWithoutParameterValue_When_WithParameterValueIsFalse()
    {
        // Arrange
        var context = CreateExecutionContext(configureParameters: p =>
        {
            p.DbParameters.Add("Id", CreateDbParameter("Id", 42, DbType.Int32));
        });

        // Act
        var result = context.FormatSql(withParameterValue: false);

        // Assert
        result.Should().Contain("Statement.Id:[TestScope.GetById]");
        result.Should().Contain("Sql:");
        result.Should().Contain("SELECT * FROM T_User WHERE Id=@Id");
        result.Should().Contain("Parameters:[Id=42]");
        result.Should().NotContain("Sql with parameter value:");
    }

    [Fact]
    public void Should_FormatSqlWithParameterValue_When_WithParameterValueIsTrue()
    {
        // Arrange
        var context = CreateExecutionContext(configureParameters: p =>
        {
            p.DbParameters.Add("Id", CreateDbParameter("Id", 42, DbType.Int32));
        });

        // Act
        var result = context.FormatSql(withParameterValue: true);

        // Assert
        result.Should().Contain("Sql with parameter value:");
        result.Should().Contain("42");
    }

    [Fact]
    public void Should_FormatSqlWithMultipleParameters_When_SqlHasMultipleParams()
    {
        // Arrange
        var context = CreateExecutionContext(
            realSql: "SELECT * FROM T_User WHERE Name=@Name AND Age=@Age",
            configureParameters: p =>
            {
                p.DbParameters.Add("Name", CreateDbParameter("Name", "Alice", DbType.String));
                p.DbParameters.Add("Age", CreateDbParameter("Age", 30, DbType.Int32));
            });

        // Act
        var result = context.FormatSql(withParameterValue: false);

        // Assert
        result.Should().Contain("Name=Alice");
        result.Should().Contain("Age=30");
    }

    [Fact]
    public void Should_ShowEmptyParameters_When_NoDbParameters()
    {
        // Arrange
        var context = CreateExecutionContext(realSql: "SELECT * FROM T_User");

        // Act
        var result = context.FormatSql(withParameterValue: false);

        // Assert
        result.Should().Contain("Parameters:[]");
    }

    [Fact]
    public void Should_QuoteStringValue_When_FormattingWithParameterValues()
    {
        // Arrange
        var context = CreateExecutionContext(
            realSql: "SELECT * FROM T_User WHERE Name=@Name",
            configureParameters: p =>
            {
                p.DbParameters.Add("Name", CreateDbParameter("Name", "Alice", DbType.String));
            });

        // Act
        var result = context.FormatSql(withParameterValue: true);

        // Assert
        result.Should().Contain("'Alice'");
    }

    [Fact]
    public void Should_ReturnNull_When_ParameterValueIsDBNull()
    {
        // Arrange
        var context = CreateExecutionContext(
            realSql: "SELECT * FROM T_User WHERE Name=@Name",
            configureParameters: p =>
            {
                p.DbParameters.Add("Name", CreateDbParameter("Name", DBNull.Value, DbType.String));
            });

        // Act
        var result = context.FormatSql(withParameterValue: true);

        // Assert
        result.Should().Contain("NULL");
    }

    [Fact]
    public void Should_ConvertBooleanToOneOrZero_When_FormattingWithParameterValues()
    {
        // Arrange
        var context = CreateExecutionContext(
            realSql: "SELECT * FROM T_User WHERE Active=@Active",
            configureParameters: p =>
            {
                p.DbParameters.Add("Active", CreateDbParameter("Active", true, DbType.Boolean));
            });

        // Act
        var result = context.FormatSql(withParameterValue: true);

        // Assert
        result.Should().Contain("1");
    }

    [Fact]
    public void Should_ConvertBooleanFalseToZero_When_FormattingWithParameterValues()
    {
        // Arrange
        var context = CreateExecutionContext(
            realSql: "SELECT * FROM T_User WHERE Active=@Active",
            configureParameters: p =>
            {
                p.DbParameters.Add("Active", CreateDbParameter("Active", false, DbType.Boolean));
            });

        // Act
        var result = context.FormatSql(withParameterValue: true);

        // Assert
        result.Should().Contain("0");
    }

    [Fact]
    public void Should_IncludeStatementId_When_Formatting()
    {
        // Arrange
        var context = CreateExecutionContext(realSql: "SELECT 1");

        // Act
        var result = context.FormatSql(withParameterValue: false);

        // Assert
        result.Should().StartWith("Statement.Id:[TestScope.GetById],");
    }

    [Fact]
    public void Should_QuoteDateTimeValue_When_FormattingWithParameterValues()
    {
        // Arrange
        var date = new DateTime(2024, 1, 15, 10, 30, 0);
        var context = CreateExecutionContext(
            realSql: "SELECT * FROM T_User WHERE CreatedAt=@CreatedAt",
            configureParameters: p =>
            {
                p.DbParameters.Add("CreatedAt", CreateDbParameter("CreatedAt", date, DbType.DateTime));
            });

        // Act
        var result = context.FormatSql(withParameterValue: true);

        // Assert
        result.Should().Contain($"'{date}'");
    }

    [Fact]
    public void Should_QuoteGuidValue_When_FormattingWithParameterValues()
    {
        // Arrange
        var guid = Guid.Parse("550e8400-e29b-41d4-a716-446655440000");
        var context = CreateExecutionContext(
            realSql: "SELECT * FROM T_User WHERE Uid=@Uid",
            configureParameters: p =>
            {
                p.DbParameters.Add("Uid", CreateDbParameter("Uid", guid, DbType.Guid));
            });

        // Act
        var result = context.FormatSql(withParameterValue: true);

        // Assert
        result.Should().Contain($"'{guid}'");
    }

    [Fact]
    public void Should_KeepOriginalParameterPrefix_When_ParameterNotFound()
    {
        // Arrange - SQL has @Name but we don't add that parameter
        var context = CreateExecutionContext(
            realSql: "SELECT * FROM T_User WHERE Name=@Name AND Status=@Status",
            configureParameters: p =>
            {
                p.DbParameters.Add("Status", CreateDbParameter("Status", 1, DbType.Int32));
            });

        // Act
        var result = context.FormatSql(withParameterValue: true);

        // Assert - @Name should remain as-is since no matching parameter exists
        result.Should().Contain("@Name");
        result.Should().Contain("1");
    }

    #endregion
}
