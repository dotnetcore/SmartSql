using System;
using System.Threading.Tasks;

namespace SmartSql.InvokeSync
{
    public interface ISubscriber
    {
        event EventHandler<SyncRequest> Received;
        void Start();
        void Stop();
    }
}