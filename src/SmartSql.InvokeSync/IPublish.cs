using System;
using System.Threading.Tasks;

namespace SmartSql.InvokeSync
{
    public interface IPublish : IDisposable
    {
        Task PublishAsync(PublishRequest publishRequest);
    }
}