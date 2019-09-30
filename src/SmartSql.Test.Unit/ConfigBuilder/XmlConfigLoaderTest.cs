using System;
using System.Collections.Generic;
using System.Text;
using SmartSql.ConfigBuilder;
using Xunit;

namespace SmartSql.Test.Unit.ConfigBuilder
{
    public class XmlConfigLoaderTest
    {
        [Fact]
        public void Load()
        {
            var configLoader = new XmlConfigBuilder(ResourceType.File, "SmartSqlMapConfig.xml");
            var config = configLoader.Build();
        }
    }
}
