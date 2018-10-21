using SmartSql.Abstractions.Config;
using System;
using System.IO;
using Microsoft.Extensions.Logging;
using System.Threading;
using SmartSql.Configuration;
using System.Linq;
using SmartSql.Utils;
using SmartSql.Configuration.Maps;

namespace SmartSql
{
    /// <summary>
    /// 本地文件配置加载器
    /// </summary>
    public class LocalFileConfigLoader : ConfigLoader
    {
        private readonly ILogger _logger;
        private readonly string _sqlMapConfigFilePath;
        private const int DELAYED_LOAD_FILE = 500;
        private FileWatcherLoader _fileWatcherLoader;

        public override event OnChangedHandler OnChanged;

        public LocalFileConfigLoader(String sqlMapConfigFilePath, ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<LocalFileConfigLoader>();
            _sqlMapConfigFilePath = sqlMapConfigFilePath;
            _fileWatcherLoader = new FileWatcherLoader();
        }

        public override SmartSqlMapConfig Load()
        {
            if (_logger.IsEnabled(LogLevel.Debug))
            {
                _logger.LogDebug($"LocalFileConfigLoader Load: {_sqlMapConfigFilePath} Starting");
            }
            var configStream = LoadConfigStream(_sqlMapConfigFilePath);
            var config = LoadConfig(configStream);

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
            InitDependency();
            if (_logger.IsEnabled(LogLevel.Debug))
            {
                _logger.LogDebug($"LocalFileConfigLoader Load: {_sqlMapConfigFilePath} End");
            }

            if (config.Settings.IsWatchConfigFile)
            {
                if (_logger.IsEnabled(LogLevel.Debug))
                {
                    _logger.LogDebug($"LocalFileConfigLoader Load Add WatchConfig: {_sqlMapConfigFilePath} Starting.");
                }
                WatchConfig();
                if (_logger.IsEnabled(LogLevel.Debug))
                {
                    _logger.LogDebug($"LocalFileConfigLoader Load Add WatchConfig: {_sqlMapConfigFilePath} End.");
                }
            }
            return config;
        }

        private void LoadSmartSqlMapAndInConfig(string sqlMapPath)
        {
            var sqlMap = LoadSmartSqlMap(sqlMapPath);
            SqlMapConfig.SmartSqlMaps.Add(sqlMap);
        }

        private SmartSqlMap LoadSmartSqlMap(String sqlMapPath)
        {
            if (_logger.IsEnabled(LogLevel.Debug))
            {
                _logger.LogDebug($"LoadSmartSqlMap Load: {sqlMapPath}");
            }
            var sqlmapStream = LoadConfigStream(sqlMapPath);
            return LoadSmartSqlMap(sqlmapStream);
        }

        public ConfigStream LoadConfigStream(string path)
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
        private void WatchConfig()
        {
            #region SmartSqlMapConfig File Watch
            if (_logger.IsEnabled(LogLevel.Debug))
            {
                _logger.LogDebug($"LocalFileConfigLoader Watch SmartSqlMapConfig: {_sqlMapConfigFilePath} .");
            }
            var cofigFileInfo = FileLoader.GetInfo(_sqlMapConfigFilePath);
            _fileWatcherLoader.Watch(cofigFileInfo, () =>
            {
                Thread.Sleep(DELAYED_LOAD_FILE);
                lock (this)
                {
                    try
                    {
                        if (_logger.IsEnabled(LogLevel.Debug))
                        {
                            _logger.LogDebug($"LocalFileConfigLoader Changed ReloadConfig: {_sqlMapConfigFilePath} Starting");
                        }
                        SqlMapConfig = Load();
                        OnChanged?.Invoke(this, new OnChangedEventArgs
                        {
                            SqlMapConfig = SqlMapConfig,
                            EventType = EventType.ConfigChanged
                        });
                        if (_logger.IsEnabled(LogLevel.Debug))
                        {
                            _logger.LogDebug($"LocalFileConfigLoader Changed ReloadConfig: {_sqlMapConfigFilePath} End");
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(new EventId(ex.HResult), ex, ex.Message);
                    }
                }
            });
            #endregion
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
                            var newSqlMap = LoadSmartSqlMap(sqlmap.Path);
                            var oldSqlMap = SqlMapConfig.SmartSqlMaps.First(s => s.Path == sqlmap.Path);
                            oldSqlMap.Caches = newSqlMap.Caches;
                            oldSqlMap.Scope = newSqlMap.Scope;
                            oldSqlMap.Statements = newSqlMap.Statements;
                            InitDependency();
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
                #endregion
            }
            #endregion
        }

        public override void Dispose()
        {
            _fileWatcherLoader.Dispose();
        }
    }
}
