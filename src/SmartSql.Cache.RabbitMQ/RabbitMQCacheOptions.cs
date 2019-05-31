using System;

namespace SmartSql.Cache.RabbitMQ
{
    public class RabbitMQCacheOptions
    {
        public string HostName { get; set; } = "localhost";
        public string VirtualHost { get; set; }= "/";
        public string Password { get; set; } 
        public string UserName { get; set; }
        public string Exchange { get; set; } = "smartsql";
        public string ExchangeType { get; set; } = "direct";
        public ushort RequestedHeartbeat { get; set; } = 60;
        public bool AutomaticRecoveryEnabled { get; set; } = true;
        public String RoutingKey { get; set; } = "sync";
        public String QueueName { get; set; } = "smartsql.cache";
    }
}