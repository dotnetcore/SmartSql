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
        public void Init()
        {
            var alias = "NamedTypeHandlerCacheTest";
            var namedTypeHandlers = new Dictionary<string, ITypeHandler>
            {
                {"Json", new JsonTypeHandler()},
                { "Xml", new XmlTypeHandler()}
            };
            NamedTypeHandlerCache.Build(alias, namedTypeHandlers);
            var jsonHandlerField = NamedTypeHandlerCache.GetTypeHandlerField(alias, "Json");
            var jsonTypeHandler = jsonHandlerField.GetValue(null);
        }
    }
}
