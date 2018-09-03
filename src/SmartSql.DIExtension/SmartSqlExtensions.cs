using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using SmartSql;
using SmartSql.Abstractions;
using SmartSql.Abstractions.Config;
using System;
using System.IO;
namespace Microsoft.Extensions.DependencyInjection
{
    public static class SmartSqlExtensions
    {
        public static void AddSmartSql(this IServiceCollection services)
        {
            services.TryAddSingleton(sp =>
            {
                var options = new SmartSqlOptions();
                InitOptions(sp, options);
                return MapperContainer.Instance.GetSqlMapper(options);
            });
            AddOthers(services);
        }

        public static void AddSmartSql(this IServiceCollection services, Func<IServiceProvider, SmartSqlOptions> setupOptions)
        {
            services.TryAddSingleton((sp =>
            {
                var options = setupOptions(sp);
                InitOptions(sp, options);
                return MapperContainer.Instance.GetSqlMapper(options);
            }));
            AddOthers(services);
        }

        private static void InitOptions(IServiceProvider sp, SmartSqlOptions options)
        {
            if (String.IsNullOrEmpty(options.ConfigPath))
            {
                var env = sp.GetService<IHostingEnvironment>();
                if (env != null && !env.IsProduction())
                {
                    options.ConfigPath = $"SmartSqlMapConfig.{env.EnvironmentName}.xml";
                }
                if (!File.Exists(options.ConfigPath))
                {
                    options.ConfigPath = Consts.DEFAULT_SMARTSQL_CONFIG_PATH;
                }
            }
            options.LoggerFactory = options.LoggerFactory ?? sp.GetService<ILoggerFactory>();
            if (options.ConfigLoader == null)
            {
                options.ConfigLoader = sp.GetService<IConfigLoader>();
            }
        }

        private static void AddOthers(IServiceCollection services)
        {
            services.TryAddSingleton<ISmartSqlMapperAsync>(sp =>
            {
                return sp.GetRequiredService<ISmartSqlMapper>();
            });
            services.TryAddSingleton<ITransaction>(sp =>
            {
                return sp.GetRequiredService<ISmartSqlMapper>();
            });
            services.TryAddSingleton<ISession>(sp =>
            {
                return sp.GetRequiredService<ISmartSqlMapper>();
            });
        }
    }
}
