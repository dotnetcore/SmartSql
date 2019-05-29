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
                smartSqlBuilder.SmartSqlConfig.InvokeSucceedListener.InvokeSucceed += (sender, args) =>
                {
                    syncService.Sync(args.ExecutionContext);
                };
            }

            return serviceProvider;
        }
    }
}