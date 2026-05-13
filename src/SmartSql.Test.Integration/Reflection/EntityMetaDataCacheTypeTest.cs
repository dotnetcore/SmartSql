using SmartSql.Reflection.TypeConstants;
using SmartSql.Test.Entities;
using Xunit;

namespace SmartSql.Test.Integration.Reflection;

public class EntityMetaDataCacheTypeTest
{
    [Fact]
    public void GetTableName()
    {
        var tableName = EntityMetaDataCacheType.GetTableName(typeof(AllPrimitive));
        Assert.Equal("T_AllPrimitive", tableName);
    }
}