using System;
using SmartSql.Configuration;

namespace SmartSql.Cache.RabbitMQ
{
    public class RabbitMQCacheManager : CacheManager
    {
        public RabbitMQCacheManager(SmartSqlConfig smartSqlConfig) : base(smartSqlConfig)
        {
            
        }

        protected override void ListenInvokeSucceeded()
        {
        }

        public override void Dispose()
        {
            base.Dispose();
        }
    }
}