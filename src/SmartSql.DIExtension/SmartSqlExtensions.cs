using System;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SmartSql;
using SmartSql.Configuration;
using SmartSql.DbSession;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class SmartSqlExtensions
    {
        public static IServiceCollection AddSmartSql(this IServiceCollection services, String alias = SmartSqlConfig.DEFAULT_ALIAS)
        {
            services.AddSingleton<SmartSqlBuilder>(sp =>
            {
                var configPath = ResolveConfigPath(sp);
                return SmartSqlBuilder.AddXmlConfig(SmartSql.ConfigBuilder.ResourceType.File, configPath)
                 .UseAlias(alias)
                 .UseLoggerFactory(sp.GetService<ILoggerFactory>())
                 .Build();
            });
            AddOthers(services);
            return services;
        }

        private static string ResolveConfigPath(IServiceProvider sp)
        {
            var env = sp.GetService<IHostingEnvironment>();
            var configPath = SmartSqlBuilder.DEFAULT_SMARTSQL_CONFIG_PATH;
            if (env != null && !env.IsProduction())
            {
                configPath = $"SmartSqlMapConfig.{env.EnvironmentName}.xml";
            }
            if (!File.Exists(configPath))
            {
                configPath = SmartSqlBuilder.DEFAULT_SMARTSQL_CONFIG_PATH;
            }
            return configPath;
        }

        private static void AddOthers(IServiceCollection services)
        {
            services.AddSingleton<IDbSessionFactory>(sp => sp.GetRequiredService<SmartSqlBuilder>().DbSessionFactory);
            services.AddSingleton<ISqlMapper>(sp => sp.GetRequiredService<SmartSqlBuilder>().SqlMapper);
            services.AddSingleton<ITransaction>(sp => sp.GetRequiredService<SmartSqlBuilder>().SqlMapper);
        }

        public static IServiceCollection AddSmartSql(this IServiceCollection services, Func<IServiceProvider, SmartSqlBuilder> setup)
        {
            services.AddSingleton<SmartSqlBuilder>(sp => setup(sp).Build());
            AddOthers(services);
            return services;
        }
        public static IServiceCollection AddSmartSql(this IServiceCollection services, Action<IServiceProvider, SmartSqlBuilder> setup)
        {
            services.AddSingleton<SmartSqlBuilder>(sp =>
            {
                var configPath = ResolveConfigPath(sp);
                var smartSqlBuilder = SmartSqlBuilder.AddXmlConfig(SmartSql.ConfigBuilder.ResourceType.File, configPath)
                 .UseLoggerFactory(sp.GetService<ILoggerFactory>());
                setup(sp, smartSqlBuilder);
                return smartSqlBuilder.Build();
            });
            AddOthers(services);
            return services;
        }
        public static SmartSqlBuilder GetSmartSql(this IServiceProvider sp, string alias = SmartSqlConfig.DEFAULT_ALIAS)
        {
            return sp.GetServices<SmartSqlBuilder>().FirstOrDefault(m => m.SmartSqlConfig.Alias == alias);
        }
    }
}
