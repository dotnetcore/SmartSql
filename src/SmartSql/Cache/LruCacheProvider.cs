using SmartSql.Abstractions.Cache;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace SmartSql.Cache
{
    /// <summary>
    /// Least Recently Used
    /// </summary>
    public class LruCacheProvider : ICacheProvider
    {
        private int _cacheSize = 0;
        private Hashtable _cache = null;
        private IList _keyList = null;

        public LruCacheProvider()
        {
            _cacheSize = 100;
            _cache = Hashtable.Synchronized(new Hashtable());
            _keyList = ArrayList.Synchronized(new ArrayList());
        }

        public bool Remove(CacheKey cacheKey)
        {
            object o = this[cacheKey,typeof(object)];

            _keyList.Remove(cacheKey);
            _cache.Remove(cacheKey);
            return true;
        }

        public void Flush()
        {
            _cache.Clear();
            _keyList.Clear();
        }
        public object this[CacheKey cacheKey, Type type]
        {
            get
            {
                _keyList.Remove(cacheKey);
                _keyList.Add(cacheKey);
                return _cache[cacheKey];
            }
            set
            {
                _cache[cacheKey] = value;
                _keyList.Add(cacheKey);
                if (_keyList.Count > _cacheSize)
                {
                    object oldestKey = _keyList[0];
                    _keyList.RemoveAt(0);
                    _cache.Remove(oldestKey);
                }
            }
        }
        public bool Contains(CacheKey key)
        {
            return _cache.ContainsKey(key);
        }
        public void Initialize(IDictionary properties)
        {
            string size = (string)properties["CacheSize"]; ;
            if (size != null)
            {
                _cacheSize = Convert.ToInt32(size);
            }
        }

    }
}
