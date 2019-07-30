using System;

namespace SmartSql.InvokeSync.RabbitMQ
{
    public class SubscriberOptions : RabbitMQOptions
    {
        public String QueueName { get; set; } = "smartsql.listen";
    }
}