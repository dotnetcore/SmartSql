using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SmartSql.DIExtension;

namespace SmartSql.InvokeSync.RabbitMQ
{
    public static class SmartSqlDIExtensions
    {
        public static SmartSqlDIBuilder AddRabbitMQPublish(this SmartSqlDIBuilder smartSqlDiBuilder,
            Action<RabbitMQOptions> configure)
        {
            RabbitMQOptions rabbitMqOptions = new RabbitMQOptions();
            configure?.Invoke(rabbitMqOptions);
            smartSqlDiBuilder.Services.AddSingleton(rabbitMqOptions);
            smartSqlDiBuilder.Services.TryAddSingleton<PersistentConnection>();
            smartSqlDiBuilder.Services.TryAddSingleton<IPublish, RabbitMQPublish>();
            return smartSqlDiBuilder;
        }
    }
}