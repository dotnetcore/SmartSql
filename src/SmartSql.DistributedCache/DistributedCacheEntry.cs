using SmartSql.Cache;
using System;

namespace SmartSql.DistributedCache
{
    public class DistributedCacheEntry
    {
        public CacheKey CacheKey { get; set; }
        public Object Data { get; set; }
    }
}
