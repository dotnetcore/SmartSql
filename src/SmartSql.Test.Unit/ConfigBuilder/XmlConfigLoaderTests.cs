using FluentAssertions;
using SmartSql.ConfigBuilder;
using Xunit;

namespace SmartSql.Test.Unit.ConfigBuilder;

public class XmlConfigLoaderTests
{
    [Fact]
    public void Should_LoadConfig_When_ValidXmlFile()
    {
        var configLoader = new XmlConfigBuilder(ResourceType.File, "SmartSqlMapConfig-UnitTest.xml");

        var config = configLoader.Build();

        config.Should().NotBeNull();
    }
}
