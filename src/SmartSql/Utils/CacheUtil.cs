using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Concurrent;

namespace SmartSql.Utils
{
    public static class CacheUtil<TType, TKey, TCacheItem>
    {
        public static Type CacheType = typeof(TType);

        private static readonly ConcurrentDictionary<TKey, TCacheItem> _cacheItems =
            new ConcurrentDictionary<TKey, TCacheItem>();

        public static TCacheItem GetOrAdd(TKey key, Func<TKey, TCacheItem> factory)
        {
            return _cacheItems.GetOrAdd(key, factory);
        }
    }
}