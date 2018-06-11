using Microsoft.Extensions.Logging;
using SmartSql.Abstractions;
using SmartSql.Abstractions.Command;
using SmartSql.Abstractions.Config;
using SmartSql.Abstractions.DataReaderDeserializer;
using SmartSql.Abstractions.DataSource;
using SmartSql.Abstractions.DbSession;
using SmartSql.Abstractions.TypeHandler;
using SmartSql.Cache;
using SmartSql.Cahce;
using SmartSql.Command;
using SmartSql.Configuration;
using SmartSql.DataReaderDeserializer;
using SmartSql.DbSession;
using SmartSql.Logging;
using System;

namespace SmartSql
{
    public class SmartSqlOptions
    {
        public String ConfigPath { get; set; } = "SmartSqlMapConfig.xml";
        public ILoggerFactory LoggerFactory { get; set; }
        public IConfigLoader ConfigLoader { get; set; }
        public IDbConnectionSessionStore DbSessionStore { get; set; }
        public IDbConnectionSession DbSession { get; set; }
        public IDataSourceFilter DataSourceFilter { get; set; }
        public ISqlBuilder SqlBuilder { get; set; }
        public IPreparedCommand PreparedCommand { get; set; }
        public ICommandExecuter CommandExecuter { get; set; }
        public IDataReaderDeserializerFactory DataReaderDeserializerFactory { get; set; }
        public Abstractions.Cache.ICacheManager CacheManager { get; set; }
        public SmartSqlContext SmartSqlContext { get; internal set; }

        public void Setup()
        {
            if (String.IsNullOrEmpty(ConfigPath))
            {
                ConfigPath = "SmartSqlMapConfig.xml";
            }
            if (LoggerFactory == null)
            {
                LoggerFactory = NoneLoggerFactory.Instance;
            }

            if (ConfigLoader == null)
            {
                ConfigLoader = new LocalFileConfigLoader(ConfigPath, LoggerFactory);
            }
            var sqlMapConfig = ConfigLoader.Load();
            SmartSqlContext = new SmartSqlContext(LoggerFactory.CreateLogger<SmartSqlContext>(), sqlMapConfig);
            if (DbSessionStore == null)
            {
                var dbProviderFactory = DbProviderFactoryFactory.Create(SmartSqlContext.DbProvider.Type);
                DbSessionStore = new DbConnectionSessionStore(LoggerFactory, dbProviderFactory);
            }
            if (DataSourceFilter == null)
            {
                DataSourceFilter = new DataSourceFilter(LoggerFactory.CreateLogger<DataSourceFilter>(), DbSessionStore, SmartSqlContext);
            }
            if (SqlBuilder == null)
            {
                SqlBuilder = new SqlBuilder(LoggerFactory.CreateLogger<SqlBuilder>(), SmartSqlContext);
            }

            if (PreparedCommand == null)
            {
                PreparedCommand = new PreparedCommand(SmartSqlContext);
            }
            if (CommandExecuter == null)
            {
                CommandExecuter = new CommandExecuter(LoggerFactory.CreateLogger<CommandExecuter>(), PreparedCommand);
            }
            if (DataReaderDeserializerFactory == null)
            {
                DataReaderDeserializerFactory = new EmitDataReaderDeserializerFactory();
            }
            if (CacheManager == null)
            {
                if (SmartSqlContext.IsCacheEnabled)
                {
                    CacheManager = new CacheManager(LoggerFactory.CreateLogger<CacheManager>(), SmartSqlContext, DbSessionStore);
                }
                else
                {
                    CacheManager = new NoneCacheManager();
                }
            }
            ConfigLoader.OnChanged += ConfigLoader_OnChanged;

        }

        private void ConfigLoader_OnChanged(object sender, OnChangedEventArgs eventArgs)
        {
            SmartSqlContext.SqlMapConfig = eventArgs.SqlMapConfig;
            SmartSqlContext.Setup();
        }
    }
}
