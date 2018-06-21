using Microsoft.Extensions.Logging;
using SmartSql.Abstractions;
using SmartSql.DyRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class DyRepositoryExtensions
    {
        public static void AddRepositoryFactory(this IServiceCollection services, string scope_template = "")
        {
            services.AddSingleton<IRepositoryBuilder>((sp) =>
            {
                var loggerFactory = sp.GetService<ILoggerFactory>();
                var logger = loggerFactory.CreateLogger<RepositoryBuilder>();
                return new RepositoryBuilder(scope_template, logger);
            });
            services.AddSingleton<IRepositoryFactory>((sp) =>
            {
                var repositoryBuilder = sp.GetRequiredService<IRepositoryBuilder>();
                var loggerFactory = sp.GetService<ILoggerFactory>();
                var logger = loggerFactory.CreateLogger<RepositoryFactory>();
                return new RepositoryFactory(repositoryBuilder, logger);
            });
        }
        public static void AddRepository<T>(this IServiceCollection services) where T : class
        {
            services.AddSingleton<T>(sp =>
            {
                var sqlMapper = sp.GetRequiredService<ISmartSqlMapper>();
                var factory = sp.GetRequiredService<IRepositoryFactory>();
                return factory.CreateInstance<T>(sqlMapper);
            });
        }

        public static void AddRepositoryFromAssembly(this IServiceCollection services, Action<AssemblyAutoRegisterOptions> setupOptions)
        {
            var options = new AssemblyAutoRegisterOptions
            {
                Filter = (type) => { return type.IsInterface; }
            };
            setupOptions(options);
            var assembly = Assembly.Load(options.AssemblyString);
            var allTypes = assembly.GetTypes().Where(options.Filter);
            foreach (var type in allTypes)
            {
                services.AddSingleton(type, sp =>
                {
                    var sqlMapper = sp.GetRequiredService<ISmartSqlMapper>();
                    var factory = sp.GetRequiredService<IRepositoryFactory>();
                    return factory.CreateInstance(type, sqlMapper);
                });
            }
        }
    }
    public class AssemblyAutoRegisterOptions
    {
        public string AssemblyString { get; set; }
        public Func<Type, bool> Filter { get; set; }

        public void UseTypeFilter<T>()
        {
            Filter = (type) =>
            {
                return typeof(T).IsAssignableFrom(type);
            };
        }
    }
}
