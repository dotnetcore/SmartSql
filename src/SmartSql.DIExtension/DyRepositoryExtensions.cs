using System;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using SmartSql.DIExtension;
using SmartSql.DyRepository;
using SmartSql.Utils;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class DyRepositoryExtensions
    {
        /// <summary>
        /// 注入SmartSql仓储工厂
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="scope_template">Scope模板，默认：I{Scope}Repository</param>
        /// <param name="sqlIdNamingConvert">SqlId命名转换</param>
        /// <returns></returns>
        public static SmartSqlDIBuilder AddRepositoryFactory(this SmartSqlDIBuilder builder, string scope_template = "", Func<Type, MethodInfo, String> sqlIdNamingConvert = null)
        {
            builder.Services.TryAddSingleton<IRepositoryBuilder>((sp) =>
            {
                var loggerFactory = sp.GetService<ILoggerFactory>() ?? Logging.Abstractions.NullLoggerFactory.Instance;
                var logger = loggerFactory.CreateLogger<EmitRepositoryBuilder>();
                return new EmitRepositoryBuilder(scope_template, sqlIdNamingConvert, logger);
            });
            builder.Services.TryAddSingleton<IRepositoryFactory>((sp) =>
            {
                var loggerFactory = sp.GetService<ILoggerFactory>() ?? Logging.Abstractions.NullLoggerFactory.Instance;
                var logger = loggerFactory.CreateLogger<RepositoryFactory>();
                var repositoryBuilder = sp.GetRequiredService<IRepositoryBuilder>();
                return new RepositoryFactory(repositoryBuilder, logger);
            });
            return builder;
        }
        /// <summary>
        /// 注入单个仓储接口
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="builder"></param>
        /// <param name="smartSqlAlias"></param>
        /// <param name="scope"></param>
        /// <returns></returns>
        public static SmartSqlDIBuilder AddRepository<T>(this SmartSqlDIBuilder builder, string smartSqlAlias, string scope = "") where T : class
        {
            builder.AddRepositoryFactory();
            builder.Services.AddSingleton(sp =>
            {
                var sqlMapper = string.IsNullOrEmpty(smartSqlAlias)
                    ? sp.EnsureSmartSql().SqlMapper
                    : sp.EnsureSmartSql(smartSqlAlias).SqlMapper;

                var factory = sp.GetRequiredService<IRepositoryFactory>();
                return factory.CreateInstance(typeof(T), sqlMapper, scope) as T;
            });
            return builder;
        }
        /// <summary>
        /// 注入仓储结构 By 程序集
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="setupOptions"></param>
        /// <returns></returns>
        public static SmartSqlDIBuilder AddRepositoryFromAssembly(this SmartSqlDIBuilder builder, Action<AssemblyAutoRegisterOptions> setupOptions)
        {
            builder.AddRepositoryFactory();
            var options = new AssemblyAutoRegisterOptions
            {
                Filter = type => type.IsInterface
            };
            setupOptions(options);
            ScopeTemplateParser templateParser = new ScopeTemplateParser(options.ScopeTemplate);
            var allTypes = TypeScan.Scan(options);
            foreach (var type in allTypes)
            {
                builder.Services.AddSingleton(type, sp =>
                {
                    var sqlMapper = string.IsNullOrEmpty(options.SmartSqlAlias)
                        ? sp.EnsureSmartSql().SqlMapper
                        : sp.EnsureSmartSql(options.SmartSqlAlias).SqlMapper;

                    var factory = sp.GetRequiredService<IRepositoryFactory>();
                    var scope = string.Empty;
                    if (!String.IsNullOrEmpty(options.ScopeTemplate))
                    {
                        scope = templateParser.Parse(type.Name);
                    }

                    var instance = factory.CreateInstance(type, sqlMapper, scope);
                    if (instance.IsDyRepository())
                    {
                        sqlMapper.SmartSqlConfig.CacheManager.Reset();
                    }
                    return instance;
                });
            }
            return builder;
        }
    }
}
