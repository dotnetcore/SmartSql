using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using FluentAssertions;
using SmartSql.Utils;
using Xunit;

namespace SmartSql.Test.Unit.Utils
{
    public class ResourceUtilTests
    {
        [Fact]
        public void Should_LoadXml_When_UriProvided()
        {
            var smartSqlConfig_github =
                "https://raw.githubusercontent.com/Smart-Kit/SmartSql/master/src/SmartSql.Test.Unit/SmartSqlMapConfig.xml";
            Uri uri = new Uri(smartSqlConfig_github);

            var xml = ResourceUtil.LoadUriAsXml(uri);

            xml.Should().NotBeNull();
        }

        [Fact]
        public void Should_LoadXml_When_EmbeddedResourceProvided()
        {
            var xml = ResourceUtil.LoadEmbeddedAsXml("SmartSql.Test.Unit.SmartSqlMapConfig-Embedded.xml,SmartSql.Test.Unit");

            xml.Should().NotBeNull();
        }

        [Fact]
        public void Should_LoadXml_When_FilePathProvided()
        {
            var xml = ResourceUtil.LoadFileAsXml("SmartSqlMapConfig.xml");

            xml.Should().NotBeNull();
        }
    }
}
