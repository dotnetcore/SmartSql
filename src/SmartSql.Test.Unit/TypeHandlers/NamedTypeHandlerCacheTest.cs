using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Text;
using SmartSql.TypeHandler;
using SmartSql.TypeHandlers;
using Xunit;

namespace SmartSql.Test.Unit.TypeHandlers
{
    public class NamedTypeHandlerCacheTest
    {
        [Fact]
        public void Build()
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
            Assert.Equal(expectedJson, actualJson);

            var actualXml = NamedTypeHandlerCache.GetTypeHandlerField(alias, "Xml").GetValue(null);
            Assert.Equal(expectedXml, actualXml);
        }
    }
}