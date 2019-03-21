using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SmartSql.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xunit;

namespace SmartSql.Test.Unit
{
    public class OptionConfigBuilderTest
    {
        [Fact]
        public void DI()
        {
            var configBuilder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                 .AddJsonFile("SmartSqlMapConfig.json", false, true);

            var configuration = configBuilder.Build();
            var services = new ServiceCollection();
            services.AddOptions();
            var smartSqlConfigJson = configuration.GetSection("SmartSqlMapConfig");
            services.Configure<SmartSqlConfigOptions>("SmartSql", smartSqlConfigJson);

            services.AddSmartSql((sp) =>
            {
                var options = sp.GetRequiredService<IOptionsSnapshot<SmartSqlConfigOptions>>().Get("SmartSql");
                return new SmartSqlBuilder()
                .UseOptions(options, sp.GetService<ILoggerFactory>());
            });
            var serviceProvider = services.BuildServiceProvider();
            var sqlMapper = serviceProvider.GetRequiredService<ISqlMapper>();
        }
    }
}
