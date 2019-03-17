using SmartSql.DataSource;
using SmartSql.Test.Entities;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using Xunit;

namespace SmartSql.Test.Unit
{
    public class SmartSqlBuilderTest : AbstractTest
    {
        [Fact]
        public void Build_By_DataSource()
        {
            var dbSessionFactory = SmartSqlBuilder
                .AddDataSource(DbProvider.SQLSERVER, ConnectionString)
                .Build().GetDbSessionFactory();

            using (var dbSession = dbSessionFactory.Open())
            {

            }
            SmartSqlContainer.Instance.Dispose();
        }
        [Fact]
        public void Build_By_Config()
        {
            DbProviderManager.Instance.TryGet(DbProvider.SQLSERVER, out var dbProvider);
            var dbSessionFactory = SmartSqlBuilder
               .AddConfig(new Configuration.SmartSqlConfig
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
               .Build();
            SmartSqlContainer.Instance.Dispose();
        }
        [Fact]
        public void Build_By_Xml()
        {
            var dbSessionFactory = SmartSqlBuilder
               .AddXmlConfig()
               .Build();
            SmartSqlContainer.Instance.Dispose();
        }
        [Fact]
        public void Build_As_Mapper()
        {
            var sqlMapper = SmartSqlBuilder
               .AddXmlConfig()
               .Build()
               .GetSqlMapper();
            SmartSqlContainer.Instance.Dispose();
        }

    }


}
