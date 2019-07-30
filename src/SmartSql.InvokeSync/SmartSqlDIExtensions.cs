using System;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SmartSql;
using SmartSql.DIExtension;
using SmartSql.InvokeSync;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class SmartSqlDIExtensions
    {
        public static SmartSqlDIBuilder AddInvokeSync(this SmartSqlDIBuilder smartSqlDiBuilder,
            Action<SyncFilterOptions> configure)
        {
            SyncFilterOptions syncFilterOptions = new SyncFilterOptions();
            configure?.Invoke(syncFilterOptions);
            smartSqlDiBuilder.Services.AddSingleton(syncFilterOptions);
            smartSqlDiBuilder.Services.TryAddSingleton<ISyncFilter, SyncFilter>();
            smartSqlDiBuilder.Services.TryAddSingleton<ISyncService, SyncService>();
            return smartSqlDiBuilder;
        }

        public static IServiceProvider UseSmartSqlSync(this IServiceProvider serviceProvider)
        {
            var syncService = serviceProvider.GetRequiredService<ISyncService>();
            foreach (var smartSqlBuilder in serviceProvider.GetServices<SmartSqlBuilder>())
            {
                smartSqlBuilder.SmartSqlConfig.InvokeSucceedListener.InvokeSucceeded += (sender, args) =>
                {
                    syncService.Sync(args.ExecutionContext);
                };
            }

            return serviceProvider;
        }

        public static IServiceProvider UseSmartSqlSubscriber(this IServiceProvider serviceProvider,
            Action<SyncRequest> onReceived)
        {
            if (onReceived == null) throw new ArgumentNullException(nameof(onReceived));

            var subscribers = serviceProvider.GetServices<ISubscriber>();
            foreach (var subscriber in subscribers)
            {
                subscriber.Received += (sender, request) => { onReceived(request); };
                subscriber.Start();
            }

            return serviceProvider;
        }
    }
}