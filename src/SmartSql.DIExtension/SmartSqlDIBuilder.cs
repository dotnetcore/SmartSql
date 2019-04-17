using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;

namespace SmartSql.DIExtension
{
    public class SmartSqlDIBuilder
    {
        public IServiceCollection Services { get; }

        public SmartSqlDIBuilder(IServiceCollection services)
        {
            Services = services;
        }
    }
}
