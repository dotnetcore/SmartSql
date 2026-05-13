using FluentAssertions;
using SmartSql.TypeHandler;
using SmartSql.TypeHandlers;
using System.Collections.Generic;
using Xunit;

namespace SmartSql.Test.Unit.TypeHandlers
{
    public class NamedTypeHandlerCacheTests
    {
        [Fact]
        public void Should_BuildAndRetrieveTypeHandlers_When_CacheIsBuilt()
        {
            var alias = "NamedTypeHandlerCacheTest";

            JsonTypeHandler expectedJson = new JsonTypeHandler();
            XmlTypeHandler expectedXml = new XmlTypeHandler();
            var namedTypeHandlers = new Dictionary<string, ITypeHandler>
            {
                { "Json", expectedJson },
                { "Xml", expectedXml }
            };

            NamedTypeHandlerCache.Build(alias, namedTypeHandlers);

            var actualJson = NamedTypeHandlerCache.GetTypeHandlerField(alias, "Json").GetValue(null);
            actualJson.Should().Be(expectedJson);

            var actualXml = NamedTypeHandlerCache.GetTypeHandlerField(alias, "Xml").GetValue(null);
            actualXml.Should().Be(expectedXml);
        }
    }
}
