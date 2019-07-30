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
        private readonly SubscriberOptions _subscriberOptions;
        private readonly PersistentConnection _connection;
        private IModel _consumerChannel;
        public RabbitMQSubscriber(
            ILogger<RabbitMQSubscriber> logger
            , SubscriberOptions subscriberOptions
            , PersistentConnection connection)
        {
            _logger = logger;
            _subscriberOptions = subscriberOptions;
            _connection = connection;
        }

        public event EventHandler<SyncRequest> Received;

        public void Start()
        {
            if (!_connection.IsConnected)
            {
                _connection.TryConnect();
            }

            _consumerChannel = _connection.CreateModel();
            _consumerChannel.ExchangeDeclare(_subscriberOptions.Exchange,
                _subscriberOptions.ExchangeType, true, false);
            _consumerChannel.QueueDeclare(_subscriberOptions.QueueName, true, false, false, null);
            _consumerChannel.QueueBind(_subscriberOptions.QueueName,
                _subscriberOptions.Exchange,
                _subscriberOptions.RoutingKey);
            var consumer = new EventingBasicConsumer(_consumerChannel);
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
            _consumerChannel.BasicQos(0, 1, false);
            _consumerChannel.BasicConsume(_subscriberOptions.QueueName, true, consumer);

            _consumerChannel.CallbackException += (sender, ea) =>
            {
                _logger.LogError(ea.Exception,$"consumerChannel callback exception:{ea.Exception?.Message}");
            };
        }

        public void Stop()
        {
            _consumerChannel.Dispose();
        }
    }
}