using Microsoft.Extensions.Logging;
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
        private static object syncObj = new object();
        private static ZooKeeper instance;
        private readonly ILogger<ZooKeeperManager> logger;
        private readonly ILoggerFactory loggerFactory;
        private readonly CreateOptions options;
        public ZooKeeperManager(
            ILoggerFactory loggerFactory
            , CreateOptions options)
        {
            this.logger = loggerFactory.CreateLogger<ZooKeeperManager>();
            this.loggerFactory = loggerFactory;
            this.options = options;
        }

        public ZooKeeper Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncObj)
                    {
                        if (instance == null)
                        {
                            ZooKeeperWatcher zooKeeperWatcher = new ZooKeeperWatcher(loggerFactory, options.OnWatch);
                            instance = new ZooKeeper(options.ConnectionString, options.SessionTimeout, zooKeeperWatcher);

                            if (options.AuthInfos != null)
                            {
                                foreach (var authInfo in options.AuthInfos)
                                {
                                    instance.addAuthInfo(authInfo.Scheme, authInfo.Data);
                                }
                            }
                            logger.LogDebug($"ZooKeeper Initialized,ConnectString:{options.ConnectionString}!");
                            WaitConnected();
                        }
                    }
                }
                if (!IsAlive())
                {
                    instance = null;
                    logger.LogError($"ZooKeeper ConnectString:{options.ConnectionString} is not alive!");
                    throw new Exception($"ZooKeeper ConnectString:{ options.ConnectionString } is not alive!");
                }
                return instance;
            }
        }


        private void WaitConnected()
        {
            int initTryTimes = 0;
            while (!IsAlive())
            {
                ++initTryTimes;
                if (options.Delay > 0)
                {
                    System.Threading.Thread.Sleep(options.Delay);
                }
                if (initTryTimes > options.MaxTryTimes)
                {
                    instance = null;
                    logger.LogDebug($"ZooKeeper connect time out!:{options.ConnectionString}!");
                    throw new Exception("ZooKeeper connect time out!");
                }
            }
        }

        private bool IsAlive()
        {
            if (instance != null)
            {
                var zkState = instance.getState();
                bool isAlive = (zkState != ZooKeeper.States.CLOSED && zkState != ZooKeeper.States.AUTH_FAILED);
                logger.LogDebug($"ZooKeeper.States is not OK!");
                return isAlive;
            }
            return false;
        }

        public void Dispose()
        {
            instance.closeAsync().Wait();
        }
    }


}
