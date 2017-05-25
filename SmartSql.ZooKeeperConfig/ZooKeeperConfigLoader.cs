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
using SmartSql.Abstractions.Config;
using System.Linq;
namespace SmartSql.ZooKeeperConfig
{
    /// <summary>
    /// ZooKeeper 配置加载器
    /// </summary>
    public class ZooKeeperConfigLoader : ConfigLoader
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
        public override SmartSqlMapConfig Load(string path, ISmartSqlMapper smartSqlMapper)
        {
            var loadTask = LoadAsync(path, smartSqlMapper);
            loadTask.Wait();
            return loadTask.Result;
        }


        public async Task<SmartSqlMapConfig> LoadAsync(string path, ISmartSqlMapper smartSqlMapper)
        {
            _logger.Debug($"SmartSql.ZooKeeperConfigLoader Load: {path} Starting");
            var configResult = await ZooClient.getDataAsync(path, new SmartSqlMapConfigWatcher(smartSqlMapper, this));
            var configStream = new ConfigStream
            {
                Path = path,
                Stream = new MemoryStream(configResult.Data)
            };
            var config = LoadConfig(configStream, smartSqlMapper);

            foreach (var sqlmapSource in config.SmartSqlMapSources)
            {

                var sqlmap = await LoadSmartSqlMapAsync(sqlmapSource.Path, config);
                config.SmartSqlMaps.Add(sqlmap);
            }
            _logger.Debug($"SmartSql.ZooKeeperConfigLoader Load: {path} End");

            smartSqlMapper.LoadConfig(config);
            return config;
        }

        public async Task<SmartSqlMap> LoadSmartSqlMapAsync(String path, SmartSqlMapConfig config)
        {
            _logger.Debug($"SmartSql.LoadSmartSqlMapAsync Load: {path}");
            var sqlmapResult = await ZooClient.getDataAsync(path, new SmartSqlMapWatcher(config, this));
            var sqlmapStream = new ConfigStream
            {
                Path = path,
                Stream = new MemoryStream(sqlmapResult.Data)
            };
            return LoadSmartSqlMap(sqlmapStream, config);
        }


        public override void Dispose()
        {
            ZooKeeperManager.Instance.Dispose();
        }
    }
    /// <summary>
    /// SmartSqlMapConfig 监控
    /// </summary>
    public class SmartSqlMapConfigWatcher : Watcher
    {
        private static readonly ILog _logger = LogManager.GetLogger(typeof(SmartSqlMapConfigWatcher));

        public ISmartSqlMapper SmartSqlMapper { get; private set; }
        public ZooKeeperConfigLoader ConfigLoader { get; set; }
        public SmartSqlMapConfigWatcher(ISmartSqlMapper smartSqlMapper, ZooKeeperConfigLoader configLoader)
        {
            SmartSqlMapper = smartSqlMapper;
            ConfigLoader = configLoader;
        }
        public override async Task process(WatchedEvent @event)
        {
            string path = @event.getPath();
            _logger.Debug($"ZooKeeperConfigLoader.SmartSqlMapConfigWatcher process : {path} .");
            var eventType = @event.get_Type();
            if (eventType == Event.EventType.NodeDataChanged)
            {
                var config = SmartSqlMapper.SqlMapConfig;
                if (!config.Settings.IsWatchConfigFile)
                {
                    _logger.Debug($"ZooKeeperConfigLoader.SmartSqlMapConfigWatcher Changed ,dot not watch: {path} .");
                }
                else
                {
                    #region SmartSqlMapConfig File Watch

                    _logger.Debug($"ZooKeeperConfigLoader.SmartSqlMapConfigWatcher Changed ReloadConfig: {path} Starting");
                    var newConfig = await ConfigLoader.LoadAsync(path, SmartSqlMapper);
                    _logger.Debug($"ZooKeeperConfigLoader.SmartSqlMapConfigWatcher Changed ReloadConfig: {path} End");

                    #endregion
                }
            }
        }
    }
    /// <summary>
    /// SmartSqlMap 监控
    /// </summary>
    public class SmartSqlMapWatcher : Watcher
    {
        private static readonly ILog _logger = LogManager.GetLogger(typeof(SmartSqlMapWatcher));

        public SmartSqlMapConfig SmartSqlMapConfig { get; private set; }
        public ZooKeeperConfigLoader ConfigLoader { get; set; }
        public SmartSqlMapWatcher(SmartSqlMapConfig smartSqlMapConfig, ZooKeeperConfigLoader configLoader)
        {
            SmartSqlMapConfig = smartSqlMapConfig;
            ConfigLoader = configLoader;
        }
        public override async Task process(WatchedEvent @event)
        {
            string path = @event.getPath();
            _logger.Debug($"ZooKeeperConfigLoader.SmartSqlMapWatcher process : {path} .");
            var eventType = @event.get_Type();
            if (eventType == Event.EventType.NodeDataChanged)
            {
                if (!SmartSqlMapConfig.Settings.IsWatchConfigFile)
                {
                    _logger.Debug($"ZooKeeperConfigLoader.SmartSqlMapWatcher Changed Reload SmartSqlMap,dot not watch: {path} .");
                }
                else
                {
                    _logger.Debug($"ZooKeeperConfigLoader.SmartSqlMapWatcher Changed Reload SmartSqlMap: {path} Starting");
                    var sqlmap = SmartSqlMapConfig.SmartSqlMaps.FirstOrDefault(m => m.Path == path);
                    var newSqlmap = await ConfigLoader.LoadSmartSqlMapAsync(path, SmartSqlMapConfig);
                    sqlmap.Scope = newSqlmap.Scope;
                    sqlmap.Statements = newSqlmap.Statements;
                    sqlmap.Caches = newSqlmap.Caches;
                    SmartSqlMapConfig.ResetMappedStatements();
                    SmartSqlMapConfig.SmartSqlMapper.CacheManager.ResetMappedCaches();
                    _logger.Debug($"ZooKeeperConfigLoader.SmartSqlMapWatcher Changed Reload SmartSqlMap: {path} End");
                }
            }

        }
    }
}
