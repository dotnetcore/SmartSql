using System.Threading.Tasks;

namespace SmartSql.InvokeSync
{
    public interface ISyncService
    {
        Task Sync(ExecutionContext executionContext);
    }
}