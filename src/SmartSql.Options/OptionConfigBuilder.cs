using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using SmartSql.ConfigBuilder;
using SmartSql.Configuration;
using SmartSql.DataSource;
using SmartSql.Exceptions;
using SmartSql.Reflection;
using System;
using System.Linq;
using SmartSql.AutoConverter;

namespace SmartSql.Options
{
    public class OptionConfigBuilder : AbstractConfigBuilder
    {
        private readonly SmartSqlConfigOptions _configOptions;
        
        private readonly IWordsConverterBuilder _wordsConverterBuilder = new WordsConverterBuilder();
        private readonly ITokenizerBuilder _tokenizerBuilder = new TokenizerBuilder();
        public OptionConfigBuilder(SmartSqlConfigOptions configOptions, ILoggerFactory loggerFactory = null)
        {
            loggerFactory = loggerFactory ?? NullLoggerFactory.Instance;
            Logger = loggerFactory.CreateLogger<XmlConfigBuilder>();
            _configOptions = configOptions;
        }
        protected override void OnBeforeBuild()
        {
            if (Logger.IsEnabled(LogLevel.Debug))
            {
                Logger.LogDebug($"OptionConfigBuilder Build  Starting.");
            }
        }
        protected override void OnAfterBuild()
        {
            if (Logger.IsEnabled(LogLevel.Debug))
            {
                Logger.LogDebug($"OptionConfigBuilder Build  End.");
            }
        }

        protected override void BuildSqlMaps()
        {
            foreach (var sqlMapSource in _configOptions.SmartSqlMaps)
            {
                var resourceType = sqlMapSource.Type;
                var path = sqlMapSource.Path;
                if (Logger.IsEnabled(LogLevel.Debug))
                {
                    Logger.LogDebug($"XmlConfigBuilder BuildSqlMap ->> ResourceType:[{resourceType}] , Path :[{path}] Starting.");
                }
                BuildSqlMap(resourceType, path);
                if (Logger.IsEnabled(LogLevel.Debug))
                {
                    Logger.LogDebug($"XmlConfigBuilder BuildSqlMap ->> ResourceType:[{resourceType}] , Path :[{path}] End.");
                }
            }
        }

        protected override void BuildTagBuilders()
        {
            foreach (var tagBuilder in _configOptions.TagBuilders)
            {
                RegisterTagBuilder(tagBuilder.Name, tagBuilder.Type);
            }
        }

        protected override void BuildTypeHandlers()
        {
            foreach (var typeHandler in _configOptions.TypeHandlers)
            {
                var typeHandlerConfig = new Configuration.TypeHandler
                {
                    Name = typeHandler.Name,
                    Properties = typeHandler.Properties,
                    HandlerType = TypeUtils.GetType(typeHandler.Type)
                };

                if (typeHandlerConfig.HandlerType.IsGenericType)
                {
                    if (!String.IsNullOrEmpty(typeHandler.PropertyType))
                    {
                        typeHandlerConfig.PropertyType = TypeUtils.GetType(typeHandler.PropertyType);
                    }
                    if (!String.IsNullOrEmpty(typeHandler.FieldType))
                    {
                        typeHandlerConfig.FieldType = TypeUtils.GetType(typeHandler.FieldType);
                    }
                }
                RegisterTypeHandler(typeHandlerConfig);
            }
        }

        protected override void BuildProperties()
        {
            SmartSqlConfig.Properties.Import(_configOptions.Properties);
        }

        protected override void BuildIdGenerators()
        {
            if (_configOptions.IdGenerators == null) { return; }
            SmartSqlConfig.IdGenerators.Clear();
            foreach (var idGenConfig in _configOptions.IdGenerators)
            {
                var idGen = IdGeneratorBuilder.Build(idGenConfig.Type, idGenConfig.Properties);
                SmartSqlConfig.IdGenerators.Add(idGenConfig.Name, idGen);
            }
        }

        protected override void BuildDatabase()
        {
            var dbProvider = _configOptions.Database.DbProvider;
            DbProviderManager.Instance.TryInit(ref dbProvider);
            SmartSqlConfig.Database = new SmartSql.DataSource.Database
            {
                DbProvider = dbProvider,
                Write = new WriteDataSource
                {
                    Name = _configOptions.Database.Write.Name,
                    DbProvider = dbProvider,
                    ConnectionString = _configOptions.Database.Write.ConnectionString
                },
                Reads = _configOptions.Database.Reads.ToDictionary(r => r.Name, r => new ReadDataSource
                {
                    Name = r.Name,
                    ConnectionString = r.ConnectionString,
                    DbProvider = dbProvider,
                    Weight = r.Weight
                })
            };
        }
        private string GetExpString(string expStr)
        {
            return SmartSqlConfig.Properties.GetPropertyValue(expStr);
        }

        // todo: not impl
        protected override void BuildAutoConverters()
        {
            if (_configOptions.AutoConverterBuilders == null || !_configOptions.AutoConverterBuilders.Any())
            {
                return;
            }

            foreach (var autoConverterBuilder in _configOptions.AutoConverterBuilders)
            {
                if (String.IsNullOrEmpty(autoConverterBuilder.Name))
                {
                    throw new SmartSqlException("AutoConverterBuilder.Name can not be null");
                }
                
                if (String.IsNullOrEmpty(autoConverterBuilder.TokenizerName))
                {
                    throw new SmartSqlException("AutoConverterBuilder.TokenizerName can not be null");   
                }
                
                if (String.IsNullOrEmpty(autoConverterBuilder.WordsConverterName))
                {
                    throw new SmartSqlException("AutoConverterBuilder.WordsConverterName can not be null");
                }
                
                var tokenizer = _tokenizerBuilder.Build(autoConverterBuilder.TokenizerName, autoConverterBuilder.TokenizerProperties);
                var wordsConverter = _wordsConverterBuilder.Build(autoConverterBuilder.WordsConverterName, autoConverterBuilder.WordsConverterProperties);

                var autoConverter = new AutoConverter.AutoConverter(autoConverterBuilder.Name, tokenizer, wordsConverter);

                SmartSqlConfig.AutoConverters.Add(autoConverter.Name, autoConverter);
                if (autoConverterBuilder.IsDefault)
                {
                    SmartSqlConfig.DefaultAutoConverter = autoConverter;
                }
            }
        }

        public override void Dispose()
        {

        }

        protected override void BuildSettings()
        {
            SmartSqlConfig.Settings = _configOptions.Settings;
        }
    }
}
