using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using SmartSql.ZooKeeperConfig;
using SmartSql;
using SmartSql.Abstractions;
using System.Threading;

namespace SmartSql.Tests.ZooKeeperConfig
{
    public class ZooKeeperConfigLoader_Tests : IDisposable
    {
        protected ISmartSqlMapper SqlMapper { get; set; }

        public ZooKeeperConfigLoader_Tests()
        {
            string connStr = "192.168.31.103:2181";//192.168.1.5:2181,192.168.1.5:2182,192.168.1.5:2183 192.168.31.103:2181
            var configLoader = new ZooKeeperConfigLoader(connStr);
            string configPath = "/Config/App1/SmartSqlMapConfig.xml";
            SqlMapper = new SmartSqlMapper(configPath, configLoader);
        }

        [Fact]
        public void Query_OnChangeConfig()
        {
            int i = 0;
            for (i = 0; i < 10; i++)
            {
                var list = SqlMapper.Query<T_Test>(new RequestContext
                {
                    Scope = "T_Test",
                    SqlId = "GetList",
                    Request = new { Ids = new long[] { 1, 2, 3, 4 } }
                });
                Thread.Sleep(50);
            }
        }

        public void Dispose()
        {
            SqlMapper.Dispose();
        }
    }
}
