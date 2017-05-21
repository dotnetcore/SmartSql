using SmartSql.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;
using SmartSql.SqlMap;
using org.apache.zookeeper;
using System.Threading.Tasks;
using System.IO;
using System.Xml.Serialization;
using System.Xml;
using SmartSql.Abstractions.Logging;

namespace SmartSql.ZooKeeperConfig
{
    /// <summary>
    /// ZooKeeper 配置加载器
    /// </summary>
    public class ZooKeeperConfigLoader : IConfigLoader
    {
        private static readonly ILog _logger = LogManager.GetLogger(typeof(LocalFileConfigLoader));

        public String ConnectString { get; private set; }
        public ZooKeeper ZooClient
        {
            get
            {
                return ZooKeeperManager.Instance.Get(ConnectString).Result;
            }
        }
        public ZooKeeperConfigLoader(String connStr)
        {
            ConnectString = connStr;
        }
        public SmartSqlMapConfig Load(string path, ISmartSqlMapper smartSqlMapper)
        {
            _logger.Debug($"SmartSql.SmartSqlMapConfig Load: {path} Starting");
            var config = LoadConfig(path, smartSqlMapper);
            _logger.Debug($"SmartSql.SmartSqlMapConfig Load: {path} End");
            smartSqlMapper.LoadConfig(config);
            return config;
        }

        private SmartSqlMapConfig LoadConfig(String path, ISmartSqlMapper smartSqlMapper)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(SmartSqlMapConfig));
            SmartSqlMapConfig config = null;
            var configResult = ZooClient.getDataAsync(path, new SmartSqlMapConfigWatcher(smartSqlMapper)).Result;
            using (MemoryStream configStream = new MemoryStream(configResult.Data))
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

        private SmartSqlMap LoadSmartSqlMap(string path, SmartSqlMapConfig smartSqlMapConfig)
        {
            var sqlMap = new SmartSqlMap
            {
                SmartSqlMapConfig = smartSqlMapConfig,
                Path = path,
                Statements = new List<Statement> { },
                Caches = new List<SqlMap.Cache> { }
            };
            var configResult = ZooClient.getDataAsync(path, new SmartSqlMapWatcher(sqlMap)).Result;
            using (MemoryStream configStream = new MemoryStream(configResult.Data))
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
    }
    /// <summary>
    /// SmartSqlMapConfig 监控
    /// </summary>
    public class SmartSqlMapConfigWatcher : Watcher
    {
        private static readonly ILog _logger = LogManager.GetLogger(typeof(SmartSqlMapConfigWatcher));

        public ISmartSqlMapper SmartSqlMapper { get; private set; }
        public SmartSqlMapConfigWatcher(ISmartSqlMapper smartSqlMapper)
        {
            SmartSqlMapper = smartSqlMapper;
        }
        public override Task process(WatchedEvent @event)
        {
            if (@event.get_Type() == Event.EventType.NodeDataChanged)
            {
                var config = SmartSqlMapper.SqlMapConfig;
                #region SmartSqlMapConfig File Watch

                _logger.Debug($"SmartSql.SmartSqlMapConfig Changed ReloadConfig: {config.Path} Starting");
                //var newConfig = Load(config.Path, smartSqlMapper);
                //SmartSqlMapper.LoadConfig(newConfig);
                _logger.Debug($"SmartSql.SmartSqlMapConfig Changed ReloadConfig: {config.Path} End");

                #endregion
            }
            throw new NotImplementedException();
        }
    }
    /// <summary>
    /// SmartSqlMap 监控
    /// </summary>
    public class SmartSqlMapWatcher : Watcher
    {
        public SmartSqlMap CurrentSmartSqlMap { get; private set; }
        public SmartSqlMapWatcher(SmartSqlMap smartSqlMap)
        {
            CurrentSmartSqlMap = smartSqlMap;
        }
        public override Task process(WatchedEvent @event)
        {
            if (@event.get_Type() == Event.EventType.NodeDataChanged)
            {

            }
            throw new NotImplementedException();
        }
    }
}
