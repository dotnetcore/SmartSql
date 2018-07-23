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
        public static void AddSmartSqlOptionLoader(this IServiceCollection services)
        {
            services.AddSingleton<IConfigLoader>((sp) =>
            {
                var loggerFactory = sp.GetService<ILoggerFactory>() ?? NoneLoggerFactory.Instance;
                var optionsMonitor = sp.GetService<IOptionsMonitor<SmartSqlConfigOptions>>();
                var _configLoader = new OptionConfigLoader(optionsMonitor.CurrentValue, loggerFactory);
                SmartSqlOptions smartSqlOptions = new SmartSqlOptions
                {
                    ConfigLoader = _configLoader,
                    LoggerFactory = loggerFactory
                };
                if (optionsMonitor.CurrentValue.Settings.IsWatchConfigFile)
                {
                    optionsMonitor.OnChange((ops, name) =>
                    {
                        _configLoader.TriggerChanged(ops);
                    });
                }
                return _configLoader;
            });
        }
        public static void AddSmartSqlOption(this IServiceCollection services)
        {
            services.AddSmartSqlOptionLoader();
            services.AddSmartSql(sp =>
            {
                var loggerFactory = sp.GetService<ILoggerFactory>() ?? NoneLoggerFactory.Instance;
                var _configLoader = sp.GetRequiredService<IConfigLoader>();
                return new SmartSqlOptions
                {
                    ConfigLoader = _configLoader,
                    LoggerFactory = loggerFactory
                };
            });
        }

        public static void UserOptions(this SmartSqlOptions smartSqlOptions, IServiceProvider sp)
        {
            var _configLoader = sp.GetRequiredService<IConfigLoader>();
            smartSqlOptions.ConfigLoader = _configLoader;
        }
    }
}
