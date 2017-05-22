using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using SmartSql.ZooKeeperConfig;
using SmartSql;
namespace SmartSql.Tests.ZooKeeperConfig
{
    public class ZooKeeperConfigLoader_Tests 
    {


        [Fact]
        public void Load()
        {
            string connStr = "192.168.31.103:2181";
            var configLoader = new ZooKeeperConfigLoader(connStr);
            string configPath = "/Config/App1/SmartSqlMapConfig.xml";
            var mapper = new SmartSqlMapper(configPath, configLoader);

            Assert.NotNull(mapper);

        }
    }
}
