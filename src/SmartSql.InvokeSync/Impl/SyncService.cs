using System.Threading.Tasks;

namespace SmartSql.InvokeSync.Impl
{
    public class SyncService : ISyncService
    {
        private readonly IPublish _publish;

        public SyncService(IPublish publish)
        {
            _publish = publish;
        }

        public async Task Sync(ExecutionContext executionContext)
        {
            await _publish.PublishAsync(executionContext.AsPublishRequest());
        }
    }
}