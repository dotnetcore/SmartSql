using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace SmartSql.InvokeSync
{
    public class SyncService : ISyncService
    {
        private readonly ILogger<SyncService> _logger;
        private readonly ISyncFilter _syncFilter;
        private readonly IPublisher _publisher;

        public SyncService(
            ILogger<SyncService> logger
            ,ISyncFilter syncFilter
            ,IPublisher publisher)
        {
            _logger = logger;
            _syncFilter = syncFilter;
            _publisher = publisher;
        }

        public async Task Sync(ExecutionContext executionContext)
        {
            if (!_syncFilter.Filter(executionContext))
            {
                if (_logger.IsEnabled(LogLevel.Debug))
                {
                    _logger.LogDebug($"Sync Request -> StatementType:[{executionContext.Request.Statement?.StatementType}], Scope:[{executionContext.Request.Scope}],SqlId:[{executionContext.Request.SqlId}],RealSql:{executionContext.Request.RealSql} Filter false.");
                }
                return;
            }

            var syncRequest = executionContext.AsSyncRequest();
            await _publisher.PublishAsync(syncRequest);
            if (_logger.IsEnabled(LogLevel.Debug))
            {
                _logger.LogDebug($"Sync Request -> Id:[{syncRequest.Id}],StatementType:[{syncRequest.StatementType}],Scope:[{syncRequest.Scope}],SqlId:[{syncRequest.SqlId}],RealSql:{syncRequest.RealSql} succeeded.");
            }
        }
    }
}