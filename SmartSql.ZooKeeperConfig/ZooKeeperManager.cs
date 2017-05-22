using org.apache.zookeeper;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SmartSql.ZooKeeperConfig
{
    public class ZooKeeperManager : IDisposable
    {
        private ZooKeeperManager() { }
        public static readonly ZooKeeperManager Instance = new ZooKeeperManager();
        const int sessionTimeout = 4000;

        public Dictionary<String, ZooKeeper> MappedZooKeepers { get; set; } = new Dictionary<string, ZooKeeper>();

        public async Task<ZooKeeper> Get(String connStr)
        {
            ZooKeeper zk = null;
            bool isExists = MappedZooKeepers.ContainsKey(connStr);
            if (isExists)
            {
                zk = MappedZooKeepers[connStr];
                var zkState = zk.getState();
                if (zkState == ZooKeeper.States.CLOSED
                    ||
                    zkState == ZooKeeper.States.NOT_CONNECTED
                    )
                {
                    await Remove(connStr);
                    zk = new ZooKeeper(connStr, sessionTimeout, NoneWatcher.Instance);
                    MappedZooKeepers.Add(connStr, zk);
                }
                return zk;
            }
            zk = new ZooKeeper(connStr, sessionTimeout, NoneWatcher.Instance);
            MappedZooKeepers.Add(connStr, zk);
            return zk;
        }

        public async Task<bool> Remove(String connStr)
        {
            bool isExists = MappedZooKeepers.ContainsKey(connStr);
            if (!isExists)
            {
                return true;
            }
            var zk = MappedZooKeepers[connStr];
            await zk.closeAsync();
            return MappedZooKeepers.Remove(connStr);
        }

        public async void Dispose()
        {
            foreach (var zk in MappedZooKeepers.Values)
            {
                await zk.closeAsync();
            }
            MappedZooKeepers.Clear();
        }
    }

    public class NoneWatcher : Watcher
    {
        public static readonly NoneWatcher Instance = new NoneWatcher();
        private NoneWatcher() { }
        public override Task process(WatchedEvent @event)
        {
            return Task.CompletedTask;
        }
    }
}
