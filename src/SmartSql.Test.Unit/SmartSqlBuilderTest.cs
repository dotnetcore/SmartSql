using SmartSql.DataSource;
using SmartSql.Test.Entities;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using SmartSql.TypeHandler;
using Xunit;

namespace SmartSql.Test.Unit
{
    public class SmartSqlBuilderTest : AbstractTest
    {
        [Fact]
        public void Build_By_DataSource()
        {
            var dbSessionFactory = new SmartSqlBuilder()
                .UseOracleCommandExecuter()
                .UseDataSource(DbProvider.SQLSERVER, ConnectionString)
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
        public void Build_By_Config()
        {
            DbProviderManager.Instance.TryGet(DbProvider.SQLSERVER, out var dbProvider);
            var dbSessionFactory = new SmartSqlBuilder()
                .UseNativeConfig(new Configuration.SmartSqlConfig
                {
                    Database = new Database
                    {
                        DbProvider = dbProvider,
                        Write = new WriteDataSource
                        {
                            Name = "Write",
                            ConnectionString = ConnectionString,
                            DbProvider = dbProvider
                        },
                        Reads = new Dictionary<String, ReadDataSource>()
                    }
                })
                .UseAlias("Build_By_Config")
                .Build();
        }

        [Fact]
        public void Build_By_Xml()
        {
            var dbSessionFactory = new SmartSqlBuilder()
                .UseXmlConfig()
                .UseAlias("Build_By_Xml")
                .Build();
        }

        [Fact]
        public void Build_As_Mapper()
        {
            var sqlMapper = new SmartSqlBuilder()
                .UseXmlConfig()
                .UseAlias("Build_As_Mapper")
                .Build()
                .GetSqlMapper();
        }
    }
}