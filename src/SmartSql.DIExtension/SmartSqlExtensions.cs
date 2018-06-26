using Microsoft.Extensions.Logging;
using SmartSql;
using SmartSql.Abstractions;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class SmartSqlExtensions
    {
        public static void AddSmartSql(this IServiceCollection services)
        {
            services.AddSingleton<ISmartSqlMapper>(sp =>
            {
                var options = new SmartSqlOptions
                {
                    LoggerFactory = sp.GetService<ILoggerFactory>()
                };
                return MapperContainer.Instance.GetSqlMapper(options);
            });
            AddOthers(services);
        }

        public static void AddSmartSql(this IServiceCollection services, Func<IServiceProvider, SmartSqlOptions> setupOptions)
        {
            services.AddSingleton<ISmartSqlMapper>((sp =>
           {
               var options = setupOptions(sp);
               return MapperContainer.Instance.GetSqlMapper(options);
           }));
            AddOthers(services);
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
    }
}
