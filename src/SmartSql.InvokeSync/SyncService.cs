using System.Threading.Tasks;

namespace SmartSql.InvokeSync
{
    public class SyncService : ISyncService
    {
        private readonly ISyncFilter _syncFilter;
        private readonly IPublisher _publisher;

        public SyncService(ISyncFilter syncFilter,IPublisher publisher)
        {
            _syncFilter = syncFilter;
            _publisher = publisher;
        }

        public async Task Sync(ExecutionContext executionContext)
        {
            if (!_syncFilter.Filter(executionContext))
            {
                return;
            }
            await _publisher.PublishAsync(executionContext.AsPublishRequest());
        }
    }
}