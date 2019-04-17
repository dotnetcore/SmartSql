using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using SmartSql.Utils;
using Xunit;

namespace SmartSql.Test.Unit.Utils
{
    public class ResourceUtilTest
    {
        [Fact]
        public void LoadUriAsXml()
        {
            var smartSqlConfig_github =
                "https://raw.githubusercontent.com/Smart-Kit/SmartSql/master/src/SmartSql.Test.Unit/SmartSqlMapConfig.xml";
            Uri uri=new Uri(smartSqlConfig_github);
            var xml = ResourceUtil.LoadUriAsXml(uri);
            Assert.NotNull(xml);
        }
        [Fact]
        public void LoadEmbeddedAsXml()
        {
            var xml = ResourceUtil.LoadEmbeddedAsXml("SmartSql.Test.Unit.SmartSqlMapConfig-Embedded.xml,SmartSql.Test.Unit");
            Assert.NotNull(xml);
        }
        [Fact]
        public void LoadFileAsXml()
        {
            var xml = ResourceUtil.LoadFileAsXml("SmartSqlMapConfig.xml");
            Assert.NotNull(xml);
        }
    }
}
