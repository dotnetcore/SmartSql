using System;
using org.apache.zookeeper;
using System.Threading.Tasks;
using System.IO;
using SmartSql.Abstractions.Config;
using Microsoft.Extensions.Logging;
using SmartSql.Configuration;
using SmartSql.Logging;
using SmartSql.Configuration.Maps;

namespace SmartSql.ZooKeeperConfig
{
    /// <summary>
    /// ZooKeeper 配置加载器
    /// </summary>
    public class ZooKeeperConfigLoader : ConfigLoader
    {
        private readonly ILoggerFactory _loggerFactory;
        private readonly ZooKeeperOptions _options;
        private readonly ILogger _logger;
        private readonly ZooKeeperManager _zooKeeperManager;

        public String ConnectString { get; private set; }
        public ZooKeeper ZooClient
        {
            get
            {
                return _zooKeeperManager.Instance;
            }
        }

        public override event OnChangedHandler OnChanged;

        public ZooKeeperConfigLoader(String connStr, String sqlMapConfigPath) : this(NoneLoggerFactory.Instance, connStr, sqlMapConfigPath)
        {

        }
        public ZooKeeperConfigLoader(
             ILoggerFactory loggerFactory
            , String connStr
            , String sqlMapConfigPath
            ) : this(loggerFactory, new ZooKeeperOptions
            {
                ConnectionString = connStr,
                SqlMapConfigPath = sqlMapConfigPath
            })
        {

        }
        public ZooKeeperConfigLoader(ILoggerFactory loggerFactory, ZooKeeperOptions options)
        {
            _loggerFactory = loggerFactory;
            this._options = options;
            _logger = loggerFactory.CreateLogger<ZooKeeperConfigLoader>();
            _zooKeeperManager = new ZooKeeperManager(loggerFactory, options);

        }
        public void TriggerChanged(OnChangedEventArgs eventArgs)
        {
            OnChanged?.Invoke(this, eventArgs);
        }

        public override SmartSqlMapConfig Load()
        {
            var loadTask = LoadAsync();
            loadTask.Wait();
            return loadTask.Result;
        }


        public async Task<SmartSqlMapConfig> LoadAsync()
        {
            _logger.LogDebug($"SmartSql.ZooKeeperConfigLoader Load: {_options.SqlMapConfigPath} Starting");
            var configResult = await ZooClient.getDataAsync(_options.SqlMapConfigPath, new SmartSqlMapConfigWatcher(_loggerFactory, this));
            var configStream = new ConfigStream
            {
                Path = _options.SqlMapConfigPath,
                Stream = new MemoryStream(configResult.Data)
            };
            var config = LoadConfig(configStream);

            foreach (var sqlmapSource in config.SmartSqlMapSources)
            {
                switch (sqlmapSource.Type)
                {
                    case SmartSqlMapSource.ResourceType.File:
                        {
                            var sqlmap = await LoadSmartSqlMapAsync(sqlmapSource.Path, config);
                            config.SmartSqlMaps.Add(sqlmap.Scope, sqlmap);
                            break;
                        }
                    case SmartSqlMapSource.ResourceType.Directory:
                        {
                            var sqlmapChildren = await ZooClient.getChildrenAsync(sqlmapSource.Path);
                            foreach (var sqlmapChild in sqlmapChildren.Children)
                            {
                                var sqlmapPath = $"{sqlmapSource.Path}/{sqlmapChild}";
                                var sqlmap = await LoadSmartSqlMapAsync(sqlmapPath, config);
                                config.SmartSqlMaps.Add(sqlmap.Scope, sqlmap);
                            }
                            break;
                        }
                    default:
                        {
                            throw new ArgumentException("unknow SmartSqlMap.Type!");
                        }
                }

            }
            _logger.LogDebug($"SmartSql.ZooKeeperConfigLoader Load: {_options.SqlMapConfigPath} End");
            return config;
        }

        public async Task<SmartSqlMap> LoadSmartSqlMapAsync(String path, SmartSqlMapConfig config)
        {
            _logger.LogDebug($"SmartSql.LoadSmartSqlMapAsync Load: {path}");
            var sqlmapResult = await ZooClient.getDataAsync(path, new SmartSqlMapWatcher(_loggerFactory, config, this));
            var sqlmapStream = new ConfigStream
            {
                Path = path,
                Stream = new MemoryStream(sqlmapResult.Data)
            };
            return LoadSmartSqlMap(sqlmapStream);
        }


        public override void Dispose()
        {
            _zooKeeperManager.Dispose();
        }
    }

}
