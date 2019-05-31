using System.Collections;
using System.Collections.Generic;

namespace SmartSql.Cache.Default
{
    public class NoneCacheProvider : ICacheProvider
    {
        public bool SupportExpire => true;
        public void Dispose()
        {
           
        }

        public void Flush()
        {
            
        }

        public void Initialize(IDictionary<string, object> properties)
        {
            
        }
        public bool TryAdd(CacheKey cacheKey, object cacheItem)
        {
            return true;
        }

        public bool TryGetValue(CacheKey cacheKey, out object cacheItem)
        {
            cacheItem = default;
            return false;
        }
    }
}
