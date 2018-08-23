using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using Microsoft.Extensions.Logging;
using SmartSql.Abstractions.Config;
using SmartSql.Configuration;
using SmartSql.Configuration.Maps;
using SmartSql.Utils;

namespace SmartSql.Options
{
    public class OptionConfigLoader : ConfigLoader
    {
        private readonly ILogger _logger;
        private SmartSqlConfigOptions _options;
        private const int DELAYED_LOAD_FILE = 500;
        private FileWatcherLoader _fileWatcherLoader;

        public override event OnChangedHandler OnChanged;

        public OptionConfigLoader(SmartSqlConfigOptions options, ILoggerFactory loggerFactory)
        {
            _options = options;
            _logger = loggerFactory.CreateLogger<OptionConfigLoader>();
            _fileWatcherLoader = new FileWatcherLoader();
        }

        public override void Dispose()
        {
        }

        public void TriggerChanged(SmartSqlConfigOptions options)
        {
            _options = options;
            var newConfig = Load();
            OnChanged?.Invoke(this, new OnChangedEventArgs
            {
                EventType = EventType.ConfigChanged,
                SqlMapConfig = newConfig
            });
        }

        public override SmartSqlMapConfig Load()
        {
            SqlMapConfig = new SmartSqlMapConfig()
            {
                SmartSqlMaps = new List<SmartSqlMap>(),
                SmartSqlMapSources = _options.SmartSqlMaps,
                Database = new Configuration.Database()
                {
                    DbProvider = _options.Database.DbProvider,
                    WriteDataSource = _options.Database.Write,
                    ReadDataSources = _options.Database.Read
                },
                Settings = _options.Settings,
                TypeHandlers = _options.TypeHandlers,
            };

            foreach (var sqlMapSource in SqlMapConfig.SmartSqlMapSources)
            {
                switch (sqlMapSource.Type)
                {
                    case SmartSqlMapSource.ResourceType.File:
                        {
                            LoadSmartSqlMap(SqlMapConfig, sqlMapSource.Path);
                            break;
                        }
                    case SmartSqlMapSource.ResourceType.Directory:
                    case SmartSqlMapSource.ResourceType.DirectoryWithAllSub:
                        {
                            SearchOption searchOption = SearchOption.TopDirectoryOnly;
                            if (sqlMapSource.Type == SmartSqlMapSource.ResourceType.DirectoryWithAllSub)
                            {
                                searchOption = SearchOption.AllDirectories;
                            }
                            var dicPath = Path.Combine(AppContext.BaseDirectory, sqlMapSource.Path);
                            var childSqlmapSources = Directory.EnumerateFiles(dicPath, "*.xml", searchOption);
                            foreach (var childSqlmapSource in childSqlmapSources)
                            {
                                LoadSmartSqlMap(SqlMapConfig, childSqlmapSource);
                            }
                            break;
                        }
                    default:
                        {
                            if (_logger.IsEnabled(LogLevel.Debug))
                            {
                                _logger.LogDebug($"OptionConfigLoader unknow SmartSqlMapSource.ResourceType:{sqlMapSource.Type}.");
                            }
                            break;
                        }
                }
            }

            if (_logger.IsEnabled(LogLevel.Debug))
            {
                _logger.LogDebug($"OptionConfigLoader Load End");
            }

            if (SqlMapConfig.Settings.IsWatchConfigFile)
            {
                if (_logger.IsEnabled(LogLevel.Debug))
                {
                    _logger.LogDebug($"OptionConfigLoader Load Add WatchConfig Starting.");
                }
                WatchConfig(SqlMapConfig);
                if (_logger.IsEnabled(LogLevel.Debug))
                {
                    _logger.LogDebug($"OptionConfigLoader Load Add WatchConfig End.");
                }
            }
            return SqlMapConfig;
        }

        private SmartSqlMap LoadSmartSqlMap(SmartSqlMapConfig config, string sqlMapPath)
        {
            if (_logger.IsEnabled(LogLevel.Debug))
            {
                _logger.LogDebug($"LoadSmartSqlMap Load: {sqlMapPath}");
            }
            var sqlmapStream = LoadConfigStream(sqlMapPath);
            var sqlmap = LoadSmartSqlMap(sqlmapStream);
            config.SmartSqlMaps.Add(sqlmap);

            return sqlmap;
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

        /// <summary>
        /// 监控配置文件-热更新
        /// </summary>
        private void WatchConfig(SmartSqlMapConfig config)
        {
            #region SmartSqlMaps File Watch

            foreach (var sqlmap in SqlMapConfig.SmartSqlMaps)
            {
                #region SqlMap File Watch

                if (_logger.IsEnabled(LogLevel.Debug))
                {
                    _logger.LogDebug($"LocalFileConfigLoader Watch SmartSqlMap: {sqlmap.Path} .");
                }
                var sqlMapFileInfo = FileLoader.GetInfo(sqlmap.Path);
                _fileWatcherLoader.Watch(sqlMapFileInfo, () =>
                {
                    Thread.Sleep(DELAYED_LOAD_FILE);
                    lock (this)
                    {
                        try
                        {
                            if (_logger.IsEnabled(LogLevel.Debug))
                            {
                                _logger.LogDebug($"LocalFileConfigLoader Changed Reload SmartSqlMap: {sqlmap.Path} Starting");
                            }
                            var newSqlMap = LoadSmartSqlMap(config, sqlmap.Path);
                            var oldSqlMap = SqlMapConfig.SmartSqlMaps.First(s => s.Path == sqlmap.Path);
                            oldSqlMap.Caches = newSqlMap.Caches;
                            oldSqlMap.Scope = newSqlMap.Scope;
                            oldSqlMap.Statements = newSqlMap.Statements;
                            OnChanged?.Invoke(this, new OnChangedEventArgs
                            {
                                SqlMapConfig = SqlMapConfig,
                                SqlMap = oldSqlMap,
                                EventType = EventType.SqlMapChangeed
                            });
                            if (_logger.IsEnabled(LogLevel.Debug))
                            {
                                _logger.LogDebug($"LocalFileConfigLoader Changed Reload SmartSqlMap: {sqlmap.Path} End");
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(new EventId(ex.HResult), ex, ex.Message);
                        }
                    }
                });

                #endregion SqlMap File Watch
            }

            #endregion SmartSqlMaps File Watch
        }
    }
}