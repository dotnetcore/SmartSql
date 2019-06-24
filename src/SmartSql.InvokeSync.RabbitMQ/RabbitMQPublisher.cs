using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Framing;

namespace SmartSql.InvokeSync.RabbitMQ
{
    public class RabbitMQPublisher : IPublisher
    {
        private readonly ILogger<RabbitMQPublisher> _logger;
        private readonly RabbitMQOptions _rabbitMqOptions;
        private readonly PersistentConnection _connection;
        private IModel _channel;

        public RabbitMQPublisher(
            ILogger<RabbitMQPublisher> _logger
            , RabbitMQOptions rabbitMqOptions
            , PersistentConnection connection)
        {
            this._logger = _logger;
            _rabbitMqOptions = rabbitMqOptions;
            _connection = connection;
        }

        public Task PublishAsync(SyncRequest syncRequest)
        {
            if (!_connection.IsConnected)
            {
                _connection.TryConnect();
            }

            var channel = GetChannel();

            channel.ExchangeDeclare(_rabbitMqOptions.Exchange,
                _rabbitMqOptions.ExchangeType, true, false, null);

            var data = JsonConvert.SerializeObject(syncRequest);
            var body = Encoding.UTF8.GetBytes(data);

            channel.BasicPublish(_rabbitMqOptions.Exchange,
                _rabbitMqOptions.RoutingKey,
                false,
                new BasicProperties {Persistent = true},
                body);

            if (_logger.IsEnabled(LogLevel.Debug))
            {
                _logger.LogDebug(
                    $"Publish SyncRequest -> Id:{syncRequest.Id} succeeded, {nameof(RabbitMQOptions.Exchange)}:[{_rabbitMqOptions.Exchange}], {nameof(RabbitMQOptions.ExchangeType)}:[{_rabbitMqOptions.ExchangeType}], {nameof(RabbitMQOptions.RoutingKey)}:[{_rabbitMqOptions.RoutingKey}].");
            }

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