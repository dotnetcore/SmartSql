using SmartSql.Abstractions;
using SmartSql.Abstractions.Config;
using System;
using System.Collections.Generic;
using System.Text;
using SmartSql.SqlMap;
using SmartSql.Common;
using System.Xml.Serialization;
using System.Xml;
using System.IO;
using Microsoft.Extensions.Logging;
using System.Threading;

namespace SmartSql
{
    /// <summary>
    /// 本地文件配置加载器
    /// </summary>
    public class LocalFileConfigLoader : ConfigLoader
    {
        private readonly ILogger _logger;
        private readonly string sqlMapConfigFilePath;
        private const int DELAYED_LOAD_FILE = 500;

        public override Action<ConfigChangedEvent> OnChanged { get; set; }
        public override SmartSqlMapConfig SqlMapConfig { get; protected set; }

        public LocalFileConfigLoader(String sqlMapConfigFilePath, ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<LocalFileConfigLoader>();
            this.sqlMapConfigFilePath = sqlMapConfigFilePath;
        }

        public override SmartSqlMapConfig Load()
        {
            _logger.LogDebug($"LocalFileConfigLoader Load: {sqlMapConfigFilePath} Starting");
            var configStream = LoadConfigStream(sqlMapConfigFilePath);
            var config = LoadConfig(configStream);

            foreach (var sqlmapSource in config.SmartSqlMapSources)
            {
                switch (sqlmapSource.Type)
                {
                    case SmartSqlMapSource.ResourceType.File:
                        {
                            LoadSmartSqlMap(sqlmapSource.Path);
                            break;
                        }
                    case SmartSqlMapSource.ResourceType.Directory:
                        {
                            var childSqlmapSources = Directory.EnumerateFiles(sqlmapSource.Path, "*.xml");
                            foreach (var childSqlmapSource in childSqlmapSources)
                            {
                                LoadSmartSqlMap(childSqlmapSource);
                            }
                            break;
                        }
                    default:
                        {
                            _logger.LogDebug($"LocalFileConfigLoader unknow SmartSqlMapSource.ResourceType:{sqlmapSource.Type}.");
                            break;
                        }
                }
            }
            _logger.LogDebug($"LocalFileConfigLoader Load: {sqlMapConfigFilePath} End");

            if (config.Settings.IsWatchConfigFile)
            {
                _logger.LogDebug($"LocalFileConfigLoader Load Add WatchConfig: {sqlMapConfigFilePath} Starting.");
                WatchConfig();
                _logger.LogDebug($"LocalFileConfigLoader Load Add WatchConfig: {sqlMapConfigFilePath} End.");
            }
            return config;
        }

        private void LoadSmartSqlMap(String sqlmapSourcePath)
        {
            _logger.LogDebug($"LoadSmartSqlMap Load: {sqlmapSourcePath}");
            var sqlmapStream = LoadConfigStream(sqlmapSourcePath);
            var sqlmap = LoadSmartSqlMap(sqlmapStream);
            SqlMapConfig.SmartSqlMaps.Add(sqlmap);
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
            _logger.LogDebug($"LocalFileConfigLoader Watch SmartSqlMapConfig: {sqlMapConfigFilePath} .");
            var cofigFileInfo = FileLoader.GetInfo(sqlMapConfigFilePath);
            FileWatcherLoader.Instance.Watch(cofigFileInfo, () =>
            {
                Thread.Sleep(DELAYED_LOAD_FILE);
                lock (this)
                {
                    try
                    {
                        _logger.LogDebug($"LocalFileConfigLoader Changed ReloadConfig: {sqlMapConfigFilePath} Starting");
                        var newConfig = Load();
                        OnChanged?.Invoke(new ConfigChangedEvent
                        {
                            SqlMapConfig = newConfig,
                            EventType = EventType.ConfigChanged
                        });
                        _logger.LogDebug($"LocalFileConfigLoader Changed ReloadConfig: {sqlMapConfigFilePath} End");
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
                _logger.LogDebug($"LocalFileConfigLoader Watch SmartSqlMap: {sqlmap.Path} .");
                var sqlMapFileInfo = FileLoader.GetInfo(sqlmap.Path);
                FileWatcherLoader.Instance.Watch(sqlMapFileInfo, () =>
                {
                    Thread.Sleep(DELAYED_LOAD_FILE);
                    lock (this)
                    {
                        try
                        {
                            _logger.LogDebug($"LocalFileConfigLoader Changed Reload SmartSqlMap: {sqlmap.Path} Starting");
                            var sqlmapStream = LoadConfigStream(sqlmap.Path);
                            var newSqlmap = LoadSmartSqlMap(sqlmapStream);
                            sqlmap.Scope = newSqlmap.Scope;
                            sqlmap.Statements = newSqlmap.Statements;
                            sqlmap.Caches = newSqlmap.Caches;
                            SqlMapConfig.ResetMappedStatements();
                            OnChanged?.Invoke(new ConfigChangedEvent
                            {
                                SqlMap = sqlmap,
                                EventType = EventType.SqlMapChangeed
                            });
                            _logger.LogDebug($"LocalFileConfigLoader Changed Reload SmartSqlMap: {sqlmap.Path} End");
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
            FileWatcherLoader.Instance.Clear();
        }
    }
}
