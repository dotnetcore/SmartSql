using System.Threading.Tasks;

namespace SmartSql.InvokeSync
{
    public class SyncService : ISyncService
    {
        private readonly ISyncFilter _syncFilter;
        private readonly IPublish _publish;

        public SyncService(ISyncFilter syncFilter,IPublish publish)
        {
            _syncFilter = syncFilter;
            _publish = publish;
        }

        public async Task Sync(ExecutionContext executionContext)
        {
            if (!_syncFilter.Filter(executionContext))
            {
                return;
            }
            await _publish.PublishAsync(executionContext.AsPublishRequest());
        }
    }
}