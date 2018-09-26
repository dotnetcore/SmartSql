using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using SmartSql.Abstractions;
using SmartSql.DIExtension;
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
        /// <summary>
        /// 注入SmartSql仓储工厂
        /// </summary>
        /// <param name="services"></param>
        /// <param name="scope_template">Scope模板，默认：I{Scope}Repository</param>
        /// <param name="sqlIdNamingConvert">SqlId命名转换</param>
        public static void AddRepositoryFactory(this IServiceCollection services, string scope_template = "", Func<Type, MethodInfo, String> sqlIdNamingConvert = null)
        {
            services.AddSingleton<IRepositoryBuilder>((sp) =>
            {
                var loggerFactory = sp.GetRequiredService<ILoggerFactory>();
                var logger = loggerFactory.CreateLogger<RepositoryBuilder>();
                return new RepositoryBuilder(scope_template, sqlIdNamingConvert, logger);
            });
            services.AddSingleton<IRepositoryFactory>((sp) =>
            {
                var repositoryBuilder = sp.GetRequiredService<IRepositoryBuilder>();
                var loggerFactory = sp.GetRequiredService<ILoggerFactory>();
                var logger = loggerFactory.CreateLogger<RepositoryFactory>();
                return new RepositoryFactory(repositoryBuilder, logger);
            });
        }
        /// <summary>
        /// 注入单个仓储接口
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="services"></param>
        /// <param name="getSmartSql">获取ISmartSqlMapper函数</param>
        /// <param name="scope">SqlMaper.Scope</param>
        public static void AddRepository<T>(this IServiceCollection services, Func<IServiceProvider, ISmartSqlMapper> getSmartSql = null, string scope = "") where T : class
        {
            services.AddSingleton<T>(sp =>
            {
                ISmartSqlMapper sqlMapper = null;
                if (getSmartSql != null)
                {
                    sqlMapper = getSmartSql(sp);
                }
                sqlMapper = sqlMapper ?? sp.GetRequiredService<ISmartSqlMapper>();
                var factory = sp.GetRequiredService<IRepositoryFactory>();
                return factory.CreateInstance<T>(sqlMapper, scope);
            });
        }
        /// <summary>
        /// 注入仓储结构 By 程序集
        /// </summary>
        /// <param name="services"></param>
        /// <param name="setupOptions"></param>
        public static void AddRepositoryFromAssembly(this IServiceCollection services, Action<AssemblyAutoRegisterOptions> setupOptions)
        {
            var options = new AssemblyAutoRegisterOptions
            {
                Filter = (type) => { return type.IsInterface; }
            };
            setupOptions(options);
            ScopeTemplateParser templateParser = new ScopeTemplateParser(options.ScopeTemplate);
            var assembly = Assembly.Load(options.AssemblyString);
            var allTypes = assembly.GetTypes().Where(options.Filter);
            ISmartSqlMapper sqlMapper = null;
            foreach (var type in allTypes)
            {
                services.AddSingleton(type, sp =>
                {
                    if (sqlMapper == null && options.GetSmartSql != null)
                    {
                        sqlMapper = options.GetSmartSql(sp);
                    }
                    sqlMapper = sqlMapper ?? sp.GetRequiredService<ISmartSqlMapper>();
                    var factory = sp.GetRequiredService<IRepositoryFactory>();
                    var scope = string.Empty;
                    if (!String.IsNullOrEmpty(options.ScopeTemplate))
                    {
                        scope = templateParser.Parse(type.Name);
                    }
                    return factory.CreateInstance(type, sqlMapper, scope);
                });
            }
        }
        /// <summary>
        /// AddSmartSql And AddRepositoryFactory
        /// </summary>
        /// <param name="services"></param>
        /// <param name="scope_template"></param>
        /// <param name="sqlIdNamingConvert"></param>
        public static void AddSmartSqlRepositoryFactory(this IServiceCollection services, string scope_template = "", Func<Type, MethodInfo, String> sqlIdNamingConvert = null)
        {
            services.AddSmartSql();
            services.AddRepositoryFactory(scope_template, sqlIdNamingConvert);
        }
        /// <summary>
        /// AddSmartSqlRepositoryFactory And AddRepositoryFromAssembly
        /// </summary>
        /// <param name="services"></param>
        /// <param name="setupOptions"></param>
        public static void AddSmartSqlRepositoryFromAssembly(this IServiceCollection services, Action<AssemblyAutoRegisterOptions> setupOptions)
        {
            services.AddSmartSqlRepositoryFactory();
            services.AddRepositoryFromAssembly(setupOptions);
        }

    }

}
