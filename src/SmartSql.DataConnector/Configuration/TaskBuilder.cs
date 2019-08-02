using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.Logging;
using SmartSql.InvokeSync.Kafka;
using SmartSql.InvokeSync.RabbitMQ;
using YamlDotNet.Serialization;

namespace SmartSql.DataConnector.Configuration
{
    public class TaskBuilder
    {
        private readonly ILoggerFactory _loggerFactory;
        private readonly IDeserializer _deserializer;
        private readonly ILogger<TaskBuilder> _logger;
        public string ConfigPath { get; }
        public Task Task { get; private set; }

        public TaskBuilder(string configPath, ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;
            _logger = _loggerFactory.CreateLogger<TaskBuilder>();
            ConfigPath = configPath;
            _deserializer = new DeserializerBuilder().Build();
        }

        public Task Build()
        {
            _logger.LogInformation($"Build Path:[{ConfigPath}].");
            using (StreamReader configStream = new StreamReader(ConfigPath))
            {
                var configStr = configStream.ReadToEnd();
                Task = _deserializer.Deserialize<Task>(configStr);
                Init();
                return Task;
            }
        }

        private void Init()
        {
            InitSubscriber();
            switch (Task.DataSource.Type)
            {
                case "RDB":
                {
                    Task.DataSource.Parameters.EnsureValue("DbProvider", out string dbProvider);
                    Task.DataSource.Parameters.EnsureValue("ConnectionString", out string connectionString);
                    Task.DataSource.Instance = new SmartSqlBuilder()
                                               .UseAlias(Task.Name)
                                               .RegisterToContainer(false)
                                               .UseLoggerFactory(_loggerFactory)
                                               .UseDataSource(dbProvider, connectionString).Build();
                    break;
                }

                default:
                {
                    throw new ArgumentException($"Can not support DataSource.Type:[{Task.DataSource.Type}].");
                }
            }
        }

        private void InitSubscriber()
        {
            var taskParameters = Task.Subscriber.Parameters;
            switch (Task.Subscriber.Type.ToUpper())
            {
                case "KAFKA":
                {
                    taskParameters.EnsureValue(nameof(KafkaOptions.Servers), out string services);
                    taskParameters.EnsureValue(nameof(KafkaOptions.Servers), out string topic);
                    KafkaOptions kafkaOptions = new KafkaOptions
                    {
                        Servers = services,
                        Topic = topic,
                        Config = Task.Subscriber.Parameters
                    };
                    Task.Subscriber.Instance =
                        new KafkaSubscriber(kafkaOptions, _loggerFactory.CreateLogger<KafkaSubscriber>());
                    break;
                }

                case "RABBITMQ":
                {
                    taskParameters.EnsureValue(nameof(SubscriberOptions.HostName), out string hostName);
                    taskParameters.EnsureValue(nameof(SubscriberOptions.UserName), out string userName);
                    taskParameters.EnsureValue(nameof(SubscriberOptions.Password), out string password);
                    taskParameters.EnsureValue(nameof(SubscriberOptions.RoutingKey), out string routerKey);
                    taskParameters.EnsureValue(nameof(SubscriberOptions.QueueName), out string queueName);
                    taskParameters.EnsureValue(nameof(SubscriberOptions.Exchange), out string exchange);
                    SubscriberOptions rabbitMqOptions = new SubscriberOptions
                    {
                        HostName = hostName,
                        UserName = userName,
                        Password = password,
                        RoutingKey = routerKey,
                        QueueName = queueName,
                        Exchange = exchange
                    };
                    if (taskParameters.Value(nameof(SubscriberOptions.VirtualHost), out String virtualHost))
                    {
                        rabbitMqOptions.VirtualHost = virtualHost;
                    }

                    if (taskParameters.Value(nameof(SubscriberOptions.ExchangeType), out String exchangeType))
                    {
                        rabbitMqOptions.ExchangeType = exchangeType;
                    }

                    if (taskParameters.Value(nameof(SubscriberOptions.RequestedHeartbeat),
                                             out ushort requestedHeartbeat))
                    {
                        rabbitMqOptions.RequestedHeartbeat = requestedHeartbeat;
                    }

                    if (taskParameters.Value(nameof(SubscriberOptions.AutomaticRecoveryEnabled),
                                             out bool automaticRecoveryEnabled))
                    {
                        rabbitMqOptions.AutomaticRecoveryEnabled = automaticRecoveryEnabled;
                    }

                    Task.Subscriber.Instance = new RabbitMQSubscriber(_loggerFactory.CreateLogger<RabbitMQSubscriber>()
                                                                      , rabbitMqOptions
                                                                      , new PersistentConnection(rabbitMqOptions,
                                                                                                 _loggerFactory.CreateLogger<PersistentConnection>()
                                                                                                ));
                    break;
                }

                default:
                {
                    throw new ArgumentException($"Can not support Subscriber.Type:[{Task.Subscriber.Type}].");
                }
            }
        }
    }
}