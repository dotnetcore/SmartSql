using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SmartSql.Abstractions.Config;
using SmartSql.Configuration;
using SmartSql.Configuration.Maps;
using SmartSql.Utils;

namespace SmartSql.Options
{
    public class OptionConfigLoader : ConfigLoader
    {
        private readonly ILogger _logger;
        private readonly SmartSqlConfigOptions options;
        private const int DELAYED_LOAD_FILE = 500;
        private FileWatcherLoader _fileWatcherLoader;

        public override event OnChangedHandler OnChanged;

        public OptionConfigLoader(IOptionsSnapshot<SmartSqlConfigOptions> options, ILoggerFactory loggerFactory)
        {
            this.options = options.Value;
            this._logger = loggerFactory.CreateLogger<OptionConfigLoader>();
        }

        public override void Dispose()
        {
        }

        public override SmartSqlMapConfig Load()
        {
            SqlMapConfig = new SmartSqlMapConfig()
            {
                SmartSqlMaps = new List<SmartSqlMap>(),
                SmartSqlMapSources = options.SmartSqlMaps,
                Database = new Configuration.Database()
                {
                    DbProvider = options.Database.DbProvider,
                    WriteDataSource = options.Database.Write,
                    ReadDataSources = options.Database.Read
                },
                Settings = options.Settings,
                TypeHandlers = options.TypeHandlers,
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
                        {
                            var childSqlmapSources = Directory.EnumerateFiles(sqlMapSource.Path, "*.xml");
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