using SmartSql.Abstractions;
using SmartSql.Abstractions.Config;
using System;
using System.Collections.Generic;
using System.Text;
using SmartSql.SqlMap;
using SmartSql.Abstractions.Logging;
using SmartSql.Common;
using System.Xml.Serialization;
using System.Xml;

namespace SmartSql
{
    /// <summary>
    /// 本地文件配置加载器
    /// </summary>
    public class LocalFileConfigLoader : ConfigLoader
    {
        private static readonly ILog _logger = LogManager.GetLogger(typeof(LocalFileConfigLoader));

        public override SmartSqlMapConfig Load(String path, ISmartSqlMapper smartSqlMapper)
        {
            _logger.Debug($"SmartSql.LocalFileConfigLoader Load: {path} Starting");
            var configStream = LoadConfigStream(path);
            var config = LoadConfig(configStream, smartSqlMapper);

            foreach (var sqlmapSource in config.SmartSqlMapSources)
            {
                _logger.Debug($"SmartSql.LoadSmartSqlMap Load: {sqlmapSource.Path}");
                var sqlmapStream = LoadConfigStream(sqlmapSource.Path);
                var sqlmap = LoadSmartSqlMap(sqlmapStream, config);
                config.SmartSqlMaps.Add(sqlmap);
            }
            _logger.Debug($"SmartSql.LocalFileConfigLoader Load: {path} End");

            smartSqlMapper.LoadConfig(config);

            if (config.Settings.IsWatchConfigFile)
            {
                _logger.Debug($"SmartSql.LocalFileConfigLoader Load Add WatchConfig: {path} .");
                WatchConfig(smartSqlMapper);
            }
            return config;
        }

        public  ConfigStream LoadConfigStream(string path)
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
        /// <param name="smartSqlMapper"></param>
        /// <param name="config"></param>
        private void WatchConfig(ISmartSqlMapper smartSqlMapper)
        {
            var config = smartSqlMapper.SqlMapConfig;
            #region SmartSqlMapConfig File Watch
            var cofigFileInfo = FileLoader.GetInfo(config.Path);
            FileWatcherLoader.Instance.Watch(cofigFileInfo, () =>
            {
                _logger.Debug($"SmartSql.LocalFileConfigLoader Changed ReloadConfig: {config.Path} Starting");
                var newConfig = Load(config.Path, smartSqlMapper);
                _logger.Debug($"SmartSql.LocalFileConfigLoader Changed ReloadConfig: {config.Path} End");
            });
            #endregion
            #region SmartSqlMaps File Watch
            foreach (var sqlmap in config.SmartSqlMaps)
            {
                #region SqlMap File Watch
                var sqlMapFileInfo = FileLoader.GetInfo(sqlmap.Path);
                FileWatcherLoader.Instance.Watch(sqlMapFileInfo, () =>
                {
                    _logger.Debug($"SmartSql.LocalFileConfigLoader Changed Reload SmartSqlMap: {sqlmap.Path} Starting");
                    var sqlmapStream = LoadConfigStream(sqlmap.Path);
                    var newSqlmap = LoadSmartSqlMap(sqlmapStream, config);
                    sqlmap.Scope = newSqlmap.Scope;
                    sqlmap.Statements = newSqlmap.Statements;
                    sqlmap.Caches = newSqlmap.Caches;
                    config.ResetMappedStatements();
                    smartSqlMapper.CacheManager.ResetMappedCaches();
                    _logger.Debug($"SmartSql.LocalFileConfigLoader Changed Reload SmartSqlMap: {sqlmap.Path} End");
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
