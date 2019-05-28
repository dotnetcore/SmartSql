using System;
using System.Threading.Tasks;

namespace SmartSql.InvokeSync
{
    public interface IPublish
    {
        Task PublishAsync(PublishRequest publishRequest);
    }
}