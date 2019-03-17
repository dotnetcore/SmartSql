using SmartSql.Cache;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SmartSql.DistributedCache
{
    public abstract class DistributedCacheProvider : ICacheProvider, ICacheProviderAsync
    {
        protected ICacheKeyGegenerator CacheKeyGegenerator { get; private set; }
        public DistributedCacheProvider()
        {
            CacheKeyGegenerator = new MD5CacheKeyGegenerator();
        }
        public abstract void Initialize(IDictionary<string, object> properties);

        public abstract bool TryAdd(CacheKey cacheKey, object cacheItem);
        public abstract bool Remove(CacheKey cacheKey);
        public abstract bool TryGetValue(CacheKey cacheKey, out object cacheItem);
        public abstract void Flush();
        #region Async
        public abstract Task<bool> TryAddAsync(CacheKey cacheKey, object cacheItem);
        public abstract Task<bool> RemoveAsync(CacheKey cacheKey);
        public abstract Task<bool> TryGetValueAsync(CacheKey cacheKey, out object cacheItem);
        public abstract Task FlushAsync();

        public void Dispose()
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
