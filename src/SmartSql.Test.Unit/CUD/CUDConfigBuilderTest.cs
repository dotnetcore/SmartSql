using System;
using SmartSql.ConfigBuilder;
using SmartSql.Configuration;
using SmartSql.Configuration.Tags;
using SmartSql.CUD;
using SmartSql.Utils;
using Xunit;

namespace SmartSql.Test.Unit.CUD
{
    public class CUDConfigBuilderTest
    {
        [Fact]
        public void Build()
        {
            var entityTypeList = TypeScan.Scan(new TypeScanOptions
            {
                AssemblyString = "SmartSql.Test",
                Filter = type => type.FullName == "SmartSql.Test.Entities.WebMenu"
            });
            var configBuilder = new CUDConfigBuilder(new NativeConfigBuilder(new SmartSqlConfig()), entityTypeList);
            Assert.Throws<NullReferenceException>(() => { configBuilder.Build(); });
        }


        [Fact]
        public void BuildWhenParentIsXmlConfigBuilder()
        {
            var xmlConfigBuilder =
                new XmlConfigBuilder(ResourceType.File, SmartSqlBuilder.DEFAULT_SMARTSQL_CONFIG_PATH);
            var entityTypeList = TypeScan.Scan(new TypeScanOptions
            {
                AssemblyString = "SmartSql.Test",
                Filter = type => type.FullName == "SmartSql.Test.CUD.CudEntity"
            });
            var configBuilder = new CUDConfigBuilder(xmlConfigBuilder, entityTypeList);
            var smartSqlConfig = configBuilder.Build();
            Assert.True(smartSqlConfig.SqlMaps.TryGetValue("CudEntity", out SqlMap sqlMap));

            Assert.True(sqlMap.Statements.TryGetValue("CudEntity.Insert", out Statement insertStatement));
            Assert.Equal("Insert Into CudEntity (`Id`,`Name`) Values (?Id,?Name)",
                ((SqlText)insertStatement.SqlTags[0]).BodyText);

            Assert.True(
                sqlMap.Statements.TryGetValue("CudEntity.InsertReturnId", out Statement insertReturnIdStatement));
            Assert.Equal("Insert Into CudEntity (`Id`,`Name`) Values (?Id,?Name)",
                ((SqlText)insertReturnIdStatement.SqlTags[0]).BodyText);
            Assert.Equal(";Select Last_Insert_Id();",
                ((SqlText)insertReturnIdStatement.SqlTags[1]).BodyText);

            Assert.True(sqlMap.Statements.TryGetValue("CudEntity.Update", out Statement updateStatement));
            Assert.Equal("Update CudEntity",
                ((SqlText)updateStatement.SqlTags[0]).BodyText);
            //TODO Assert Child of Set
            Assert.Equal(typeof(Set), updateStatement.SqlTags[1].GetType());
            Assert.Equal(" Where `Id`=?Id",
                ((SqlText)updateStatement.SqlTags[2]).BodyText);

            Assert.True(sqlMap.Statements.TryGetValue("CudEntity.DeleteAll", out Statement deleteAllStatement));
            Assert.Equal("Delete From CudEntity",
                ((SqlText)deleteAllStatement.SqlTags[0]).BodyText);

            Assert.True(sqlMap.Statements.TryGetValue("CudEntity.DeleteById", out Statement deleteByIdStatement));
            Assert.Equal("Delete From CudEntity Where `Id`=?Id",
                ((SqlText)deleteByIdStatement.SqlTags[0]).BodyText);

            Assert.True(sqlMap.Statements.TryGetValue("CudEntity.DeleteMany", out Statement deleteManyStatement));
            Assert.Equal("Delete From CudEntity Where Id In ?Ids",
                ((SqlText)deleteManyStatement.SqlTags[0]).BodyText);

            Assert.True(sqlMap.Statements.TryGetValue("CudEntity.GetById", out Statement getByIdStatement));
            Assert.Equal("Select * From CudEntity Where `Id`=?Id",
                ((SqlText)getByIdStatement.SqlTags[0]).BodyText);
        }
    }
}