using System;
using System.Threading.Tasks;

namespace SmartSql.InvokeSync.RabbitMQ
{
    public class RabbitMQPublish : IPublish
    {
        private readonly RabbitMQOptions _rabbitMqOptions;

        public RabbitMQPublish(RabbitMQOptions rabbitMqOptions)
        {
            _rabbitMqOptions = rabbitMqOptions;
        }
        public Task PublishAsync(PublishRequest publishRequest)
        {
            throw new NotImplementedException();
        }
    }
}