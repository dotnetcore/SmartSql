using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SmartSql;
using SmartSql.Abstractions.Config;
using SmartSql.Logging;
using SmartSql.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class SmartSqlOptionsExtensions
    {
        [Obsolete("Please use SmartSqlOptions.UseOptions!")]
        public static void AddSmartSqlOptionLoader(this IServiceCollection services, string configName = "")
        {
            services.AddSingleton((sp) =>
            {
                return BuildConfigLoader(sp, configName);
            });
        }
        [Obsolete("Please use SmartSqlOptions.UseOptions!")]
        public static void AddSmartSqlOption(this IServiceCollection services)
        {
            services.AddSmartSqlOptionLoader();
            services.AddSmartSql(sp =>
            {
                var loggerFactory = sp.GetService<ILoggerFactory>() ?? NoneLoggerFactory.Instance;
                var configLoader = sp.GetRequiredService<IConfigLoader>();
                return new SmartSqlOptions
                {
                    ConfigLoader = configLoader,
                    LoggerFactory = loggerFactory
                };
            });
        }
        private static IConfigLoader BuildConfigLoader(IServiceProvider sp, string configName)
        {
            var loggerFactory = sp.GetService<ILoggerFactory>() ?? NoneLoggerFactory.Instance;
            var optionsMonitor = sp.GetService<IOptionsMonitor<SmartSqlConfigOptions>>();

            var smartSqlOptions = String.IsNullOrEmpty(configName)
                                        ? optionsMonitor.CurrentValue : optionsMonitor.Get(configName);

            var _configLoader = new OptionConfigLoader(smartSqlOptions, loggerFactory);
            if (smartSqlOptions.Settings.IsWatchConfigFile)
            {
                optionsMonitor.OnChange((ops, name) =>
                {
                    if (name == configName) { return; }
                    _configLoader.TriggerChanged(ops);
                });
            }
            return _configLoader;
        }
        public static SmartSqlOptions UseOptions(this SmartSqlOptions smartSqlOptions, IServiceProvider sp)
        {
            var configLoader = BuildConfigLoader(sp, smartSqlOptions.ConfigPath);
            smartSqlOptions.ConfigLoader = configLoader;
            return smartSqlOptions;
        }
    }
}
