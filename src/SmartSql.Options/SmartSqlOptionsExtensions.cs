using Microsoft.Extensions.Logging;
using SmartSql.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmartSql
{
    public static class SmartSqlOptionsExtensions
    {
        public static SmartSqlBuilder UseOptions(this SmartSqlBuilder builder, SmartSqlConfigOptions configOptions, ILoggerFactory loggerFactory = null)
        {
            var configBuilder = new OptionConfigBuilder(configOptions, loggerFactory);
            builder.UseConfigBuilder(configBuilder, loggerFactory);
            return builder;
        }



    }
}
