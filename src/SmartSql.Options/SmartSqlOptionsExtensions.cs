using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SmartSql.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmartSql
{
    public static class SmartSqlOptionsExtensions
    {
        public static SmartSqlBuilder UseOptions(this SmartSqlBuilder builder
            , IServiceProvider serviceProvider)
        {
            var configOptions = serviceProvider
                   .GetRequiredService<IOptionsSnapshot<SmartSqlConfigOptions>>()
                   .Get(builder.Alias);
            var loggerFactory = serviceProvider.GetService<ILoggerFactory>();
            var configBuilder = new OptionConfigBuilder(configOptions, loggerFactory);
            builder.UseConfigBuilder(configBuilder);
            return builder;
        }
    }
}
