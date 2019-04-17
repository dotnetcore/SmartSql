using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SmartSql.Cache
{
    public interface ICacheProviderAsync 
    {
        Task<bool> TryGetValueAsync(CacheKey cacheKey, out object cacheItem);
        Task<bool> RemoveAsync(CacheKey cacheKey);
        Task<bool> TryAddAsync(CacheKey cacheKey, object cacheItem);
        Task FlushAsync();
    }
}
