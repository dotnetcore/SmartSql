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
using System.Collections.Generic;

namespace SmartSql
{
    public class SmartSqlBuilder
    {
        /// <summary>
        /// 默认 XML 配置文件地址
        /// </summary>
        public const string DEFAULT_SMARTSQL_CONFIG_PATH = "SmartSqlMapConfig.xml";
        public IConfigBuilder ConfigBuilder { get; }
        public SmartSqlConfig SmartSqlConfig { get; private set; }
        public IDbSessionFactory DbSessionFactory { get; private set; }
        public ISqlMapper SqlMapper { get; private set; }
        public bool Built { get; private set; }

        private SmartSqlBuilder(IConfigBuilder configBuilder)
        {
            ConfigBuilder = configBuilder;
        }

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
            SmartSqlConfig = ConfigBuilder.Build(_importProperties);
            if (_loggerFactory != null)
            {
                SmartSqlConfig.LoggerFactory = _loggerFactory;
            }
            if (_dataSourceFilter != null)
            {
                SmartSqlConfig.DataSourceFilter = _dataSourceFilter;
            }
            else
            {
                SmartSqlConfig.DataSourceFilter = new DataSourceFilter(SmartSqlConfig.LoggerFactory);
            }
            if (_isCacheEnabled.HasValue)
            {
                SmartSqlConfig.Settings.IsCacheEnabled = _isCacheEnabled.Value;
            }
            if (!String.IsNullOrEmpty(_alias))
            {
                SmartSqlConfig.Alias = _alias;
            }
            SmartSqlConfig.SqlParamAnalyzer = new SqlParamAnalyzer(SmartSqlConfig.Settings.IgnoreParameterCase, SmartSqlConfig.Database.DbProvider.ParameterPrefix);
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

        private ILoggerFactory _loggerFactory = NullLoggerFactory.Instance;
        private IDataSourceFilter _dataSourceFilter;
        private bool? _isCacheEnabled;
        private string _alias;
        private IDictionary<string, string> _importProperties;
        public SmartSqlBuilder UseLoggerFactory(ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory; return this;
        }
        public SmartSqlBuilder UseDataSourceFilter(IDataSourceFilter dataSourceFilter)
        {
            _dataSourceFilter = dataSourceFilter; return this;
        }
        public SmartSqlBuilder UseCache(bool isCacheEnabled = true)
        {
            _isCacheEnabled = isCacheEnabled; return this;
        }
        public SmartSqlBuilder UseAlias(String alias)
        {
            _alias = alias; return this;
        }
        public SmartSqlBuilder UseProperties(IDictionary<string, string> importProperties)
        {
            _importProperties = importProperties; return this;
        }
        #endregion

        #region Static Config
        public static SmartSqlBuilder AddConfigBuilder(IConfigBuilder configBuilder)
        {
            if (configBuilder == null)
            {
                throw new ArgumentNullException(nameof(configBuilder));
            }
            return new SmartSqlBuilder(configBuilder);
        }
        /// <summary>
        /// SmartSqlMapConfig 配置方式构建
        /// </summary>
        /// <param name="smartSqlConfig"></param>
        /// <returns></returns>
        public static SmartSqlBuilder AddNativeConfig(SmartSqlConfig smartSqlConfig)
        {
            if (smartSqlConfig == null)
            {
                throw new ArgumentNullException(nameof(smartSqlConfig));
            }
            var configBuilder = new NativeConfigBuilder(smartSqlConfig);
            return AddConfigBuilder(configBuilder);
        }
        /// <summary>
        /// Xml 配置方式构建
        /// </summary>
        /// <param name="smartSqlMapConfig"></param>
        /// <returns></returns>
        public static SmartSqlBuilder AddXmlConfig(ResourceType resourceType = ResourceType.File, String smartSqlMapConfig = DEFAULT_SMARTSQL_CONFIG_PATH)
        {
            var configBuilder = new XmlConfigBuilder(resourceType, smartSqlMapConfig);
            return AddConfigBuilder(configBuilder);
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
            return AddNativeConfig(smartSqlConfig);
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
