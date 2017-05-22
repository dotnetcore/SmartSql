using SmartSql.Abstractions;
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
    public class LocalFileConfigLoader : IConfigLoader
    {
        private static readonly ILog _logger = LogManager.GetLogger(typeof(LocalFileConfigLoader));

        public SmartSqlMapConfig Load(String path, ISmartSqlMapper smartSqlMapper)
        {
            _logger.Debug($"SmartSql.SmartSqlMapConfig Load: {path} Starting");
            var config = LoadConfig(path, smartSqlMapper);
            _logger.Debug($"SmartSql.SmartSqlMapConfig Load: {path} End");
            smartSqlMapper.LoadConfig(config);
            if (config.Settings.IsWatchConfigFile)
            {
                _logger.Debug($"SmartSql.SmartSqlMapConfig.Load Add WatchConfig: {path} .");
                WatchConfig(smartSqlMapper);
            }
            return config;
        }

        private SmartSqlMapConfig LoadConfig(String path, ISmartSqlMapper smartSqlMapper)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(SmartSqlMapConfig));
            SmartSqlMapConfig config = null;
            using (var configStream = FileLoader.Load(path))
            {
                config = xmlSerializer.Deserialize(configStream) as SmartSqlMapConfig;
                config.Path = path;
                config.SmartSqlMapper = smartSqlMapper;
            }
            config.SmartSqlMaps = new List<SmartSqlMap> { };
            foreach (var sqlmapSource in config.SmartSqlMapSources)
            {
                var sqlmap = LoadSmartSqlMap(sqlmapSource.Path, config);
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
            var cofigFileInfo = FileLoader.GetInfo(config.Path);
            FileWatcherLoader.Instance.Watch(cofigFileInfo, () =>
            {
                _logger.Debug($"SmartSql.SmartSqlMapConfig Changed ReloadConfig: {config.Path} Starting");
                var newConfig = Load(config.Path, smartSqlMapper);
                _logger.Debug($"SmartSql.SmartSqlMapConfig Changed ReloadConfig: {config.Path} End");
            });
            #endregion
            #region SmartSqlMaps File Watch
            foreach (var sqlmap in config.SmartSqlMaps)
            {
                #region SqlMap File Watch
                var sqlMapFileInfo = FileLoader.GetInfo(sqlmap.Path);
                FileWatcherLoader.Instance.Watch(sqlMapFileInfo, () =>
                {
                    _logger.Debug($"SmartSql.SmartSqlMapConfig Changed Reload SmartSqlMap: {sqlmap.Path} Starting");
                    var newSqlmap = LoadSmartSqlMap(sqlmap.Path, config);
                    sqlmap.Scope = newSqlmap.Scope;
                    sqlmap.Statements = newSqlmap.Statements;
                    config.ResetMappedStatements();
                    _logger.Debug($"SmartSql.SmartSqlMapConfig Changed Reload SmartSqlMap: {sqlmap.Path} End");
                });
                #endregion
            }
            #endregion
        }


        private SmartSqlMap LoadSmartSqlMap(string path, SmartSqlMapConfig smartSqlMapConfig)
        {
            var sqlMap = new SmartSqlMap
            {
                SmartSqlMapConfig = smartSqlMapConfig,
                Path = path,
                Statements = new List<Statement> { },
                Caches = new List<SqlMap.Cache> { }
            };
            using (var configStream = FileLoader.Load(path))
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(configStream);
                XmlNamespaceManager xmlNsM = new XmlNamespaceManager(xmlDoc.NameTable);
                xmlNsM.AddNamespace("ns", "http://SmartSql.net/schemas/SmartSqlMap.xsd");
                sqlMap.Scope = xmlDoc.SelectSingleNode("//ns:SmartSqlMap", xmlNsM)
                    .Attributes["Scope"].Value;
                #region Init Caches
                var cacheNodes = xmlDoc.SelectNodes("//ns:Cache", xmlNsM);
                foreach (XmlElement cacheNode in cacheNodes)
                {
                    var cache = SqlMap.Cache.Load(cacheNode);
                    sqlMap.Caches.Add(cache);
                }
                #endregion
                #region Init Statement
                var statementNodes = xmlDoc.SelectNodes("//ns:Statement", xmlNsM);
                foreach (XmlElement statementNode in statementNodes)
                {
                    var statement = Statement.Load(statementNode, sqlMap);
                    sqlMap.Statements.Add(statement);
                }
                #endregion
                return sqlMap;
            }

        }

        public void Dispose()
        {
            FileWatcherLoader.Instance.Clear();
        }
    }
}
