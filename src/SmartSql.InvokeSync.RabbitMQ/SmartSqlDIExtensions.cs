using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using SmartSql.DIExtension;

namespace SmartSql.InvokeSync.RabbitMQ
{
    public static class SmartSqlDIExtensions
    {
        public static SmartSqlDIBuilder AddRabbitMQPublisher(this SmartSqlDIBuilder smartSqlDiBuilder,
            Action<RabbitMQOptions> configure)
        {
            RabbitMQOptions rabbitMqOptions = new RabbitMQOptions();
            configure?.Invoke(rabbitMqOptions);
            smartSqlDiBuilder.Services.AddSingleton<IPublisher, RabbitMQPublisher>(sp =>
            {
                var loggerFactory = sp.GetRequiredService<ILoggerFactory>();
                return new RabbitMQPublisher(loggerFactory.CreateLogger<RabbitMQPublisher>()
                    , rabbitMqOptions
                    , new PersistentConnection(rabbitMqOptions, loggerFactory.CreateLogger<PersistentConnection>()
                    ));
            });
            return smartSqlDiBuilder;
        }

        public static SmartSqlDIBuilder AddRabbitMQSubscriber(this SmartSqlDIBuilder smartSqlDiBuilder,
            Action<SubscriberOptions> configure)
        {
            SubscriberOptions rabbitMqOptions = new SubscriberOptions();
            configure?.Invoke(rabbitMqOptions);
            smartSqlDiBuilder.Services.AddSingleton<ISubscriber, RabbitMQSubscriber>(sp =>
            {
                var loggerFactory = sp.GetRequiredService<ILoggerFactory>();
                return new RabbitMQSubscriber(loggerFactory.CreateLogger<RabbitMQSubscriber>()
                    , rabbitMqOptions
                    , new PersistentConnection(rabbitMqOptions, loggerFactory.CreateLogger<PersistentConnection>()
                    ));
            });
            return smartSqlDiBuilder;
        }
    }
}