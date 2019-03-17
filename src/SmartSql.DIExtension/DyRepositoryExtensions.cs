using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using SmartSql;
using SmartSql.DIExtension;
using SmartSql.DyRepository;
using System;
using System.Linq;
using System.Reflection;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class DyRepositoryExtensions
    {
        /// <summary>
        /// 注入SmartSql仓储工厂
        /// </summary>
        /// <param name="services"></param>
        /// <param name="scope_template">Scope模板，默认：I{Scope}Repository</param>
        /// <param name="sqlIdNamingConvert">SqlId命名转换</param>
        public static IServiceCollection AddRepositoryFactory(this IServiceCollection services, string scope_template = "", Func<Type, MethodInfo, String> sqlIdNamingConvert = null)
        {
            services.TryAddSingleton<IRepositoryBuilder>((sp) =>
            {
                var loggerFactory = sp.GetService<ILoggerFactory>() ?? Logging.Abstractions.NullLoggerFactory.Instance;
                var logger = loggerFactory.CreateLogger<EmitRepositoryBuilder>();
                return new EmitRepositoryBuilder(scope_template, sqlIdNamingConvert, logger);
            });
            services.TryAddSingleton<IRepositoryFactory>((sp) =>
            {
                var loggerFactory = sp.GetService<ILoggerFactory>() ?? Logging.Abstractions.NullLoggerFactory.Instance;
                var logger = loggerFactory.CreateLogger<RepositoryFactory>();
                var repositoryBuilder = sp.GetRequiredService<IRepositoryBuilder>();
                return new RepositoryFactory(repositoryBuilder, logger);
            });
            return services;
        }
        /// <summary>
        /// 注入单个仓储接口
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="services"></param>
        /// <param name="smartSqlAlias">SmartSql别名</param>
        /// <param name="scope">SqlMaper.Scope</param>
        public static IServiceCollection AddRepository<T>(this IServiceCollection services, string smartSqlAlias, string scope = "") where T : class
        {
            services.AddRepositoryFactory();
            services.AddSingleton<T>(sp =>
            {
                ISqlMapper sqlMapper = sp.GetRequiredService<ISqlMapper>(); ;
                if (!String.IsNullOrEmpty(smartSqlAlias))
                {
                    sqlMapper = sp.GetSmartSql(smartSqlAlias).SqlMapper;
                }
                var factory = sp.GetRequiredService<IRepositoryFactory>();
                return factory.CreateInstance(typeof(T), sqlMapper, scope) as T;
            });
            return services;
        }
        /// <summary>
        /// 注入仓储结构 By 程序集
        /// </summary>
        /// <param name="services"></param>
        /// <param name="setupOptions"></param>
        public static IServiceCollection AddRepositoryFromAssembly(this IServiceCollection services, Action<AssemblyAutoRegisterOptions> setupOptions)
        {
            services.AddRepositoryFactory();
            var options = new AssemblyAutoRegisterOptions
            {
                Filter = (type) => type.IsInterface
            };
            setupOptions(options);
            ScopeTemplateParser templateParser = new ScopeTemplateParser(options.ScopeTemplate);
            var assembly = Assembly.Load(options.AssemblyString);
            var allTypes = assembly.GetTypes().Where(options.Filter);
            foreach (var type in allTypes)
            {
                services.AddSingleton(type, sp =>
                {
                    ISqlMapper sqlMapper = sp.GetRequiredService<ISqlMapper>(); ;
                    if (!String.IsNullOrEmpty(options.SmartSqlAlias))
                    {
                        sqlMapper = sp.GetSmartSql(options.SmartSqlAlias).SqlMapper;
                    }
                    var factory = sp.GetRequiredService<IRepositoryFactory>();
                    var scope = string.Empty;
                    if (!String.IsNullOrEmpty(options.ScopeTemplate))
                    {
                        scope = templateParser.Parse(type.Name);
                    }
                    return factory.CreateInstance(type, sqlMapper, scope);
                });
            }
            return services;
        }
        /// <summary>
        /// AddSmartSql And AddRepositoryFactory
        /// </summary>
        /// <param name="services"></param>
        /// <param name="scope_template"></param>
        /// <param name="sqlIdNamingConvert"></param>
        public static IServiceCollection AddSmartSqlRepositoryFactory(this IServiceCollection services, string scope_template = "", Func<Type, MethodInfo, String> sqlIdNamingConvert = null)
        {
            services.AddSmartSql();
            services.AddRepositoryFactory(scope_template, sqlIdNamingConvert);
            return services;
        }
        /// <summary>
        /// AddSmartSqlRepositoryFactory And AddRepositoryFromAssembly
        /// </summary>
        /// <param name="services"></param>
        /// <param name="setupOptions"></param>
        public static IServiceCollection AddSmartSqlRepositoryFromAssembly(this IServiceCollection services, Action<AssemblyAutoRegisterOptions> setupOptions)
        {
            services.AddSmartSqlRepositoryFactory();
            services.AddRepositoryFromAssembly(setupOptions);
            return services;
        }

    }

}
