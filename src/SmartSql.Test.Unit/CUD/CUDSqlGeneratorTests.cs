using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using FluentAssertions;
using SmartSql.Annotations;
using SmartSql.Configuration;
using SmartSql.Configuration.Tags;
using SmartSql.CUD;
using SmartSql.DataSource;
using SmartSql.Test.Unit.TestEntities;
using SmartSql.Utils;
using Xunit;
using CacheConfig = SmartSql.Configuration.Cache;

namespace SmartSql.Test.Unit.CUD;

public class CUDSqlGeneratorTests
{
    private readonly CUDSqlGenerator _generator;
    private readonly SmartSqlConfig _config;

    public CUDSqlGeneratorTests()
    {
        _config = CreateTestConfig(DbProviderManager.MYSQL_DBPROVIDER);
        _generator = new CUDSqlGenerator(_config);
    }

    private static SmartSqlConfig CreateTestConfig(DbProvider dbProvider)
    {
        return new SmartSqlConfig
        {
            Database = new Database
            {
                DbProvider = dbProvider
            }
        };
    }

    private static SqlMap CreateTestSqlMap(string scope)
    {
        return new SqlMap
        {
            Scope = scope,
            Statements = new Dictionary<string, Statement>(),
            Caches = new Dictionary<string, CacheConfig>(),
            MultipleResultMaps = new Dictionary<string, MultipleResultMap>(),
            ParameterMaps = new Dictionary<string, ParameterMap>(),
            ResultMaps = new Dictionary<string, ResultMap>()
        };
    }

    #region Generate

    [Fact]
    public void Should_GenerateAllStatements_When_EntityHasAllCUDOperations()
    {
        // Arrange
        var sqlMap = CreateTestSqlMap("CUD_AllPrimitive");

        // Act
        var result = _generator.Generate(sqlMap, typeof(AllPrimitive));

        // Assert
        result.Should().ContainKey("CUD_AllPrimitive.GetById");
        result.Should().ContainKey("CUD_AllPrimitive.Insert");
        result.Should().ContainKey("CUD_AllPrimitive.InsertReturnId");
        result.Should().ContainKey("CUD_AllPrimitive.Update");
        result.Should().ContainKey("CUD_AllPrimitive.DeleteById");
        result.Should().ContainKey("CUD_AllPrimitive.DeleteAll");
        result.Should().ContainKey("CUD_AllPrimitive.DeleteMany");
        result.Count.Should().Be(7);
    }

    [Fact]
    public void Should_GenerateStatementsWithCorrectStatementIds_When_CalledForAllPrimitive()
    {
        // Arrange
        var sqlMap = CreateTestSqlMap("CUD_AllPrimitive");

        // Act
        var result = _generator.Generate(sqlMap, typeof(AllPrimitive));

        // Assert - individual Statement.Id should match the CUD statement names
        result["CUD_AllPrimitive.GetById"].Id.Should().Be(CUDStatementName.GET_BY_ID);
        result["CUD_AllPrimitive.Insert"].Id.Should().Be(CUDStatementName.INSERT);
        result["CUD_AllPrimitive.Update"].Id.Should().Be(CUDStatementName.UPDATE);
        result["CUD_AllPrimitive.DeleteById"].Id.Should().Be(CUDStatementName.DELETE_BY_ID);
        result["CUD_AllPrimitive.DeleteAll"].Id.Should().Be(CUDStatementName.DELETE_ALL);
        result["CUD_AllPrimitive.DeleteMany"].Id.Should().Be(CUDStatementName.DELETE_MANY);
    }

    #endregion

    #region BuildGetEntity

    [Fact]
    public void Should_GenerateSelectByIdSql_When_CalledWithEntity()
    {
        // Arrange
        var sqlMap = CreateTestSqlMap("CUD_AllPrimitive");
        var gParams = new GeneratorParams(sqlMap, typeof(AllPrimitive));

        // Act
        var statement = _generator.BuildGetEntity(gParams);

        // Assert
        statement.Id.Should().Be(CUDStatementName.GET_BY_ID);
        statement.CommandType.Should().Be(CommandType.Text);
        statement.SqlTags.Should().ContainSingle();
        var sqlText = statement.SqlTags.OfType<SqlText>().First();
        sqlText.BodyText.Should().StartWith("Select * From");
        sqlText.BodyText.Should().Contain("Where");
    }

    [Fact]
    public void Should_GenerateGetByIdWithMySqlPrefix_When_UsingMySqlProvider()
    {
        // Arrange
        var sqlMap = CreateTestSqlMap("CUD_AllPrimitive");
        var gParams = new GeneratorParams(sqlMap, typeof(AllPrimitive));

        // Act
        var statement = _generator.BuildGetEntity(gParams);
        var sqlText = statement.SqlTags.OfType<SqlText>().First();

        // Assert - MySQL uses ? as parameter prefix
        sqlText.BodyText.Should().Contain("?Id");
    }

    [Fact]
    public void Should_GenerateGetByIdWithSqlServerPrefix_When_UsingSqlServerProvider()
    {
        // Arrange
        var config = CreateTestConfig(DbProviderManager.SQLSERVER_DBPROVIDER);
        var generator = new CUDSqlGenerator(config);
        var sqlMap = CreateTestSqlMap("CUD_AllPrimitive");
        var gParams = new GeneratorParams(sqlMap, typeof(AllPrimitive));

        // Act
        var statement = generator.BuildGetEntity(gParams);
        var sqlText = statement.SqlTags.OfType<SqlText>().First();

        // Assert - SQL Server uses @ as parameter prefix
        sqlText.BodyText.Should().Contain("@Id");
    }

    #endregion

    #region BuildInsert

    [Fact]
    public void Should_GenerateInsertSql_When_CalledWithEntity()
    {
        // Arrange
        var sqlMap = CreateTestSqlMap("CUD_AllPrimitive");
        var gParams = new GeneratorParams(sqlMap, typeof(AllPrimitive));

        // Act
        var statement = _generator.BuildInsert(gParams);

        // Assert
        statement.Id.Should().Be(CUDStatementName.INSERT);
        var sqlText = statement.SqlTags.OfType<SqlText>().First();
        sqlText.BodyText.Should().StartWith("Insert Into");
        sqlText.BodyText.Should().Contain("Values");
    }

    [Fact]
    public void Should_SkipAutoIncrementColumn_When_GeneratingInsertSql()
    {
        // Arrange
        var sqlMap = CreateTestSqlMap("CUD_AllPrimitive");
        var gParams = new GeneratorParams(sqlMap, typeof(AllPrimitive));

        // Act
        var statement = _generator.BuildInsert(gParams);
        var sqlText = statement.SqlTags.OfType<SqlText>().First();

        // Assert - AllPrimitive.Id is IsAutoIncrement=true, so it should not appear in insert values
        sqlText.BodyText.Should().NotContain("?Id");
        sqlText.BodyText.Should().Contain("?Boolean");
    }

    [Fact]
    public void Should_IncludeAllNonAutoIncrementColumns_When_GeneratingInsertSql()
    {
        // Arrange
        var sqlMap = CreateTestSqlMap("CUD_AllPrimitive");
        var gParams = new GeneratorParams(sqlMap, typeof(AllPrimitive));

        // Act
        var statement = _generator.BuildInsert(gParams);
        var sqlText = statement.SqlTags.OfType<SqlText>().First();

        // Assert
        sqlText.BodyText.Should().Contain("?Boolean");
        sqlText.BodyText.Should().Contain("?String");
    }

    #endregion

    #region BuildInsertReturnId

    [Fact]
    public void Should_AppendSelectAutoIncrement_When_NotPostgreSql()
    {
        // Arrange
        var sqlMap = CreateTestSqlMap("CUD_AllPrimitive");
        var gParams = new GeneratorParams(sqlMap, typeof(AllPrimitive));

        // Act
        var statement = _generator.BuildInsertReturnId(gParams);

        // Assert - BuildInsertReturnId returns a Statement whose Id was set by BuildInsert
        statement.Id.Should().Be(CUDStatementName.INSERT);
        statement.SqlTags.Should().HaveCount(2);
        var lastTag = statement.SqlTags.OfType<SqlText>().Last();
        lastTag.BodyText.Should().Contain("Last_Insert_Id");
    }

    [Fact]
    public void Should_AppendReturningClause_When_PostgreSqlProvider()
    {
        // Arrange
        var config = CreateTestConfig(DbProviderManager.POSTGRESQL_DBPROVIDER);
        var generator = new CUDSqlGenerator(config);
        var sqlMap = CreateTestSqlMap("CUD_AllPrimitive");
        var gParams = new GeneratorParams(sqlMap, typeof(AllPrimitive));

        // Act
        var statement = generator.BuildInsertReturnId(gParams);

        // Assert
        statement.SqlTags.Should().HaveCount(2);
        var lastTag = statement.SqlTags.OfType<SqlText>().Last();
        lastTag.BodyText.Should().Contain("Returning");
    }

    #endregion

    #region BuildUpdate

    [Fact]
    public void Should_GenerateUpdateSql_When_CalledWithEntity()
    {
        // Arrange
        var sqlMap = CreateTestSqlMap("CUD_AllPrimitive");
        var gParams = new GeneratorParams(sqlMap, typeof(AllPrimitive));

        // Act
        var statement = _generator.BuildUpdate(gParams);

        // Assert
        statement.Id.Should().Be(CUDStatementName.UPDATE);
        statement.SqlTags.Should().HaveCount(3);
    }

    [Fact]
    public void Should_UseSetTag_When_GeneratingUpdateSql()
    {
        // Arrange
        var sqlMap = CreateTestSqlMap("CUD_AllPrimitive");
        var gParams = new GeneratorParams(sqlMap, typeof(AllPrimitive));

        // Act
        var statement = _generator.BuildUpdate(gParams);

        // Assert
        statement.SqlTags[1].Should().BeOfType<Set>();
    }

    [Fact]
    public void Should_ExcludePrimaryKeyFromSetClause_When_GeneratingUpdateSql()
    {
        // Arrange
        var sqlMap = CreateTestSqlMap("CUD_AllPrimitive");
        var gParams = new GeneratorParams(sqlMap, typeof(AllPrimitive));

        // Act
        var statement = _generator.BuildUpdate(gParams);
        var setTag = statement.SqlTags.OfType<Set>().First();

        // Assert - Set tag children should not include the primary key column
        var isProperties = setTag.ChildTags.OfType<IsProperty>().ToList();
        isProperties.Should().NotContain(p => p.Property == "Id");
    }

    [Fact]
    public void Should_UseIsPropertyTags_When_GeneratingUpdateSql()
    {
        // Arrange
        var sqlMap = CreateTestSqlMap("CUD_AllPrimitive");
        var gParams = new GeneratorParams(sqlMap, typeof(AllPrimitive));

        // Act
        var statement = _generator.BuildUpdate(gParams);
        var setTag = statement.SqlTags.OfType<Set>().First();

        // Assert
        var isProperties = setTag.ChildTags.OfType<IsProperty>().ToList();
        isProperties.Should().OnlyContain(p => p.Property != "Id");
        isProperties.Should().NotBeEmpty();
    }

    [Fact]
    public void Should_IncludeWhereClause_When_GeneratingUpdateSql()
    {
        // Arrange
        var sqlMap = CreateTestSqlMap("CUD_AllPrimitive");
        var gParams = new GeneratorParams(sqlMap, typeof(AllPrimitive));

        // Act
        var statement = _generator.BuildUpdate(gParams);
        var lastTag = statement.SqlTags.OfType<SqlText>().Last();

        // Assert
        lastTag.BodyText.Should().Contain("Where");
    }

    #endregion

    #region BuildDeleteById

    [Fact]
    public void Should_GenerateDeleteByIdSql_When_CalledWithEntity()
    {
        // Arrange
        var sqlMap = CreateTestSqlMap("CUD_AllPrimitive");
        var gParams = new GeneratorParams(sqlMap, typeof(AllPrimitive));

        // Act
        var statement = _generator.BuildDeleteById(gParams);

        // Assert
        statement.Id.Should().Be(CUDStatementName.DELETE_BY_ID);
        var sqlText = statement.SqlTags.OfType<SqlText>().First();
        sqlText.BodyText.Should().StartWith("Delete From");
        sqlText.BodyText.Should().Contain("Where");
    }

    #endregion

    #region BuildDeleteAll

    [Fact]
    public void Should_GenerateDeleteAllSql_When_CalledWithEntity()
    {
        // Arrange
        var sqlMap = CreateTestSqlMap("CUD_AllPrimitive");
        var gParams = new GeneratorParams(sqlMap, typeof(AllPrimitive));

        // Act
        var statement = _generator.BuildDeleteAll(gParams);

        // Assert
        statement.Id.Should().Be(CUDStatementName.DELETE_ALL);
        var sqlText = statement.SqlTags.OfType<SqlText>().First();
        sqlText.BodyText.Should().StartWith("Delete From");
        sqlText.BodyText.Should().NotContain("Where");
    }

    #endregion

    #region BuildDeleteMany

    [Fact]
    public void Should_GenerateDeleteManySql_When_CalledWithEntity()
    {
        // Arrange
        var sqlMap = CreateTestSqlMap("CUD_AllPrimitive");
        var gParams = new GeneratorParams(sqlMap, typeof(AllPrimitive));

        // Act
        var statement = _generator.BuildDeleteMany(gParams);

        // Assert
        statement.Id.Should().Be(CUDStatementName.DELETE_MANY);
        var sqlText = statement.SqlTags.OfType<SqlText>().First();
        sqlText.BodyText.Should().StartWith("Delete From");
        sqlText.BodyText.Should().Contain("In");
    }

    [Fact]
    public void Should_UsePluralParameterName_When_GeneratingDeleteManySql()
    {
        // Arrange
        var sqlMap = CreateTestSqlMap("CUD_AllPrimitive");
        var gParams = new GeneratorParams(sqlMap, typeof(AllPrimitive));

        // Act
        var statement = _generator.BuildDeleteMany(gParams);
        var sqlText = statement.SqlTags.OfType<SqlText>().First();

        // Assert - MySQL uses ? prefix, pk column name is "Id", so "Ids"
        sqlText.BodyText.Should().Contain("?Ids");
    }

    #endregion
}
