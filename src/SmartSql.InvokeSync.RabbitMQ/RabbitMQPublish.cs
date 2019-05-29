using System;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Framing;

namespace SmartSql.InvokeSync.RabbitMQ
{
    public class RabbitMQPublish : IPublish
    {
        private readonly RabbitMQOptions _rabbitMqOptions;
        private readonly PersistentConnection _connection;
        private IModel _channel;

        public RabbitMQPublish(RabbitMQOptions rabbitMqOptions, PersistentConnection connection)
        {
            _rabbitMqOptions = rabbitMqOptions;
            _connection = connection;
        }

        public Task PublishAsync(PublishRequest publishRequest)
        {
            if (!_connection.IsConnected)
            {
                _connection.TryConnect();
            }

            var channel = GetChannel();

            channel.ExchangeDeclare(_rabbitMqOptions.Exchange,
                _rabbitMqOptions.ExchangeType, true, false, null);

            var data = JsonConvert.SerializeObject(publishRequest);
            var body = Encoding.UTF8.GetBytes(data);

            channel.BasicPublish(_rabbitMqOptions.Exchange,
                _rabbitMqOptions.RoutingKey,
                false,
                new BasicProperties {Persistent = true},
                body);
            
            return Task.CompletedTask;
        }

        private IModel GetChannel()
        {
            if (_channel != null)
            {
                return _channel;
            }

            _channel = _connection.CreateModel();
            return _channel;
        }

        public void Dispose()
        {
            _connection?.Dispose();
            _channel?.Dispose();
        }
    }
}