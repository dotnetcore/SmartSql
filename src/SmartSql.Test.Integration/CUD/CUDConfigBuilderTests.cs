using System;
using FluentAssertions;
using SmartSql.ConfigBuilder;
using SmartSql.Configuration;
using SmartSql.Configuration.Tags;
using SmartSql.CUD;
using SmartSql.Utils;
using Xunit;

namespace SmartSql.Test.Integration.CUD;

public class CUDConfigBuilderTests : IntegrationTestBase
{
    public CUDConfigBuilderTests(SmartSqlFixture fixture) : base(fixture) { }

    [Fact]
    public void Should_Throw_When_ParentIsNativeConfigBuilder()
    {
        var entityTypeList = TypeScan.Scan(new TypeScanOptions
        {
            AssemblyString = "SmartSql.Test",
            Filter = type => type.FullName == "SmartSql.Test.Entities.WebMenu"
        });
        var configBuilder = new CUDConfigBuilder(new NativeConfigBuilder(new SmartSqlConfig()), entityTypeList);
        var act = () => configBuilder.Build();
        act.Should().Throw<NullReferenceException>();
    }

    [Fact]
    public void Should_GenerateAllStatements_When_ParentIsXmlConfigBuilder()
    {
        var xmlConfigBuilder = new XmlConfigBuilder(ResourceType.File, SmartSqlBuilder.DEFAULT_SMARTSQL_CONFIG_PATH);
        var entityTypeList = TypeScan.Scan(new TypeScanOptions
        {
            AssemblyString = "SmartSql.Test",
            Filter = type => type.FullName == "SmartSql.Test.CUD.CudEntity"
        });
        var configBuilder = new CUDConfigBuilder(xmlConfigBuilder, entityTypeList);
        var config = configBuilder.Build();

        config.SqlMaps.Should().ContainKey("CudEntity");
        var sqlMap = config.SqlMaps["CudEntity"];

        // Insert
        sqlMap.Statements.Should().ContainKey("CudEntity.Insert");
        var insertSql = (SqlText)sqlMap.Statements["CudEntity.Insert"].SqlTags[0];
        insertSql.BodyText.Should().Be("Insert Into CudEntity (`Id`,`Name`) Values (?Id,?Name)");

        // InsertReturnId
        sqlMap.Statements.Should().ContainKey("CudEntity.InsertReturnId");
        var insertReturnId = sqlMap.Statements["CudEntity.InsertReturnId"];
        ((SqlText)insertReturnId.SqlTags[0]).BodyText.Should().Be("Insert Into CudEntity (`Id`,`Name`) Values (?Id,?Name)");
        ((SqlText)insertReturnId.SqlTags[1]).BodyText.Should().Be(";Select Last_Insert_Id();");

        // Update
        sqlMap.Statements.Should().ContainKey("CudEntity.Update");
        var update = sqlMap.Statements["CudEntity.Update"];
        ((SqlText)update.SqlTags[0]).BodyText.Should().Be("Update CudEntity");
        update.SqlTags[1].Should().BeOfType<Set>();
        ((SqlText)update.SqlTags[2]).BodyText.Should().Be(" Where `Id`=?Id");

        // DeleteAll
        sqlMap.Statements.Should().ContainKey("CudEntity.DeleteAll");
        ((SqlText)sqlMap.Statements["CudEntity.DeleteAll"].SqlTags[0]).BodyText.Should().Be("Delete From CudEntity");

        // DeleteById
        sqlMap.Statements.Should().ContainKey("CudEntity.DeleteById");
        ((SqlText)sqlMap.Statements["CudEntity.DeleteById"].SqlTags[0]).BodyText.Should().Be("Delete From CudEntity Where `Id`=?Id");

        // DeleteMany
        sqlMap.Statements.Should().ContainKey("CudEntity.DeleteMany");
        ((SqlText)sqlMap.Statements["CudEntity.DeleteMany"].SqlTags[0]).BodyText.Should().Be("Delete From CudEntity Where Id In ?Ids");

        // GetById
        sqlMap.Statements.Should().ContainKey("CudEntity.GetById");
        ((SqlText)sqlMap.Statements["CudEntity.GetById"].SqlTags[0]).BodyText.Should().Be("Select * From CudEntity Where `Id`=?Id");
    }
}
