using System;
using System.Threading.Tasks;

namespace SmartSql.InvokeSync
{
    public interface IPublisher : IDisposable
    {
        Task PublishAsync(SyncRequest syncRequest);
    }
}