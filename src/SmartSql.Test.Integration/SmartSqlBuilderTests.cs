using System;
using System.Collections.Generic;
using FluentAssertions;
using SmartSql.DataSource;
using SmartSql.TypeHandler;
using Xunit;

namespace SmartSql.Test.Integration;

public class SmartSqlBuilderTests : IntegrationTestBase
{
    public SmartSqlBuilderTests(SmartSqlFixture fixture) : base(fixture) { }

    [Fact]
    public void Should_BuildSession_When_UsingDataSource()
    {
        var dbSessionFactory = new SmartSqlBuilder()
            .UseOracleCommandExecuter()
            .UseDataSource(DbProvider.MYSQL, "server=localhost;uid=root;pwd=root;database=SmartSqlTestDB")
            .UseAlias("Build_By_DataSource")
            .AddTypeHandler(new Configuration.TypeHandler
            {
                Name = "Json",
                HandlerType = typeof(JsonTypeHandler)
            })
            .Build().GetDbSessionFactory();

        using (var dbSession = dbSessionFactory.Open())
        {
        }
    }

    [Fact]
    public void Should_BuildSession_When_UsingNativeConfig()
    {
        DbProviderManager.Instance.TryGet(DbProvider.MYSQL, out var dbProvider);
        var dbSessionFactory = new SmartSqlBuilder()
            .UseNativeConfig(new Configuration.SmartSqlConfig
            {
                Database = new Database
                {
                    DbProvider = dbProvider,
                    Write = new WriteDataSource
                    {
                        Name = "Write",
                        ConnectionString = "server=localhost;uid=root;pwd=root;database=SmartSqlTestDB",
                        DbProvider = dbProvider
                    },
                    Reads = new Dictionary<String, ReadDataSource>()
                }
            })
            .UseAlias("Build_By_Config")
            .Build();
    }

    [Fact]
    public void Should_BuildSession_When_UsingXmlConfig()
    {
        new SmartSqlBuilder()
            .UseDataSource(DbProvider.MYSQL, "server=localhost;uid=root;pwd=root;database=SmartSqlTestDB")
            .UseAlias("Build_By_Xml")
            .Build();
    }

    [Fact]
    public void Should_ReturnSqlMapper_When_BuildingAsMapper()
    {
        new SmartSqlBuilder()
            .UseDataSource(DbProvider.MYSQL, "server=localhost;uid=root;pwd=root;database=SmartSqlTestDB")
            .UseAlias("Build_As_Mapper")
            .Build()
            .GetSqlMapper();
    }
}
