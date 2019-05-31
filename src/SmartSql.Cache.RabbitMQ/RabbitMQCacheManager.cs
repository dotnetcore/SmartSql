using System;
using System.Text;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Framing;
using SmartSql.Configuration;

namespace SmartSql.Cache.RabbitMQ
{
    /// <summary>
    /// TODO
    /// </summary>
    public class RabbitMQCacheManager : AbstractCacheManager
    {
        private readonly RabbitMQCacheOptions _rabbitMqCacheOptions;
        private IModel _subscribeChannel;

        public RabbitMQCacheManager(SmartSqlConfig smartSqlConfig, RabbitMQCacheOptions rabbitMqCacheOptions) : base(
            smartSqlConfig)
        {
            _rabbitMqCacheOptions = rabbitMqCacheOptions;
        }

        public override void ListenInvokeSucceeded()
        {
            SmartSqlConfig.InvokeSucceedListener.InvokeSucceeded += (sender, args) =>
            {
                var reqContext = args.ExecutionContext.Request;
                if (reqContext.IsStatementSql)
                {
                    PublishExecutedStatement(new ExecutedStatementRequest
                    {
                        FullSqlId = reqContext.FullSqlId
                    });
                }
            };
            ListenExecutedStatement();
        }

        private void PublishExecutedStatement(ExecutedStatementRequest executedStatementRequest)
        {
            _subscribeChannel.ExchangeDeclare(_rabbitMqCacheOptions.Exchange,
                _rabbitMqCacheOptions.ExchangeType, true, false, null);

            var data = JsonConvert.SerializeObject(executedStatementRequest);
            var body = Encoding.UTF8.GetBytes(data);

            _subscribeChannel.BasicPublish(_rabbitMqCacheOptions.Exchange,
                _rabbitMqCacheOptions.RoutingKey,
                false,
                new BasicProperties {Persistent = true},
                body);

            if (Logger.IsEnabled(LogLevel.Debug))
            {
                Logger.LogDebug(
                    $"PublishExecutedStatement -> FullSqlId:{executedStatementRequest.FullSqlId} succeeded.");
            }
        }

        private void SubscribeExecutedStatement(ExecutedStatementRequest executedStatementRequest)
        {
            FlushOnExecuted(executedStatementRequest.FullSqlId);
        }

        private void ListenExecutedStatement()
        {
            _subscribeChannel.ExchangeDeclare(_rabbitMqCacheOptions.Exchange,
                _rabbitMqCacheOptions.ExchangeType, true, false);
            _subscribeChannel.QueueDeclare(_rabbitMqCacheOptions.QueueName, true, false, false, null);
            _subscribeChannel.QueueBind(_rabbitMqCacheOptions.QueueName,
                _rabbitMqCacheOptions.Exchange,
                _rabbitMqCacheOptions.RoutingKey);
            var consumer = new EventingBasicConsumer(_subscribeChannel);
            consumer.Received += (model, ea) =>
            {
                var message = Encoding.UTF8.GetString(ea.Body);
                var executedReq = JsonConvert.DeserializeObject<ExecutedStatementRequest>(message);
                try
                {
                    SubscribeExecutedStatement(executedReq);
                    if (Logger.IsEnabled(LogLevel.Debug))
                    {
                        Logger.LogDebug(
                            $"SubscribeExecutedStatement -> FullSqlId:[{executedReq.FullSqlId}] succeeded.");
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogError(new EventId(ex.HResult), ex, ex.Message);
                }
            };
            _subscribeChannel.BasicQos(0, 1, false);
            _subscribeChannel.BasicConsume(_rabbitMqCacheOptions.QueueName, true, consumer);

            _subscribeChannel.CallbackException += (sender, ea) =>
            {
                Logger.LogError(ea.Exception, $"consumerChannel callback exception:{ea.Exception?.Message}");
            };
        }
    }
}