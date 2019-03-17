using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using SmartSql.Cache;
using SmartSql.ConfigBuilder;
using SmartSql.Configuration;
using SmartSql.Configuration.Tags;
using SmartSql.DataSource;
using SmartSql.DbSession;
using SmartSql.Deserializer;
using SmartSql.Exceptions;
using SmartSql.Middlewares;
using SmartSql.Reflection.ObjectFactoryBuilder;
using SmartSql.TypeHandlers;
using SmartSql.Utils;
using System;

namespace SmartSql
{
    public class SmartSqlBuilder
    {
        /// <summary>
        /// 默认 XML 配置文件地址
        /// </summary>
        public const string DEFAULT_SMARTSQL_CONFIG_PATH = "SmartSqlMapConfig.xml";
        public SmartSqlConfig SmartSqlConfig { get; private set; }
        public IDbSessionFactory DbSessionFactory { get; private set; }
        public ISqlMapper SqlMapper { get; private set; }
        public bool Built { get; private set; }
        public SmartSqlBuilder Build()
        {
            if (Built) return this;
            Built = true;
            BeforeBuildInitService();
            DbSessionFactory = SmartSqlConfig.DbSessionFactory;
            SqlMapper = new SqlMapper(SmartSqlConfig);
            SmartSqlContainer.Instance.TryRegister(SmartSqlConfig.Alias, this);
            return this;
        }
        public IDbSessionFactory GetDbSessionFactory()
        {
            return DbSessionFactory;
        }
        public ISqlMapper GetSqlMapper()
        {
            return SqlMapper;
        }

        private void BeforeBuildInitService()
        {
            if (SmartSqlConfig.LoggerFactory == null)
            {
                SmartSqlConfig.LoggerFactory = NullLoggerFactory.Instance;
            }
            if (SmartSqlConfig.ObjectFactoryBuilder == null)
            {
                SmartSqlConfig.ObjectFactoryBuilder = new ExpressionObjectFactoryBuilder();
            }
            if (SmartSqlConfig.TagBuilderFactory == null)
            {
                SmartSqlConfig.TagBuilderFactory = new TagBuilderFactory();
            }
            if (SmartSqlConfig.TypeHandlerFactory == null)
            {
                SmartSqlConfig.TypeHandlerFactory = new TypeHandlerFactory();
            }
            if (SmartSqlConfig.DeserializerFactory == null)
            {
                SmartSqlConfig.DeserializerFactory = new DeserializerFactory();
            }
            if (SmartSqlConfig.DbSessionFactory == null)
            {
                SmartSqlConfig.DbSessionFactory = new DbSessionFactory(SmartSqlConfig);
            }
            if (SmartSqlConfig.SessionStore == null)
            {
                SmartSqlConfig.SessionStore = new DbSessionStore(SmartSqlConfig.DbSessionFactory);
            }
            if (SmartSqlConfig.DataSourceFilter == null)
            {
                SmartSqlConfig.DataSourceFilter = new DataSourceFilter();
            }
            if (SmartSqlConfig.StatementAnalyzer == null)
            {
                SmartSqlConfig.StatementAnalyzer = new StatementAnalyzer();
            }
            if (SmartSqlConfig.SqlParamAnalyzer == null)
            {
                SmartSqlConfig.SqlParamAnalyzer = new SqlParamAnalyzer(SmartSqlConfig.Settings.IgnoreParameterCase, SmartSqlConfig.Database.DbProvider.ParameterPrefix);
            }
            BuildPipeline();
        }

        private void BuildPipeline()
        {
            if (SmartSqlConfig.Pipeline != null)
            {
                return;
            }
            if (SmartSqlConfig.Settings.IsCacheEnabled)
            {
                SmartSqlConfig.CacheManager = new CacheManager(SmartSqlConfig);
                SmartSqlConfig.Pipeline = new PipelineBuilder()
                     .Add(new InitializerMiddleware(SmartSqlConfig))
                     .Add(new PrepareStatementMiddleware(SmartSqlConfig))
                     .Add(new CachingMiddleware(SmartSqlConfig))
                     .Add(new DataSourceFilterMiddleware(SmartSqlConfig))
                     .Add(new CommandExecuterMiddleware())
                     .Add(new ResultHandlerMiddleware(SmartSqlConfig)).Build();
            }
            else
            {
                SmartSqlConfig.CacheManager = new NoneCacheManager();
                SmartSqlConfig.Pipeline = new PipelineBuilder()
                     .Add(new InitializerMiddleware(SmartSqlConfig))
                     .Add(new PrepareStatementMiddleware(SmartSqlConfig))
                     .Add(new DataSourceFilterMiddleware(SmartSqlConfig))
                     .Add(new CommandExecuterMiddleware())
                     .Add(new ResultHandlerMiddleware(SmartSqlConfig)).Build();
            }
        }

        #region Instance

        public SmartSqlBuilder UseLoggerFactory(ILoggerFactory loggerFactory)
        {
            SmartSqlConfig.LoggerFactory = loggerFactory; return this;
        }
        public SmartSqlBuilder UseObjectFactoryBuilder(IObjectFactoryBuilder objectFactoryBuilder)
        {
            SmartSqlConfig.ObjectFactoryBuilder = objectFactoryBuilder; return this;
        }
        public SmartSqlBuilder UseTagBuilderFactory(ITagBuilderFactory tagBuilderFactory)
        {
            SmartSqlConfig.TagBuilderFactory = tagBuilderFactory; return this;
        }
        public SmartSqlBuilder UseDeserializerFactory(IDeserializerFactory deserializerFactory)
        {
            SmartSqlConfig.DeserializerFactory = deserializerFactory; return this;
        }
        public SmartSqlBuilder UseDbSessionFactory(IDbSessionFactory dbSessionFactory)
        {
            SmartSqlConfig.DbSessionFactory = dbSessionFactory; return this;
        }
        public SmartSqlBuilder UseSessionStore(IDbSessionStore sessionStore)
        {
            SmartSqlConfig.SessionStore = sessionStore; return this;
        }
        public SmartSqlBuilder UseDataSourceFilter(IDataSourceFilter dataSourceFilter)
        {
            SmartSqlConfig.DataSourceFilter = dataSourceFilter; return this;
        }
        public SmartSqlBuilder UseCache(bool isCacheEnabled = true)
        {
            SmartSqlConfig.Settings.IsCacheEnabled = isCacheEnabled; return this;
        }
        public SmartSqlBuilder UseAlias(String alias)
        {
            SmartSqlConfig.Alias = alias; return this;
        }
        #endregion

        #region Static Config
        /// <summary>
        /// SmartSqlMapConfig 配置方式构建
        /// </summary>
        /// <param name="smartSqlConfig"></param>
        /// <returns></returns>
        public static SmartSqlBuilder AddConfig(SmartSqlConfig smartSqlConfig)
        {
            if (smartSqlConfig == null)
            {
                throw new ArgumentNullException(nameof(smartSqlConfig));
            }
            return new SmartSqlBuilder
            {
                SmartSqlConfig = smartSqlConfig
            };
        }
        /// <summary>
        /// Xml 配置方式构建
        /// </summary>
        /// <param name="smartSqlMapConfig"></param>
        /// <returns></returns>
        public static SmartSqlBuilder AddXmlConfig(ResourceType resourceType = ResourceType.File, String smartSqlMapConfig = DEFAULT_SMARTSQL_CONFIG_PATH)
        {
            var smartSqlConfig = new XmlConfigBuilder(resourceType, smartSqlMapConfig).Build();
            return AddConfig(smartSqlConfig);
        }
        /// <summary>
        /// 数据源方式构建
        /// </summary>
        /// <param name="writeDataSource"></param>
        /// <returns></returns>
        public static SmartSqlBuilder AddDataSource(WriteDataSource writeDataSource)
        {
            if (writeDataSource == null)
            {
                throw new ArgumentNullException(nameof(writeDataSource));
            }
            var smartSqlConfig = new SmartSqlConfig
            {
                Database = new Database
                {
                    DbProvider = writeDataSource.DbProvider,
                    Write = writeDataSource
                }
            };
            return AddConfig(smartSqlConfig);
        }

        public static SmartSqlBuilder AddDataSource(String dbProviderName, String connectionString)
        {
            if (string.IsNullOrEmpty(dbProviderName))
            {
                throw new ArgumentNullException(nameof(dbProviderName));
            }

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new ArgumentNullException(nameof(connectionString));
            }
            if (!DbProviderManager.Instance.TryGet(dbProviderName, out var dbProvider))
            {
                throw new SmartSqlException($"can not find {dbProviderName}.");
            }
            WriteDataSource writeDataSource = new WriteDataSource
            {
                Name = "Write",
                ConnectionString = connectionString,
                DbProvider = dbProvider
            };
            return AddDataSource(writeDataSource);
        }
        #endregion
    }
}
