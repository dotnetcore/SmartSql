using SmartSql;
using SmartSql.Abstractions;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class SmartSqlExtensions
    {
        public static void AddSmartSql(this IServiceCollection services)
        {
            services.AddSingleton<ISmartSqlMapper>(MapperContainer.Instance.GetSqlMapper());
        }
        public static void AddSmartSql(this IServiceCollection services, SmartSqlOptions smartSqlOptions)
        {
            services.AddSingleton<ISmartSqlMapper>(MapperContainer.Instance.GetSqlMapper(smartSqlOptions));
        }

        public static void AddSmartSql(this IServiceCollection services, Func<IServiceProvider, SmartSqlOptions> setupOptions)
        {
            services.AddSingleton<ISmartSqlMapper>((sp =>
            {
                var options = setupOptions(sp);
                return MapperContainer.Instance.GetSqlMapper(options);
            }));
        }
    }
}
