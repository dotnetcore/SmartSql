using System.IO;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SmartSql.Abstractions;
using SmartSql.Abstractions.Command;
using SmartSql.Abstractions.DataReaderDeserializer;
using SmartSql.Abstractions.DbSession;
using SmartSql.Command;
using SmartSql.DataReaderDeserializer;
using SmartSql.DbSession;
using SmartSql.Options;
using SmartSql.UTests.Entity;
using Xunit;

namespace SmartSql.UTests
{
    public class OptionConfig_Test : TestBase
    {
        private static IConfiguration Configuration;
        private ISmartSqlMapper _smartSqlMapper;

        public OptionConfig_Test()
        {
            string configPath = "SmartSqlConfig.json";
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                 .AddJsonFile(configPath);

            Configuration = builder.Build();

            var services = new ServiceCollection();
            services.AddOptions();
            services.Configure<SmartSqlConfigOptions>(Configuration);
            var provider = services.BuildServiceProvider();
            var options = provider.GetService<IOptionsSnapshot<SmartSqlConfigOptions>>();

            var _configLoader = new OptionConfigLoader(options.Value, LoggerFactory);
            SmartSqlOptions smartSqlOptions = new SmartSqlOptions
            {
                ConfigPath = configPath,
                ConfigLoader = _configLoader
            };
            _smartSqlMapper = MapperContainer.Instance.GetSqlMapper(smartSqlOptions);
        }

        [Fact]
        public void Des()
        {
            RequestContext context = new RequestContext
            {
                Scope = Scope,
                SqlId = "Query",
                Request = new { Taken = 10 }
            };

            var list = _smartSqlMapper.Query<T_Entity>(context);

        }
    }
}