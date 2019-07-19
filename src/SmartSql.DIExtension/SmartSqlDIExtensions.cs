using System;
using System.IO;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SmartSql;
using SmartSql.Configuration;
using SmartSql.DbSession;
using SmartSql.DIExtension;
using SmartSql.Utils;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class SmartSqlDIExtensions
    {
        public static SmartSqlDIBuilder AddSmartSql(this IServiceCollection services,
            Func<IServiceProvider, SmartSqlBuilder> setup)
        {
            services.AddSingleton<SmartSqlBuilder>(sp => setup(sp).Build());
            AddOthers(services);
            return new SmartSqlDIBuilder(services);
        }

        public static SmartSqlDIBuilder AddSmartSql(this IServiceCollection services,
            Action<IServiceProvider, SmartSqlBuilder> setup)
        {
            return services.AddSmartSql(sp =>
            {
                var loggerFactory = sp.GetService<ILoggerFactory>();
                var smartSqlBuilder = new SmartSqlBuilder().UseLoggerFactory(loggerFactory);
                setup(sp, smartSqlBuilder);
                if (smartSqlBuilder.ConfigBuilder == null)
                {
                    var configPath = ResolveConfigPath(sp);
                    smartSqlBuilder.UseXmlConfig(SmartSql.ConfigBuilder.ResourceType.File, configPath);
                }

                return smartSqlBuilder;
            });
        }

        public static SmartSqlDIBuilder AddSmartSql(this IServiceCollection services,
            String alias = SmartSqlBuilder.DEFAULT_ALIAS)
        {
            return services.AddSmartSql((sp, builder) => { builder.UseAlias(alias); });
        }

        public static SmartSqlDIBuilder AddSmartSql(this IServiceCollection services, Action<SmartSqlBuilder> setup)
        {
            return services.AddSmartSql((sp, builder) => { setup(builder); });
        }

        private static string ResolveConfigPath(IServiceProvider sp)
        {
            var envStr = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            var configPath = SmartSqlBuilder.DEFAULT_SMARTSQL_CONFIG_PATH;
            if (!String.IsNullOrEmpty(envStr) &&
                !string.Equals(envStr, "Production", StringComparison.OrdinalIgnoreCase))
            {
                configPath = $"SmartSqlMapConfig.{envStr}.xml";
            }

            if (!ResourceUtil.FileExists(configPath))
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
            services.AddSingleton<IDbSessionStore>(sp =>
                sp.GetRequiredService<SmartSqlBuilder>().SmartSqlConfig.SessionStore);
        }
    }
}