using SmartSql.DataSource;
using SmartSql.TypeHandlers;
using System;
using System.Collections.Generic;
using SmartSql.Configuration.Tags;
using SmartSql.Deserializer;
using SmartSql.Reflection.ObjectFactoryBuilder;
using SmartSql.Utils;
using Microsoft.Extensions.Logging;
using SmartSql.Exceptions;
using SmartSql.DbSession;
using SmartSql.Cache;
using SmartSql.IdGenerator;

namespace SmartSql.Configuration
{
    public class SmartSqlConfig
    {
        public const string DEFAULT_ALIAS = "SmartSql";
        public static Settings DefaultSettings = new Settings
        {
            IgnoreParameterCase = false,
            IsCacheEnabled = false,
            ParameterPrefix = "$"
        };
        public String Alias { get; set; }
        public Settings Settings { get; set; }
        public Database Database { get; set; }
        public Properties Properties { get; set; }
        public IDictionary<String, SqlMap> SqlMaps { get; set; }
        public ILoggerFactory LoggerFactory { get; set; }
        public IObjectFactoryBuilder ObjectFactoryBuilder { get; set; }
        public IDeserializerFactory DeserializerFactory { get; set; }
        public ITypeHandlerFactory TypeHandlerFactory { get; set; }
        public ITagBuilderFactory TagBuilderFactory { get; set; }
        public StatementAnalyzer StatementAnalyzer { get; set; }
        public SqlParamAnalyzer SqlParamAnalyzer { get; set; }
        public IMiddleware Pipeline { get; set; }
        public IDataSourceFilter DataSourceFilter { get; set; }
        public IDbSessionStore SessionStore { get; set; }
        public IDbSessionFactory DbSessionFactory { get; set; }
        public ICacheManager CacheManager { get; set; }
        public IIdGenerator IdGenerator { get; set; }
        public SqlMap GetSqlMap(string scope)
        {
            if (!SqlMaps.TryGetValue(scope, out var sqlMap))
            {
                throw new SmartSqlException($"Can not find SqlMap.Scope:{scope}");
            }
            return sqlMap;
        }

        public SmartSqlConfig()
        {
            Alias = DEFAULT_ALIAS;
            Settings = DefaultSettings;
            SqlMaps = new Dictionary<string, SqlMap>();
            ObjectFactoryBuilder = new ExpressionObjectFactoryBuilder();
            TagBuilderFactory = new TagBuilderFactory();
            TypeHandlerFactory = new TypeHandlerFactory();
            IdGenerator = SnowflakeId.Default;
        }
    }
    public class Settings
    {
        public bool IgnoreParameterCase { get; set; }
        public bool IsCacheEnabled { get; set; }
        public string ParameterPrefix { get; set; }
    }
}
