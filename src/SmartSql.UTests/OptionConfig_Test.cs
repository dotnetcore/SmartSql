using System.IO;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
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
        [Fact]
        public void Des()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                 .AddJsonFile("SmartSqlConfig.json", false, true);

            var configuration = builder.Build();
            var services = new ServiceCollection();
            services.AddSingleton<ILoggerFactory>(LoggerFactory);

            services.AddOptions();
            var smartSqlConfigJson = configuration.GetSection("SmartSqlConfig");
            services.Configure<SmartSqlConfigOptions>("SmartSql", smartSqlConfigJson);

            services.AddSmartSql(sp =>
            {
                return new SmartSqlOptions
                {
                     ConfigPath= "SmartSql"
                }.UseOptions(sp);
            });
            var serviceProvider = services.BuildServiceProvider();
            var _smartSqlMapper = serviceProvider.GetRequiredService<ISmartSqlMapper>();
            RequestContext context = new RequestContext
            {
                Scope = Scope,
                SqlId = "Query",
                Request = new { Taken = 10 }
            };

            var list = _smartSqlMapper.Query<T_Entity>(context);
        }

        [Fact]
        public void Des_Muti()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                 .AddJsonFile("SmartSqlConfig.json", false, true);

            var configuration = builder.Build();
            var services = new ServiceCollection();
            services.AddSingleton<ILoggerFactory>(LoggerFactory);

            services.AddOptions();
            var smartSqlConfigJson = configuration.GetSection("SmartSqlConfig");
            var smartSqlConfigJson_1 = configuration.GetSection("SmartSqlConfig-1");

            services.Configure<SmartSqlConfigOptions>("SmartSql", smartSqlConfigJson);
            services.Configure<SmartSqlConfigOptions>("SmartSql-1", smartSqlConfigJson_1);

            services.AddSmartSql(sp =>
            {
                return new SmartSqlOptions
                {
                    ConfigPath = "SmartSql"
                }.UseOptions(sp);
            });

            services.AddSmartSql(sp =>
            {
                return new SmartSqlOptions
                {
                    ConfigPath = "SmartSql-1"
                }.UseOptions(sp);
            });

            var serviceProvider = services.BuildServiceProvider();

            var smartSqlMapper = serviceProvider.GetSmartSqlMapper("SmartSql");
            var smartSqlMapper_1 = serviceProvider.GetSmartSqlMapper("SmartSql-1");

            RequestContext context = new RequestContext
            {
                Scope = Scope,
                SqlId = "Query",
                Request = new { Taken = 10 }
            };

            var list = smartSqlMapper.Query<T_Entity>(context);
            var list_1 = smartSqlMapper_1.Query<T_Entity>(context);
        }
    }
}