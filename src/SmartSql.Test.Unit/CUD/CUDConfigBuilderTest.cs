using System;
using Microsoft.Extensions.Logging;
using SmartSql.ConfigBuilder;
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
                Filter = type => type.Namespace == "SmartSql.Test.Entities"
            });
            var configBuilder = new CUDConfigBuilder(entityTypeList);
            var smartSqlConfig = configBuilder.Build();
            Assert.NotNull(smartSqlConfig);
            var maps = smartSqlConfig.SqlMaps;
            foreach (var map in maps)
            {
                var scope = map.Key;
                var v = map.Value;
                foreach (var statement in v.Statements)
                {
                    Console.WriteLine($"class {scope}.{statement.Value.Id} found");
                }
                Assert.Equal(5, v.Statements.Count);
            }
        }

        [Fact]
        public void BuildWhenParentIsXmlConfigBuilder()
        {
            var xmlConfigBuilder =
                new XmlConfigBuilder(ResourceType.File, SmartSqlBuilder.DEFAULT_SMARTSQL_CONFIG_PATH);
            var entityTypeList = TypeScan.Scan(new TypeScanOptions
            {
                AssemblyString = "SmartSql.Test",
                Filter = type => type.FullName == "SmartSql.Test.Entities.WebMenu"
            });
            var configBuilder = new CUDConfigBuilder(xmlConfigBuilder, entityTypeList);
            var smartSqlConfig = configBuilder.Build();
            Assert.NotNull(smartSqlConfig);
            //TODO check CUD Statement
            var maps = smartSqlConfig.SqlMaps;
            foreach (var map in maps)
            {
                if (map.Key != "WebMenu")
                    continue;
                var scope = map.Key;
                var v = map.Value;
                foreach (var statement in v.Statements)
                {
                    Console.WriteLine($"class {scope}.{statement.Value.Id} found");
                }
                Assert.Equal(5, v.Statements.Count);
            }
        }
    }
}