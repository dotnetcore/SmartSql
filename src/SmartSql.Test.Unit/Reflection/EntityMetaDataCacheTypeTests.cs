using FluentAssertions;
using SmartSql.Reflection.TypeConstants;
using SmartSql.Test.Unit.TestEntities;
using Xunit;

namespace SmartSql.Test.Unit.Reflection;

public class EntityMetaDataCacheTypeTests
{
    [Fact]
    public void Should_ReturnTableName_When_EntityTypeHasTableAttribute()
    {
        var tableName = EntityMetaDataCacheType.GetTableName(typeof(AllPrimitive));

        tableName.Should().Be("T_AllPrimitive");
    }
}
