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
        public String QueueName { get; }
        public String RouterKey { get; }
        private readonly ILogger<RabbitMQSubscriber> _logger;
        public SubscriberOptions SubscriberOptions { get; }
        private readonly PersistentConnection _connection;
        private IModel _consumerChannel;

        public RabbitMQSubscriber(
            ILogger<RabbitMQSubscriber> logger
            , SubscriberOptions subscriberOptions
            , PersistentConnection connection)
        {
            _logger = logger;
            SubscriberOptions = subscriberOptions;
            QueueName = subscriberOptions.QueueName;
            RouterKey = subscriberOptions.RoutingKey;
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
            _consumerChannel.ExchangeDeclare(SubscriberOptions.Exchange,
                SubscriberOptions.ExchangeType, true, false);
            _consumerChannel.QueueDeclare(SubscriberOptions.QueueName, true, false, false, null);
            _consumerChannel.QueueBind(SubscriberOptions.QueueName,
                SubscriberOptions.Exchange,
                SubscriberOptions.RoutingKey);
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
                        _logger.LogDebug(
                            $"Received Invoke -> Id:[{syncMsg.Id}],Scope:[{syncMsg.Scope}],[{syncMsg.SqlId}] succeeded.");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(new EventId(ex.HResult), ex,
                        $"Received Invoke -> Id:{syncMsg.Id} failed, {nameof(SubscriberOptions.QueueName)}:[{QueueName}]. {Environment.NewLine} -> SyncRequest: [{message}]");
                }
            };
            _consumerChannel.BasicQos(0, 1, false);
            _consumerChannel.BasicConsume(SubscriberOptions.QueueName, true, consumer);

            _consumerChannel.CallbackException += (sender, ea) =>
            {
                _logger.LogError(ea.Exception, $"consumerChannel callback exception:{ea.Exception?.Message}");
            };
        }

        public void Stop()
        {
            _connection?.Dispose();
            _consumerChannel?.Dispose();
        }
    }
}