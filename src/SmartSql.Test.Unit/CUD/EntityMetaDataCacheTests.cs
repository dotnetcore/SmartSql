using System;
using System.Collections.Generic;
using System.Reflection;
using FluentAssertions;
using SmartSql.Annotations;
using SmartSql.CUD;
using SmartSql.Exceptions;
using SmartSql.Test.Unit.TestEntities;
using Xunit;

namespace SmartSql.Test.Unit.CUD;

public class EntityMetaDataCacheTests
{
    [Fact]
    public void Should_PopulateTableName_When_EntityHasTableAttribute()
    {
        // Arrange & Act
        var metaData = EntityMetaDataCache<AllPrimitive>.MetaData;

        // Assert
        metaData.TableName.Should().Be("T_AllPrimitive");
    }

    [Fact]
    public void Should_PopulateScope_When_EntityHasScopeAttribute()
    {
        // Arrange & Act
        var metaData = EntityMetaDataCache<AllPrimitive>.MetaData;

        // Assert
        metaData.Scope.Should().Be("CUD_AllPrimitive");
    }

    [Fact]
    public void Should_PopulateTableNameFromClassName_When_EntityHasNoTableAttribute()
    {
        // Arrange & Act
        var metaData = EntityMetaDataCache<Entity>.MetaData;

        // Assert
        metaData.TableName.Should().Be("Entity");
    }

    [Fact]
    public void Should_PopulateScopeFromTableName_When_EntityHasNoScopeAttribute()
    {
        // Arrange & Act
        var metaData = EntityMetaDataCache<Entity>.MetaData;

        // Assert
        metaData.Scope.Should().Be("Entity");
    }

    [Fact]
    public void Should_SetPrimaryKey_When_PropertyMarkedAsPrimaryKey()
    {
        // Arrange & Act
        var metaData = EntityMetaDataCache<AllPrimitive>.MetaData;

        // Assert
        metaData.PrimaryKey.Should().NotBeNull();
        metaData.PrimaryKey.IsPrimaryKey.Should().BeTrue();
        metaData.PrimaryKey.Name.Should().Be("Id");
    }

    [Fact]
    public void Should_NotSetPrimaryKey_When_ColumnAttributeLacksIsPrimaryKey()
    {
        // Arrange & Act
        // User has [Column("id")] on Id but does not set IsPrimaryKey=true.
        // The naming convention only applies when no ColumnAttribute is present.
        var metaData = EntityMetaDataCache<User>.MetaData;

        // Assert
        metaData.PrimaryKey.Should().BeNull();
    }

    [Fact]
    public void Should_DetectPrimaryKeyByNameConvention_When_NoColumnAttributeOnId()
    {
        // Arrange & Act
        // Entity.Id has no ColumnAttribute, so naming convention applies (property name == "Id")
        var metaData = EntityMetaDataCache<Entity>.MetaData;

        // Assert
        metaData.PrimaryKey.Should().NotBeNull();
        metaData.PrimaryKey.IsPrimaryKey.Should().BeTrue();
    }

    [Fact]
    public void Should_PopulateColumns_When_EntityHasProperties()
    {
        // Arrange & Act
        var columns = EntityMetaDataCache<AllPrimitive>.MetaData.Columns;

        // Assert
        columns.Should().NotBeEmpty();
        columns.Should().ContainKey("Id");
        columns.Should().ContainKey("Boolean");
        columns.Should().ContainKey("String");
    }

    [Fact]
    public void Should_PopulateColumnPropertyInfo_When_EntityHasProperties()
    {
        // Arrange & Act
        var columns = EntityMetaDataCache<User>.MetaData.Columns;

        // Assert
        columns["Id"].Property.Should().NotBeNull();
        columns["Id"].Property.Name.Should().Be("Id");
    }

    [Fact]
    public void Should_SetColumnFieldType_When_PropertyHasType()
    {
        // Arrange & Act
        var columns = EntityMetaDataCache<AllPrimitive>.MetaData.Columns;

        // Assert
        columns["Boolean"].FieldType.Should().Be(typeof(bool));
        columns["String"].FieldType.Should().Be(typeof(string));
    }

    [Fact]
    public void Should_UnwrapNullableType_When_PropertyIsNullable()
    {
        // Arrange & Act
        var columns = EntityMetaDataCache<AllPrimitive>.MetaData.Columns;

        // Assert
        columns["NullableBoolean"].FieldType.Should().Be(typeof(bool));
        columns["NullableInt32"].FieldType.Should().Be(typeof(int));
    }

    [Fact]
    public void Should_SetOrdinal_When_ProcessingProperties()
    {
        // Arrange & Act
        var columns = EntityMetaDataCache<User>.MetaData.Columns;

        // Assert
        foreach (var col in columns.Values)
        {
            col.Ordinal.Should().HaveValue();
        }
    }

    [Fact]
    public void Should_SetAutoIncrement_When_PropertyMarkedAsAutoIncrement()
    {
        // Arrange & Act
        var columns = EntityMetaDataCache<AllPrimitive>.MetaData.Columns;

        // Assert
        columns["Id"].IsAutoIncrement.Should().BeTrue();
    }

    [Fact]
    public void Should_PopulateIndexColumnMaps_When_ColumnsInitialized()
    {
        // Arrange & Act
        var indexMaps = EntityMetaDataCache<User>.IndexColumnMaps;

        // Assert
        indexMaps.Should().NotBeEmpty();
    }

    [Fact]
    public void Should_PopulateEntityType_When_CacheInitialized()
    {
        // Arrange & Act
        var entityType = EntityMetaDataCache<AllPrimitive>.EntityType;

        // Assert
        entityType.Should().Be(typeof(AllPrimitive));
    }

    [Fact]
    public void Should_ReturnColumnByPropertyName_When_PropertyExists()
    {
        // Arrange & Act
        var found = EntityMetaDataCache<User>.TryGetColumnByPropertyName("UserName", out var col);

        // Assert
        found.Should().BeTrue();
        col.Name.Should().Be("user_name");
    }

    [Fact]
    public void Should_ReturnFalse_When_PropertyNameNotFound()
    {
        // Arrange & Act
        var found = EntityMetaDataCache<User>.TryGetColumnByPropertyName("NonExistent", out var col);

        // Assert
        found.Should().BeFalse();
        col.Should().BeNull();
    }

    [Fact]
    public void Should_ReturnColumnByColumnName_When_ColumnExists()
    {
        // Arrange & Act
        var found = EntityMetaDataCache<User>.TryGetColumnByColumnName("user_name", out var col);

        // Assert
        found.Should().BeTrue();
        col.Property.Name.Should().Be("UserName");
    }

    [Fact]
    public void Should_ReturnFalse_When_ColumnNameNotFound()
    {
        // Arrange & Act
        var found = EntityMetaDataCache<User>.TryGetColumnByColumnName("nonexistent", out var col);

        // Assert
        found.Should().BeFalse();
        col.Should().BeNull();
    }

    [Fact]
    public void Should_ReturnPrimaryKey_When_EntityHasPrimaryKey()
    {
        // Arrange & Act
        var pk = EntityMetaDataCache<AllPrimitive>.PrimaryKey;

        // Assert
        pk.Should().NotBeNull();
        pk.IsPrimaryKey.Should().BeTrue();
    }

    [Fact]
    public void Should_Throw_When_EntityHasNoPrimaryKey()
    {
        // Arrange
        Action act = () =>
        {
            var _ = EntityMetaDataCache<NoPkEntity>.PrimaryKey;
        };

        // Act & Assert
        act.Should().Throw<SmartSqlException>()
            .WithMessage("*can not find PrimaryKey*");
    }

    [Fact]
    public void Should_SetGetPropertyValue_When_PropertyHasPublicGetter()
    {
        // Arrange & Act
        var columns = EntityMetaDataCache<User>.MetaData.Columns;

        // Assert
        columns["Id"].GetPropertyValue.Should().NotBeNull();
        columns["UserName"].GetPropertyValue.Should().NotBeNull();
    }

    /// <summary>
    /// Test entity without a primary key to verify exception behavior.
    /// </summary>
    [Table("T_NoPk")]
    private class NoPkEntity
    {
        public string Name { get; set; }
    }
}
