using System;

namespace SmartSql.InvokeSync
{
    public interface IPublish
    {
        void Publish(ExecutionContext executionContext);
    }
}