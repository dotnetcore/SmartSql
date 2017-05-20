using SmartSql.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;
using SmartSql.SqlMap;
using SmartSql.Abstractions.Logging;
using SmartSql.Common;
using System.Xml.Serialization;

namespace SmartSql
{
    public class LocalFileConfigLoader : IConfigLoader
    {
        private static readonly ILog _logger = LogManager.GetLogger(typeof(LocalFileConfigLoader));

        public SmartSqlMapConfig Load(String filePath, ISmartSqlMapper smartSqlMapper)
        {
            _logger.Debug($"SmartSql.SmartSqlMapConfig Load: {filePath} Starting");
            var config = LoadConfig(filePath, smartSqlMapper);
            _logger.Debug($"SmartSql.SmartSqlMapConfig Load: {filePath} End");
            smartSqlMapper.LoadConfig(config);
            if (config.Settings.IsWatchConfigFile)
            {
                _logger.Debug($"SmartSql.SmartSqlMapConfig.Load Add WatchConfig: {filePath} .");
                WatchConfig(smartSqlMapper);
            }
            return config;
        }

        private SmartSqlMapConfig LoadConfig(String path, ISmartSqlMapper smartSqlMapper)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(SmartSqlMapConfig));
            SmartSqlMapConfig config = null;
            using (var configFile = FileLoader.Load(path))
            {
                config = xmlSerializer.Deserialize(configFile) as SmartSqlMapConfig;
                config.FilePath = path;
                config.SmartSqlMapper = smartSqlMapper;
            }
            config.SmartSqlMaps = new List<SmartSqlMap> { };
            foreach (var sqlmapSource in config.SmartSqlMapSources)
            {
                var sqlmap = SmartSqlMap.Load(sqlmapSource.Path, config);
                config.SmartSqlMaps.Add(sqlmap);
            }
            return config;
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
            var cofigFileInfo = FileLoader.GetInfo(config.FilePath);
            FileWatcherLoader.Instance.Watch(cofigFileInfo, () =>
            {
                _logger.Debug($"SmartSql.SmartSqlMapConfig Changed ReloadConfig: {config.FilePath} Starting");
                var newConfig = Load(config.FilePath, smartSqlMapper);
                smartSqlMapper.LoadConfig(newConfig);
                _logger.Debug($"SmartSql.SmartSqlMapConfig Changed ReloadConfig: {config.FilePath} End");
            });
            #endregion
            #region SmartSqlMaps File Watch
            foreach (var sqlmap in config.SmartSqlMaps)
            {
                #region SqlMap File Watch
                var sqlMapFileInfo = FileLoader.GetInfo(sqlmap.FilePath);
                FileWatcherLoader.Instance.Watch(sqlMapFileInfo, () =>
                {
                    _logger.Debug($"SmartSql.SmartSqlMapConfig Changed Reload SmartSqlMap: {sqlmap.FilePath} Starting");
                    var newSqlmap = SmartSqlMap.Load(sqlmap.FilePath, config);
                    sqlmap.Scope = newSqlmap.Scope;
                    sqlmap.Statements = newSqlmap.Statements;
                    config.ResetMappedStatements();
                    _logger.Debug($"SmartSql.SmartSqlMapConfig Changed Reload SmartSqlMap: {sqlmap.FilePath} End");
                });
                #endregion
            }
            #endregion
        }

    }
}
