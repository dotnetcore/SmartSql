using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using SmartSql.Cache;
using SmartSql.ConfigBuilder;
using SmartSql.Configuration;
using SmartSql.DataSource;
using SmartSql.DbSession;
using SmartSql.Exceptions;
using SmartSql.Middlewares;
using SmartSql.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using SmartSql.Deserializer;
using SmartSql.TypeHandlers;

namespace SmartSql
{
    public class SmartSqlBuilder : IDisposable
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
        public ILoggerFactory LoggerFactory { get; private set; } = NullLoggerFactory.Instance;
        public IConfigBuilder ConfigBuilder { get; private set; }

        public SmartSqlConfig SmartSqlConfig { get; private set; }
        public IDbSessionFactory DbSessionFactory { get; private set; }
        public ISqlMapper SqlMapper { get; private set; }
        public bool Built { get; private set; }
        public bool Registered { get; private set; } = true;
        private readonly IList<IDataReaderDeserializer> _dataReaderDeserializers = new List<IDataReaderDeserializer>();

        public SmartSqlBuilder Build()
        {
            if (Built) return this;
            Built = true;
            BeforeBuildInitService();
            DbSessionFactory = SmartSqlConfig.DbSessionFactory;
            SqlMapper = new SqlMapper(SmartSqlConfig);
            if (Registered)
            {
                SmartSqlContainer.Instance.Register(this);
            }
            NamedTypeHandlerCache.Build(Alias, SmartSqlConfig.TypeHandlerFactory.GetNamedTypeHandlers());
            SetupSmartSql();
            return this;
        }

        private void SetupSmartSql()
        {
            foreach (var idGen in SmartSqlConfig.IdGenerators.Values)
            {
                if (idGen is ISetupSmartSql setupSmartSql)
                {
                    setupSmartSql.SetupSmartSql(this);
                }
            }
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
            SmartSqlConfig.Alias = Alias;
            SmartSqlConfig.LoggerFactory = LoggerFactory;
            SmartSqlConfig.DataSourceFilter = _dataSourceFilter ?? new DataSourceFilter(SmartSqlConfig.LoggerFactory);
            if (_isCacheEnabled.HasValue)
            {
                SmartSqlConfig.Settings.IsCacheEnabled = _isCacheEnabled.Value;
            }
            SmartSqlConfig.SqlParamAnalyzer = new SqlParamAnalyzer(SmartSqlConfig.Settings.IgnoreParameterCase, SmartSqlConfig.Database.DbProvider.ParameterPrefix);
            InitDeserializerFactory();
            BuildPipeline();
        }

        private void InitDeserializerFactory()
        {
            IDataReaderDeserializer deser = new MultipleResultDeserializer(SmartSqlConfig.DeserializerFactory);
            SmartSqlConfig.DeserializerFactory.Add(deser);
            deser = new ValueTupleDeserializer(SmartSqlConfig.DeserializerFactory);
            SmartSqlConfig.DeserializerFactory.Add(deser);
            deser = new ValueTypeDeserializer();
            SmartSqlConfig.DeserializerFactory.Add(deser);
            deser = new DynamicDeserializer();
            SmartSqlConfig.DeserializerFactory.Add(deser);
            foreach (var deserializer in _dataReaderDeserializers)
            {
                SmartSqlConfig.DeserializerFactory.Add(deserializer);
            }
        }

        private bool UsedCache => SmartSqlConfig.Settings.IsCacheEnabled
                                  && SmartSqlConfig.SqlMaps.Values.Any(sqlMap => sqlMap.Caches.Count > 0);

        private void BuildPipeline()
        {
            if (SmartSqlConfig.Pipeline != null)
            {
                return;
            }
            if (UsedCache)
            {
                SmartSqlConfig.CacheManager = new CacheManager(SmartSqlConfig);
                SmartSqlConfig.Pipeline = new PipelineBuilder()
                     .Add(new InitializerMiddleware(SmartSqlConfig))
                     .Add(new TransactionMiddleware())
                     .Add(new PrepareStatementMiddleware(SmartSqlConfig))
                     .Add(new CachingMiddleware(SmartSqlConfig))
                     .Add(new DataSourceFilterMiddleware(SmartSqlConfig))
                     .Add(new CommandExecuterMiddleware(SmartSqlConfig))
                     .Add(new ResultHandlerMiddleware(SmartSqlConfig)).Build();
            }
            else
            {
                SmartSqlConfig.CacheManager = new NoneCacheManager();
                SmartSqlConfig.Pipeline = new PipelineBuilder()
                     .Add(new InitializerMiddleware(SmartSqlConfig))
                     .Add(new TransactionMiddleware())
                     .Add(new PrepareStatementMiddleware(SmartSqlConfig))
                     .Add(new DataSourceFilterMiddleware(SmartSqlConfig))
                     .Add(new CommandExecuterMiddleware(SmartSqlConfig))
                     .Add(new ResultHandlerMiddleware(SmartSqlConfig)).Build();
            }
        }

        #region Instance

        private IDataSourceFilter _dataSourceFilter;
        private bool? _isCacheEnabled;
        private IEnumerable<KeyValuePair<string, string>> _importProperties;
        public SmartSqlBuilder RegisterToContainer(bool registered = true)
        {
            Registered = registered; return this;
        }

        public SmartSqlBuilder AddDeserializer(IDataReaderDeserializer deserializer)
        {
            _dataReaderDeserializers.Add(deserializer); return this;
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
        public SmartSqlBuilder UseLoggerFactory(ILoggerFactory loggerFactory)
        {
            if (loggerFactory != null)
            {
                LoggerFactory = loggerFactory;
            }
            return this;
        }
        public SmartSqlBuilder UseConfigBuilder(IConfigBuilder configBuilder)
        {
            ConfigBuilder = configBuilder;
            return this;
        }
        /// <summary>
        /// SmartSqlMapConfig 配置方式构建
        /// </summary>
        /// <param name="smartSqlConfig"></param>
        /// <returns></returns>
        public SmartSqlBuilder UseNativeConfig(SmartSqlConfig smartSqlConfig)
        {
            if (smartSqlConfig == null)
            {
                throw new ArgumentNullException(nameof(smartSqlConfig));
            }
            ConfigBuilder = new NativeConfigBuilder(smartSqlConfig);
            return this;
        }

        /// <summary>
        /// Xml 配置方式构建
        /// </summary>
        /// <param name="resourceType"></param>
        /// <param name="smartSqlMapConfig"></param>
        /// <returns></returns>
        public SmartSqlBuilder UseXmlConfig(ResourceType resourceType = ResourceType.File
            , String smartSqlMapConfig = DEFAULT_SMARTSQL_CONFIG_PATH)
        {
            ConfigBuilder = new XmlConfigBuilder(resourceType, smartSqlMapConfig, LoggerFactory);
            return this;
        }
        /// <summary>
        /// 数据源方式构建
        /// </summary>
        /// <param name="writeDataSource"></param>
        /// <returns></returns>
        public SmartSqlBuilder UseDataSource(WriteDataSource writeDataSource)
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
            return UseNativeConfig(smartSqlConfig);
        }

        public SmartSqlBuilder UseDataSource(String dbProviderName, String connectionString)
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
            return UseDataSource(writeDataSource);
        }

        public void Dispose()
        {
            SmartSqlConfig.SessionStore.Dispose();
            SmartSqlConfig.CacheManager.Dispose();
        }
        #endregion
    }
}
