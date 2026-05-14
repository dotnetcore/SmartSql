using System;
using System.Collections.Generic;
using FluentAssertions;
using SmartSql.Configuration;
using SmartSql.Configuration.Tags;
using SmartSql.DataSource;
using Xunit;
using DataCommandType = System.Data.CommandType;
using DataIsolationLevel = System.Data.IsolationLevel;

namespace SmartSql.Test.Unit.Configuration;

public class StatementTests
{
    [Fact]
    public void Should_ReturnFullSqlId_When_ScopeAndIdSet()
    {
        var sqlMap = new SqlMap { Scope = "UserScope" };
        var statement = new Statement
        {
            Id = "GetById",
            SqlMap = sqlMap,
            SqlTags = new List<ITag>()
        };

        statement.FullSqlId.Should().Be("UserScope.GetById");
    }

    [Fact]
    public void Should_HaveDefaultStatementType_When_NotAnalyzed()
    {
        var statement = new Statement
        {
            Id = "Test",
            SqlMap = new SqlMap { Scope = "Scope" },
            SqlTags = new List<ITag>()
        };

        statement.StatementType.Should().Be(StatementType.Unknown);
    }

    [Fact]
    public void Should_SetCommandType_When_Provided()
    {
        var statement = new Statement
        {
            Id = "Test",
            SqlMap = new SqlMap { Scope = "Scope" },
            SqlTags = new List<ITag>(),
            CommandType = DataCommandType.StoredProcedure
        };

        statement.CommandType.Should().Be(DataCommandType.StoredProcedure);
    }

    [Fact]
    public void Should_SetSourceChoice_When_Provided()
    {
        var statement = new Statement
        {
            Id = "Test",
            SqlMap = new SqlMap { Scope = "Scope" },
            SqlTags = new List<ITag>(),
            SourceChoice = DataSourceChoice.Read
        };

        statement.SourceChoice.Should().Be(DataSourceChoice.Read);
    }

    [Fact]
    public void Should_SetTransaction_When_Provided()
    {
        var statement = new Statement
        {
            Id = "Test",
            SqlMap = new SqlMap { Scope = "Scope" },
            SqlTags = new List<ITag>(),
            Transaction = DataIsolationLevel.ReadCommitted
        };

        statement.Transaction.Should().Be(DataIsolationLevel.ReadCommitted);
    }

    [Fact]
    public void Should_SetCommandTimeout_When_Provided()
    {
        var statement = new Statement
        {
            Id = "Test",
            SqlMap = new SqlMap { Scope = "Scope" },
            SqlTags = new List<ITag>(),
            CommandTimeout = 30
        };

        statement.CommandTimeout.Should().Be(30);
    }

    [Fact]
    public void Should_HaveNullCommandTimeout_When_NotSet()
    {
        var statement = new Statement
        {
            Id = "Test",
            SqlMap = new SqlMap { Scope = "Scope" },
            SqlTags = new List<ITag>()
        };

        statement.CommandTimeout.Should().BeNull();
    }

    [Fact]
    public void Should_SetCacheId_When_Provided()
    {
        var statement = new Statement
        {
            Id = "Test",
            SqlMap = new SqlMap { Scope = "Scope" },
            SqlTags = new List<ITag>(),
            CacheId = "Scope.TestCache"
        };

        statement.CacheId.Should().Be("Scope.TestCache");
    }

    [Fact]
    public void Should_SetResultMapId_When_Provided()
    {
        var statement = new Statement
        {
            Id = "Test",
            SqlMap = new SqlMap { Scope = "Scope" },
            SqlTags = new List<ITag>(),
            ResultMapId = "Scope.TestResult"
        };

        statement.ResultMapId.Should().Be("Scope.TestResult");
    }

    [Fact]
    public void Should_SetParameterMapId_When_Provided()
    {
        var statement = new Statement
        {
            Id = "Test",
            SqlMap = new SqlMap { Scope = "Scope" },
            SqlTags = new List<ITag>(),
            ParameterMapId = "Scope.TestParam"
        };

        statement.ParameterMapId.Should().Be("Scope.TestParam");
    }

    [Fact]
    public void Should_SetMultipleResultMapId_When_Provided()
    {
        var statement = new Statement
        {
            Id = "Test",
            SqlMap = new SqlMap { Scope = "Scope" },
            SqlTags = new List<ITag>(),
            MultipleResultMapId = "Scope.TestMultiple"
        };

        statement.MultipleResultMapId.Should().Be("Scope.TestMultiple");
    }

    [Fact]
    public void Should_EnablePropertyChangedTrack_When_Set()
    {
        var statement = new Statement
        {
            Id = "Test",
            SqlMap = new SqlMap { Scope = "Scope" },
            SqlTags = new List<ITag>(),
            EnablePropertyChangedTrack = true
        };

        statement.EnablePropertyChangedTrack.Should().BeTrue();
    }
}
