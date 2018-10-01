using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using SmartSql;
using SmartSql.Abstractions;
using SmartSql.Abstractions.Config;
using System;
using System.IO;
using System.Linq;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class SmartSqlExtensions
    {
        /// <summary>
        /// 注入默认 ISmartSqlMapper
        /// </summary>
        /// <param name="services"></param>
        public static void AddSmartSql(this IServiceCollection services)
        {
            services.AddSingleton(sp =>
            {
                var options = new SmartSqlOptions();
                InitOptions(sp, options);
                return MapperContainer.Instance.GetSqlMapper(options);
            });
            AddOthers(services);
        }
        /// <summary>
        /// 注入 ISmartSqlMapper
        /// </summary>
        /// <param name="services"></param>
        /// <param name="setupOptions"></param>
        public static void AddSmartSql(this IServiceCollection services, Func<IServiceProvider, SmartSqlOptions> setupOptions)
        {
            services.AddSingleton((sp =>
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
            if (String.IsNullOrEmpty(options.Alias))
            {
                options.Alias = options.ConfigPath;
            }
            if (options.LoggerFactory == null)
            {
                options.LoggerFactory = sp.GetService<ILoggerFactory>();
            }
            if (options.ConfigLoader == null)
            {
                options.ConfigLoader = sp.GetService<IConfigLoader>();
            }
        }

        private static void AddOthers(IServiceCollection services)
        {
            services.AddSingleton<ISmartSqlMapperAsync>(sp =>
            {
                return sp.GetRequiredService<ISmartSqlMapper>();
            });
            services.AddSingleton<ITransaction>(sp =>
            {
                return sp.GetRequiredService<ISmartSqlMapper>();
            });
            services.AddSingleton<ISession>(sp =>
            {
                return sp.GetRequiredService<ISmartSqlMapper>();
            });
        }

        public static ISmartSqlMapper GetSmartSqlMapper(this IServiceProvider sp, string alias = Consts.DEFAULT_SMARTSQL_CONFIG_PATH)
        {
            return sp.GetServices<ISmartSqlMapper>().FirstOrDefault(m => m.SmartSqlOptions.Alias == alias);
        }
    }
}
