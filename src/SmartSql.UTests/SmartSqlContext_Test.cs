using Microsoft.Extensions.Logging;
using SmartSql.Abstractions.Config;
using Xunit;

namespace SmartSql.UTests
{
    public class SmartSqlContext_Test : TestBase
    {
        IConfigLoader _configLoader;
        SmartSqlContext _smartSqlContext;
        public SmartSqlContext_Test()
        {
            _configLoader = new LocalFileConfigLoader("SmartSqlMapConfig.xml", LoggerFactory);
            var config = _configLoader.Load();
            _smartSqlContext = new SmartSqlContext(LoggerFactory.CreateLogger<SmartSqlContext>(), _configLoader.Load());
        }
        [Fact]
        public void Load()
        {
            Assert.NotNull(_smartSqlContext);
        }
    }
}
