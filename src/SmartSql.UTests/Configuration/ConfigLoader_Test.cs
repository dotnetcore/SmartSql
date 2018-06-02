using SmartSql.Abstractions.Config;
using SmartSql.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace SmartSql.UTests.Configuration
{
    public class ConfigLoader_Test : TestBase
    {
        IConfigLoader _configLoader;
        public ConfigLoader_Test()
        {
            _configLoader = new LocalFileConfigLoader("SmartSqlMapConfig.xml", LoggerFactory);
        }

        [Fact]
        public void Load()
        {
            var config = _configLoader.Load();

            Assert.NotNull(config);
        }
    }
}
