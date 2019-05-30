using System;
using System.Text;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace SmartSql.InvokeSync.RabbitMQ
{
    public class RabbitMQSubscriber : ISubscriber
    {
        private readonly ILogger<RabbitMQSubscriber> _logger;
        private readonly RabbitMQOptions _rabbitMqOptions;
        private readonly PersistentConnection _connection;

        public RabbitMQSubscriber(
            ILogger<RabbitMQSubscriber> logger
            , RabbitMQOptions rabbitMqOptions
            , PersistentConnection connection)
        {
            _logger = logger;
            _rabbitMqOptions = rabbitMqOptions;
            _connection = connection;
        }

        public event EventHandler<SyncRequest> Received;

        public void Listening()
        {
            if (!_connection.IsConnected)
            {
                _connection.TryConnect();
            }

            var consumerChannel = _connection.CreateModel();
            consumerChannel.ExchangeDeclare(_rabbitMqOptions.Exchange,
                _rabbitMqOptions.ExchangeType, true, false);
            consumerChannel.QueueDeclare(_rabbitMqOptions.QueueName, true, false, false, null);
            consumerChannel.QueueBind(_rabbitMqOptions.QueueName,
                _rabbitMqOptions.Exchange,
                _rabbitMqOptions.RoutingKey);
            var consumer = new EventingBasicConsumer(consumerChannel);
            consumer.Received += (model, ea) =>
            {
                var message = Encoding.UTF8.GetString(ea.Body);
                var syncMsg = JsonConvert.DeserializeObject<SyncRequest>(message);
                try
                {
                    Received?.Invoke(this, syncMsg);
                    if (_logger.IsEnabled(LogLevel.Debug))
                    {
                         _logger.LogDebug($"Received Invoke -> Id:[{syncMsg.Id}],Scope:[{syncMsg.Scope}],[{syncMsg.SqlId}] succeeded.");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(new EventId(ex.HResult), ex, ex.Message);
                }
            };
            consumerChannel.BasicQos(0, 1, false);
            consumerChannel.BasicConsume(_rabbitMqOptions.QueueName, true, consumer);

            consumerChannel.CallbackException += (sender, ea) =>
            {
                _logger.LogError(ea.Exception,$"consumerChannel callback exception:{ea.Exception?.Message}");
            };
        }
    }
}