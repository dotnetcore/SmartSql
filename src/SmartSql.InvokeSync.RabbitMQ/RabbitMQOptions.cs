namespace SmartSql.InvokeSync.RabbitMQ
{
    public class RabbitMQOptions
    {
        public string HostName { get; set; }
        public string VirtualHost { get; set; }
        public string Password { get; set; } 
        public string UserName { get; set; }
        public string Exchange { get; set; } 
        public ushort RequestedHeartbeat { get; set; } = 60;
        public bool AutomaticRecoveryEnabled { get; set; } = true;
    }
}