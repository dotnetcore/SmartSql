using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace SmartSql.Cache
{
    public interface ICacheProvider : IDisposable
    {
        bool SupportExpire { get; }
        void Initialize(IDictionary<String, object> properties);
        bool TryGetValue(CacheKey cacheKey, out object cacheItem);
        bool TryAdd(CacheKey cacheKey, object cacheItem);
        void Flush();
    }
}
