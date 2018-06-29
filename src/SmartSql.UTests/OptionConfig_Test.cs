using System.IO;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SmartSql.Abstractions;
using SmartSql.Abstractions.Command;
using SmartSql.Abstractions.Config;
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
        private ISmartSqlMapper _smartSqlMapper;

        public OptionConfig_Test()
        {
            var builder = new ConfigurationBuilder()
                            .SetBasePath(Directory.GetCurrentDirectory())
                             .AddJsonFile("SmartSqlConfig.json", false, true);

            var configuration = builder.Build();
            var services = new ServiceCollection();
            services.AddOptions();

            services.Configure<SmartSqlConfigOptions>(configuration);

            services.AddSingleton<ILoggerFactory>(LoggerFactory);

            services.AddSmartSqlOption();

            var serviceProvider = services.BuildServiceProvider();
            _smartSqlMapper = serviceProvider.GetRequiredService<ISmartSqlMapper>();
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