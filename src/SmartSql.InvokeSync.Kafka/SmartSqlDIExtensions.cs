using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using SmartSql.DIExtension;

namespace SmartSql.InvokeSync.Kafka
{
    public static class SmartSqlDIExtensions
    {
        public static SmartSqlDIBuilder AddKafkaPublisher(this SmartSqlDIBuilder smartSqlDiBuilder,
            Action<KafkaOptions> configure)
        {
            KafkaOptions kafkaOptions = new KafkaOptions();
            configure?.Invoke(kafkaOptions);
            smartSqlDiBuilder.Services.AddSingleton<IPublisher, KafkaPublisher>(sp =>
            {
                var loggerFactory = sp.GetRequiredService<ILoggerFactory>();
                return new KafkaPublisher(kafkaOptions, loggerFactory.CreateLogger<KafkaPublisher>());
            });
            return smartSqlDiBuilder;
        }

        public static SmartSqlDIBuilder AddKafkaSubscriber(this SmartSqlDIBuilder smartSqlDiBuilder,
            Action<KafkaOptions> configure)
        {
            KafkaOptions kafkaOptions = new KafkaOptions();
            configure?.Invoke(kafkaOptions);
            smartSqlDiBuilder.Services.AddSingleton<ISubscriber, KafkaSubscriber>(sp =>
            {
                var loggerFactory = sp.GetRequiredService<ILoggerFactory>();
                return new KafkaSubscriber(kafkaOptions, loggerFactory.CreateLogger<KafkaSubscriber>());
            });
            return smartSqlDiBuilder;
        }
    }
}