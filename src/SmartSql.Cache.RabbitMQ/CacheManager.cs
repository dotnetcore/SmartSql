using System;

namespace SmartSql.Cache.RabbitMQ
{
    public class CacheManager : ICacheManager
    {
        
        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public bool TryGetCache(ExecutionContext executionContext, out object cacheItem)
        {
            throw new NotImplementedException();
        }

        public bool TryAddCache(ExecutionContext executionContext)
        {
            throw new NotImplementedException();
        }
    }
}