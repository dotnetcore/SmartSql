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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SmartSql.Command;
using SmartSql.Deserializer;
using SmartSql.Filters;
using SmartSql.Reflection.TypeConstants;
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
        public ICacheManager CacheManager { get; private set; }
        public IDataSourceFilter DataSourceFilter { get; private set; }
        public ICommandExecuter CommandExecuter { get; private set; }
        public bool? IsCacheEnabled { get; private set; }
        public bool? IgnoreDbNull { get; private set; }
        public bool Built { get; private set; }
        public bool Registered { get; private set; } = true;
        public IList<IDataReaderDeserializer> DataReaderDeserializers { get; } = new List<IDataReaderDeserializer>();
        public IList<TypeHandler> TypeHandlers { get; } = new List<TypeHandler>();
        public FilterCollection Filters { get; } = new FilterCollection();
        public Action<ExecutionContext> InvokeSucceeded { get; set; }

        public IList<KeyValuePair<string, string>> ImportProperties { get; } =
            new List<KeyValuePair<string, string>>();

        public IList<Type> EntityTypes { get; } = new List<Type>();

        public IList<IMiddleware> Middlewares { get; set; } = new List<IMiddleware>();

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
            InitEntityMetaDataCache();
            return this;
        }

        private void InitEntityMetaDataCache()
        {
            foreach (var entityType in EntityTypes)
            {
                EntityMetaDataCacheType.InitTypeHandler(entityType);
            }
        }

        private void SetupSmartSql()
        {
            #region IdGen

            foreach (var idGen in SmartSqlConfig.IdGenerators.Values)
            {
                if (idGen is ISetupSmartSql setupSmartSql)
                {
                    setupSmartSql.SetupSmartSql(this);
                }
            }

            #endregion

            #region CacheManager

            if (CacheManager is ISetupSmartSql cacheManager)
            {
                cacheManager.SetupSmartSql(this);
            }

            #endregion

            #region Filters

            foreach (var filter in Filters)
            {
                var setupSmartSql = filter as ISetupSmartSql;
                setupSmartSql?.SetupSmartSql(this);
            }

            #endregion

            #region Deserializer

            foreach (var deserializer in DataReaderDeserializers)
            {
                var setupSmartSql = deserializer as ISetupSmartSql;
                setupSmartSql?.SetupSmartSql(this);
            }

            #endregion

            #region Pipeline

            var currentMiddleware = SmartSqlConfig.Pipeline;
            while (currentMiddleware != null)
            {
                if (currentMiddleware is ISetupSmartSql setupSmartSql)
                {
                    setupSmartSql.SetupSmartSql(this);
                }

                currentMiddleware = currentMiddleware.Next;
            }

            #endregion
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
            var rootConfigBuilder = new RootConfigBuilder(ImportProperties);

            if (ConfigBuilder.Parent == null)
            {
                ConfigBuilder.SetParent(rootConfigBuilder);
            }

            SmartSqlConfig = ConfigBuilder.Build();
            SmartSqlConfig.Alias = Alias;
            SmartSqlConfig.LoggerFactory = LoggerFactory;
            SmartSqlConfig.DataSourceFilter = DataSourceFilter ?? new DataSourceFilter(SmartSqlConfig.LoggerFactory);
            SmartSqlConfig.CommandExecuter =
                CommandExecuter ?? new CommandExecuter(LoggerFactory.CreateLogger<CommandExecuter>());
            if (IsCacheEnabled.HasValue)
            {
                SmartSqlConfig.Settings.IsCacheEnabled = IsCacheEnabled.Value;
            }

            if (IgnoreDbNull.HasValue)
            {
                SmartSqlConfig.Settings.IgnoreDbNull = IgnoreDbNull.Value;
            }

            if (InvokeSucceeded != null)
            {
                SmartSqlConfig.InvokeSucceedListener.InvokeSucceeded += (sender, args) =>
                {
                    InvokeSucceeded(args.ExecutionContext);
                };
            }

            SmartSqlConfig.SqlParamAnalyzer = new SqlParamAnalyzer(SmartSqlConfig.Settings.IgnoreParameterCase,
                SmartSqlConfig.Database.DbProvider.ParameterPrefix);
            SmartSqlConfig.CacheTemplateAnalyzer = new SqlParamAnalyzer(SmartSqlConfig.Settings.IgnoreParameterCase,
                SmartSqlConfig.Settings.ParameterPrefix);
            InitDeserializerFactory();
            InitFilters();
            InitTypeHandlers();
            BuildPipeline();
        }

        private void InitFilters()
        {
            foreach (var filter in Filters)
            {
                SmartSqlConfig.Filters.Add(filter);
            }
        }

        private void InitTypeHandlers()
        {
            foreach (var typeHandler in TypeHandlers)
            {
                SmartSqlConfig.TypeHandlerFactory.Register(typeHandler);
            }
        }

        private void InitDeserializerFactory()
        {
            IDataReaderDeserializer deser = new MultipleResultDeserializer(SmartSqlConfig.DeserializerFactory);
            DataReaderDeserializers.Insert(0, deser);
            deser = new ValueTupleDeserializer(SmartSqlConfig.DeserializerFactory);
            DataReaderDeserializers.Insert(1, deser);
            deser = new ValueTypeDeserializer();
            DataReaderDeserializers.Insert(2, deser);
            deser = new DynamicDeserializer();
            DataReaderDeserializers.Insert(3, deser);
            deser = new EntityDeserializer();
            DataReaderDeserializers.Insert(4, deser);
            foreach (var deserializer in DataReaderDeserializers)
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

            var pipelineBuilder = new PipelineBuilder();
            if (UsedCache)
            {
                if (CacheManager == null)
                {
                    CacheManager = new CacheManager();
                }

                SmartSqlConfig.CacheManager = CacheManager;
                pipelineBuilder.Add(new InitializerMiddleware())
                    .Add(new PrepareStatementMiddleware())
                    .Add(new CachingMiddleware())
                    .Add(new TransactionMiddleware())
                    .Add(new DataSourceFilterMiddleware())
                    .Add(new CommandExecuterMiddleware())
                    .Add(new ResultHandlerMiddleware()).Build();
            }
            else
            {
                SmartSqlConfig.CacheManager = new NoneCacheManager();
                pipelineBuilder.Add(new InitializerMiddleware())
                    .Add(new PrepareStatementMiddleware())
                    .Add(new TransactionMiddleware())
                    .Add(new DataSourceFilterMiddleware())
                    .Add(new CommandExecuterMiddleware())
                    .Add(new ResultHandlerMiddleware()).Build();
            }

            foreach (var middleware in Middlewares)
            {
                pipelineBuilder.Add(middleware);
            }

            SmartSqlConfig.Pipeline = pipelineBuilder.Build();
        }

        #region Instance

        public SmartSqlBuilder UseCommandExecuter(ICommandExecuter commandExecuter)
        {
            CommandExecuter = commandExecuter;
            return this;
        }

        public SmartSqlBuilder ListenInvokeSucceeded(Action<ExecutionContext> invokeSucceeded)
        {
            InvokeSucceeded = invokeSucceeded;
            return this;
        }

        public SmartSqlBuilder RegisterToContainer(bool registered = true)
        {
            Registered = registered;
            return this;
        }

        #region RegisterEntity

        public SmartSqlBuilder RegisterEntity(Type entityType)
        {
            EntityTypes.Add(entityType);
            return this;
        }

        public SmartSqlBuilder RegisterEntity(TypeScanOptions typeScanOptions)
        {
            var entityTypes = TypeScan.Scan(typeScanOptions);

            foreach (var entityType in entityTypes)
            {
                EntityTypes.Add(entityType);
            }

            return this;
        }

        #endregion

        public SmartSqlBuilder AddDeserializer(IDataReaderDeserializer deserializer)
        {
            DataReaderDeserializers.Add(deserializer);
            return this;
        }

        public SmartSqlBuilder AddTypeHandler(TypeHandler typeHandler)
        {
            TypeHandlers.Add(typeHandler);
            return this;
        }

        public SmartSqlBuilder AddFilter<TFilter>()
            where TFilter : IFilter, new()
        {
            Filters.Add<TFilter>();
            return this;
        }

        public SmartSqlBuilder AddFilter(IFilter filter)
        {
            Filters.Add(filter);
            return this;
        }

        public SmartSqlBuilder UseDataSourceFilter(IDataSourceFilter dataSourceFilter)
        {
            DataSourceFilter = dataSourceFilter;
            return this;
        }

        public SmartSqlBuilder UseCache(bool isCacheEnabled = true)
        {
            IsCacheEnabled = isCacheEnabled;
            return this;
        }

        public SmartSqlBuilder UseIgnoreDbNull(bool ignoreDbNull = false)
        {
            IgnoreDbNull = ignoreDbNull;
            return this;
        }

        public SmartSqlBuilder UseCacheManager(ICacheManager cacheManager)
        {
            CacheManager = cacheManager;
            return this;
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

        public SmartSqlBuilder AddMiddleware(IMiddleware middleware)
        {
            Middlewares.Add(middleware);
            return this;
        }

        #region UseProperties

        public SmartSqlBuilder UseProperties(IEnumerable<KeyValuePair<string, string>> importProperties)
        {
            foreach (var property in importProperties)
            {
                ImportProperties.Add(property);
            }

            return this;
        }

        public SmartSqlBuilder UseProperties(IDictionary dictionary)
        {
            foreach (DictionaryEntry envVar in dictionary)
            {
                ImportProperties.Add(new KeyValuePair<string, string>(envVar.Key.ToString(), envVar.Value.ToString()));
            }

            return this;
        }

        public SmartSqlBuilder UsePropertiesFromEnv(
            EnvironmentVariableTarget target = EnvironmentVariableTarget.Process)
        {
            var envVars = Environment.GetEnvironmentVariables(target);

            return UseProperties(envVars);
        }

        #endregion


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