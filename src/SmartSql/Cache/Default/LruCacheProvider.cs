using System;
using System.Collections;
using System.Collections.Generic;

namespace SmartSql.Cache.Default
{
    /// <summary>
    /// Least Recently Used
    /// </summary>
    public class LruCacheProvider : ICacheProvider
    {
        private const String CACHE_SIZE = "CacheSize";
        private int _cacheSize = 100;
        private Dictionary<CacheKey, Object> _cache;
        private IList<CacheKey> _cacheKeys;

        public void Dispose()
        {
            Flush();
        }

        public void Flush()
        {
            lock (this)
            {
                _cache.Clear();
                _cacheKeys.Clear();
            }
        }

        public void Initialize(IDictionary<string, object> properties)
        {
            if (properties.Value(CACHE_SIZE, out int cacheSize))
            {
                _cacheSize = cacheSize;
            }
            _cache = new Dictionary<CacheKey, object>(_cacheSize);
            _cacheKeys = new List<CacheKey>(_cacheSize);
        }

        public bool TryAdd(CacheKey cacheKey, object cacheItem)
        {
            lock (this)
            {
                if (_cacheKeys.Count > _cacheSize)
                {
                    var removedKey = _cacheKeys[0];
                    _cacheKeys.RemoveAt(0);
                    _cache.Remove(removedKey);
                }
                _cache.Add(cacheKey, cacheItem);
                _cacheKeys.Add(cacheKey);
            }
            return true;
        }

        public bool TryGetValue(CacheKey cacheKey, out object cacheItem)
        {
            var cached = false;
            lock (this)
            {
                cached = _cacheKeys.Remove(cacheKey);
                _cacheKeys.Add(cacheKey);
                cacheItem = cached ? _cache[cacheKey] : default;
            }
            return cached;
        }
    }
}
