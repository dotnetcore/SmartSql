using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Microsoft.Extensions.Configuration;

namespace SmartSql
{
    public static class SmartSqlBuilderExtensions
    {
        public static SmartSqlBuilder UseProperties(this SmartSqlBuilder smartSqlBuilder, IConfiguration configuration)
        {
            return smartSqlBuilder.UseProperties(configuration.AsEnumerable());
        }
    }
}
