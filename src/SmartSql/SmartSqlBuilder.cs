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
        /// 默认 SmartSql 实例别名
        /// </summary>
        public const string DEFAULT_ALIAS = "SmartSql";
        /// <summary>
        /// 默认 XML 配置文件地址
        /// </summary>
        public const string DEFAULT_SMARTSQL_CONFIG_PATH = "SmartSqlMapConfig.xml";
        public string Alias { get; private set; } = DEFAULT_ALIAS;
        public ILoggerFactory LoggerFactory { get; } = NullLoggerFactory.Instance;
        public IConfigBuilder ConfigBuilder { get; }
        public SmartSqlConfig SmartSqlConfig { get; private set; }
        public IDbSessionFactory DbSessionFactory { get; private set; }
        public ISqlMapper SqlMapper { get; private set; }
        public bool Built { get; private set; }

        private SmartSqlBuilder(IConfigBuilder configBuilder, ILoggerFactory loggerFactory = null)
        {
            if (loggerFactory != null)
            {
                LoggerFactory = loggerFactory;
            }
            ConfigBuilder = configBuilder;
        }

        public SmartSqlBuilder Build()
        {
            if (Built) return this;
            Built = true;
            BeforeBuildInitService();
            DbSessionFactory = SmartSqlConfig.DbSessionFactory;
            SqlMapper = new SqlMapper(SmartSqlConfig);
            SmartSqlContainer.Instance.TryRegister(Alias, this);
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
            SmartSqlConfig.LoggerFactory = LoggerFactory;
            SmartSqlConfig.DataSourceFilter = _dataSourceFilter ?? new DataSourceFilter(SmartSqlConfig.LoggerFactory);
            if (_isCacheEnabled.HasValue)
            {
                SmartSqlConfig.Settings.IsCacheEnabled = _isCacheEnabled.Value;
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

        private IDataSourceFilter _dataSourceFilter;
        private bool? _isCacheEnabled;
        private IEnumerable<KeyValuePair<string, string>> _importProperties;

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
            if (String.IsNullOrEmpty(alias))
            {
                throw new ArgumentNullException(nameof(alias));
            }
            Alias = alias;
            return this;
        }
        public SmartSqlBuilder UseProperties(IEnumerable<KeyValuePair<string, string>> importProperties)
        {
            _importProperties = importProperties; return this;
        }
        #endregion

        #region Static Config
        public static SmartSqlBuilder AddConfigBuilder(IConfigBuilder configBuilder, ILoggerFactory loggerFactory = null)
        {
            if (configBuilder == null)
            {
                throw new ArgumentNullException(nameof(configBuilder));
            }
            return new SmartSqlBuilder(configBuilder, loggerFactory);
        }
        /// <summary>
        /// SmartSqlMapConfig 配置方式构建
        /// </summary>
        /// <param name="smartSqlConfig"></param>
        /// <returns></returns>
        public static SmartSqlBuilder AddNativeConfig(SmartSqlConfig smartSqlConfig, ILoggerFactory loggerFactory = null)
        {
            if (smartSqlConfig == null)
            {
                throw new ArgumentNullException(nameof(smartSqlConfig));
            }
            var configBuilder = new NativeConfigBuilder(smartSqlConfig);
            return AddConfigBuilder(configBuilder, loggerFactory);
        }
        /// <summary>
        /// Xml 配置方式构建
        /// </summary>
        /// <param name="smartSqlMapConfig"></param>
        /// <returns></returns>
        public static SmartSqlBuilder AddXmlConfig(ResourceType resourceType = ResourceType.File
            , String smartSqlMapConfig = DEFAULT_SMARTSQL_CONFIG_PATH
            , ILoggerFactory loggerFactory = null)
        {
            var configBuilder = new XmlConfigBuilder(resourceType, smartSqlMapConfig, loggerFactory);
            return AddConfigBuilder(configBuilder, loggerFactory);
        }
        /// <summary>
        /// 数据源方式构建
        /// </summary>
        /// <param name="writeDataSource"></param>
        /// <returns></returns>
        public static SmartSqlBuilder AddDataSource(WriteDataSource writeDataSource, ILoggerFactory loggerFactory = null)
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
            return AddNativeConfig(smartSqlConfig, loggerFactory);
        }

        public static SmartSqlBuilder AddDataSource(String dbProviderName, String connectionString, ILoggerFactory loggerFactory = null)
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
            return AddDataSource(writeDataSource, loggerFactory);
        }
        #endregion
    }
}
