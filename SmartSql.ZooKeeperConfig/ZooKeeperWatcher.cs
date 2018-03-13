using Microsoft.Extensions.Logging;
using org.apache.zookeeper;
using SmartSql.Abstractions;
using SmartSql.SqlMap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartSql.ZooKeeperConfig
{
    public class ZooKeeperWatcher : Watcher
    {
        private readonly ILogger<ZooKeeperWatcher> logger;
        protected readonly Func<WatchedEvent, Task> onWatch;

        public ZooKeeperWatcher(
            ILoggerFactory loggerFactory
            , Func<WatchedEvent, Task> onWatch = null
            )
        {
            this.logger = loggerFactory.CreateLogger<ZooKeeperWatcher>();
            this.onWatch = onWatch;
        }
        public override async Task process(WatchedEvent @event)
        {
            logger.LogDebug($"{@event.getPath()}|{@event.get_Type()}|{@event.getState()}");
            if (onWatch != null)
            {
                await onWatch(@event);
            }
        }
    }

    /// <summary>
    /// SmartSqlMapConfig 监控
    /// </summary>
    public class SmartSqlMapConfigWatcher : ZooKeeperWatcher
    {
        private readonly ILogger _logger;

        public ISmartSqlMapper SmartSqlMapper { get; private set; }
        public ZooKeeperConfigLoader ConfigLoader { get; set; }
        public SmartSqlMapConfigWatcher(ILoggerFactory loggerFactory
            , ISmartSqlMapper smartSqlMapper
            , ZooKeeperConfigLoader configLoader
            , Func<WatchedEvent, Task> onWatch = null
            ) : base(loggerFactory, onWatch)
        {
            _logger = loggerFactory.CreateLogger<SmartSqlMapConfigWatcher>();
            SmartSqlMapper = smartSqlMapper;
            ConfigLoader = configLoader;
        }
        public override async Task process(WatchedEvent @event)
        {
            await base.process(@event);
            string path = @event.getPath();
            _logger.LogDebug($"ZooKeeperConfigLoader.SmartSqlMapConfigWatcher process : {path} .");
            var eventType = @event.get_Type();
            if (eventType == Event.EventType.NodeDataChanged)
            {
                var config = SmartSqlMapper.SqlMapConfig;
                if (!config.Settings.IsWatchConfigFile)
                {
                    _logger.LogDebug($"ZooKeeperConfigLoader.SmartSqlMapConfigWatcher Changed ,dot not watch: {path} .");
                }
                else
                {
                    #region SmartSqlMapConfig File Watch

                    _logger.LogDebug($"ZooKeeperConfigLoader.SmartSqlMapConfigWatcher Changed ReloadConfig: {path} Starting");
                    var newConfig = await ConfigLoader.LoadAsync(path, SmartSqlMapper);
                    _logger.LogDebug($"ZooKeeperConfigLoader.SmartSqlMapConfigWatcher Changed ReloadConfig: {path} End");

                    #endregion
                }
            }
        }
    }
    /// <summary>
    /// SmartSqlMap 监控
    /// </summary>
    public class SmartSqlMapWatcher : ZooKeeperWatcher
    {
        private readonly ILogger _logger;

        public SmartSqlMapConfig SmartSqlMapConfig { get; private set; }
        public ZooKeeperConfigLoader ConfigLoader { get; set; }
        public SmartSqlMapWatcher(ILoggerFactory loggerFactory
            , SmartSqlMapConfig smartSqlMapConfig
            , ZooKeeperConfigLoader configLoader
            , Func<WatchedEvent, Task> onWatch = null
            ) : base(loggerFactory, onWatch)
        {
            _logger = loggerFactory.CreateLogger<SmartSqlMapWatcher>();
            SmartSqlMapConfig = smartSqlMapConfig;
            ConfigLoader = configLoader;
        }
        public override async Task process(WatchedEvent @event)
        {
            await base.process(@event);
            string path = @event.getPath();
            _logger.LogDebug($"ZooKeeperConfigLoader.SmartSqlMapWatcher process : {path} .");
            var eventType = @event.get_Type();
            if (eventType == Event.EventType.NodeDataChanged)
            {
                if (!SmartSqlMapConfig.Settings.IsWatchConfigFile)
                {
                    _logger.LogDebug($"ZooKeeperConfigLoader.SmartSqlMapWatcher Changed Reload SmartSqlMap,dot not watch: {path} .");
                }
                else
                {
                    _logger.LogDebug($"ZooKeeperConfigLoader.SmartSqlMapWatcher Changed Reload SmartSqlMap: {path} Starting");
                    var sqlmap = SmartSqlMapConfig.SmartSqlMaps.FirstOrDefault(m => m.Path == path);
                    var newSqlmap = await ConfigLoader.LoadSmartSqlMapAsync(path, SmartSqlMapConfig);
                    sqlmap.Scope = newSqlmap.Scope;
                    sqlmap.Statements = newSqlmap.Statements;
                    sqlmap.Caches = newSqlmap.Caches;
                    SmartSqlMapConfig.ResetMappedStatements();
                    SmartSqlMapConfig.SmartSqlMapper.CacheManager.ResetMappedCaches();
                    _logger.LogDebug($"ZooKeeperConfigLoader.SmartSqlMapWatcher Changed Reload SmartSqlMap: {path} End");
                }
            }

        }
    }
}
