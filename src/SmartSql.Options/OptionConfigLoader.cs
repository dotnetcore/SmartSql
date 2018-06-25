using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SmartSql.Abstractions.Config;
using SmartSql.Configuration.Maps;
using SmartSql.Configuration.Options;
using SmartSql.Utils;

namespace SmartSql.Configuration
{
    public class OptionConfigLoader : ConfigLoader
    {
        private readonly ILogger _logger;

        public override event OnChangedHandler OnChanged;

        private readonly SmartSqlConfigOptions options;

        public OptionConfigLoader(IOptionsSnapshot<SmartSqlConfigOptions> options, ILoggerFactory loggerFactory)
        {
            this.options = options.Value;
        }

        public override void Dispose()
        {
        }

        public override SmartSqlMapConfig Load()
        {
            var config = new SmartSqlMapConfig()
            {
                Path = options.Path,
                SmartSqlMaps = new List<SmartSqlMap>(),
                SmartSqlMapSources = new List<SmartSqlMapSource>()
                {
                    new SmartSqlMapSource()
                    {
                        Path = options.Path,
                        Type = SmartSqlMapSource.ResourceType.Directory
                    }
                },
                Database = new Database()
                {
                    DbProvider = options.Database.DbProvider,
                    WriteDataSource = options.Database.Write,
                    ReadDataSources = options.Database.Read
                },
                Settings = options.Settings
            };

            foreach (var sqlMapSource in config.SmartSqlMapSources)
            {
                switch (sqlMapSource.Type)
                {
                    case SmartSqlMapSource.ResourceType.File:
                        {
                            LoadSmartSqlMapAndInConfig(sqlMapSource.Path);
                            break;
                        }
                    case SmartSqlMapSource.ResourceType.Directory:
                        {
                            var childSqlmapSources = Directory.EnumerateFiles(sqlMapSource.Path, "*.xml");
                            foreach (var childSqlmapSource in childSqlmapSources)
                            {
                                LoadSmartSqlMapAndInConfig(childSqlmapSource);
                            }
                            break;
                        }
                    default:
                        {
                            if (_logger.IsEnabled(LogLevel.Debug))
                            {
                                _logger.LogDebug($"LocalFileConfigLoader unknow SmartSqlMapSource.ResourceType:{sqlMapSource.Type}.");
                            }
                            break;
                        }
                }
            }
            if (_logger.IsEnabled(LogLevel.Debug))
            {
                _logger.LogDebug($"ConfigurationConfigLoader Load End");
            }

            //if (config.Settings.IsWatchConfigFile)
            //{
            //    if (_logger.IsEnabled(LogLevel.Debug))
            //    {
            //        _logger.LogDebug($"ConfigurationConfigLoader Load Add WatchConfig Starting.");
            //    }

            //    if (_logger.IsEnabled(LogLevel.Debug))
            //    {
            //        _logger.LogDebug($"ConfigurationConfigLoader Load Add WatchConfig End.");
            //    }
            //}
            return config;
        }

        private void LoadSmartSqlMapAndInConfig(string sqlMapPath)
        {
            var sqlMap = LoadSmartSqlMap(sqlMapPath);
            SqlMapConfig.SmartSqlMaps.Add(sqlMap);
        }

        private SmartSqlMap LoadSmartSqlMap(string sqlMapPath)
        {
            if (_logger.IsEnabled(LogLevel.Debug))
            {
                _logger.LogDebug($"LoadSmartSqlMap Load: {sqlMapPath}");
            }
            var sqlmapStream = LoadConfigStream(sqlMapPath);
            return LoadSmartSqlMap(sqlmapStream);
        }

        private ConfigStream LoadConfigStream(string path)
        {
            var configStream = new ConfigStream
            {
                Path = path,
                Stream = FileLoader.Load(path)
            };
            return configStream;
        }
    }
}