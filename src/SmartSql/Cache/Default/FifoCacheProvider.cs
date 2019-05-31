using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;

namespace SmartSql.Cache.Default
{
    /// <summary>
    /// First In First Out
    /// </summary>
    public class FifoCacheProvider : ICacheProvider
    {
        private const String CACHE_SIZE = "CacheSize";
        private int _cacheSize = 100;
        private Dictionary<CacheKey, Object> _cache;
        private Queue<CacheKey> _cacheKeys;

        public void Dispose()
        {
            Flush();
        }

        public void Flush()
        {
            lock (this)
            {
                _cacheKeys.Clear();
                _cache.Clear();
            }
        }

        public bool SupportExpire => false;

        public void Initialize(IDictionary<string, object> properties)
        {
            if (properties.Value(CACHE_SIZE, out int cacheSize))
            {
                _cacheSize = cacheSize;
            }
            _cache = new Dictionary<CacheKey, object>(_cacheSize);
            _cacheKeys = new Queue<CacheKey>(_cacheSize);
        }
        public bool TryAdd(CacheKey cacheKey, object cacheItem)
        {
            lock (this)
            {
                if (_cache.ContainsKey(cacheKey))
                {
                    return false;
                }
                _cacheKeys.Enqueue(cacheKey);
                _cache.Add(cacheKey, cacheItem);
                if (_cacheKeys.Count > _cacheSize)
                {
                    var removedKey = _cacheKeys.Dequeue();
                    _cache.Remove(removedKey);
                }
            }
            return true;
        }
        public bool TryGetValue(CacheKey cacheKey, out object cacheItem)
        {
            lock (this)
            {
                return _cache.TryGetValue(cacheKey, out cacheItem);
            }
        }
    }
}
