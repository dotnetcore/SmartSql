using org.apache.zookeeper;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SmartSql.ZooKeeperConfig
{
    public class CreateOptions
    {
        public string ConnectionString { get; set; }
        public int SessionTimeout { get; set; } = 4000;
        public IEnumerable<AuthInfo> AuthInfos { get; set; }
        public Func<WatchedEvent, Task> OnWatch { get; set; }
        public int MaxTryTimes { get; set; } = 3;
        public int Delay { get; set; } = 50;
    }
}
