using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SmartSql.DIExtension;

namespace SmartSql.InvokeSync.Kafka
{
    public static class SmartSqlDIExtensions
    {
        public static SmartSqlDIBuilder AddKafkaPublisher(this SmartSqlDIBuilder smartSqlDiBuilder,
            Action<KafkaOptions> configure)
        {
            KafkaOptions rabbitMqOptions = new KafkaOptions();
            configure?.Invoke(rabbitMqOptions);
            smartSqlDiBuilder.Services.AddSingleton(rabbitMqOptions);
            smartSqlDiBuilder.Services.TryAddSingleton<IPublisher, KafkaPublisher>();
            smartSqlDiBuilder.Services.TryAddSingleton<ISubscriber, KafkaSubscriber>();
            return smartSqlDiBuilder;
        }
    }
}