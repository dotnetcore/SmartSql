using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace SmartSql.Abstractions.Cache.Lru
{
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

        public bool Remove(CacheKey key)
        {
            object o = this[key,typeof(object)];

            _keyList.Remove(key);
            _cache.Remove(key);
            return true;
        }

        public void Flush()
        {
            _cache.Clear();
            _keyList.Clear();
        }
        public object this[CacheKey key, Type type]
        {
            get
            {
                _keyList.Remove(key);
                _keyList.Add(key);
                return _cache[key];
            }
            set
            {
                _cache[key] = value;
                _keyList.Add(key);
                if (_keyList.Count > _cacheSize)
                {
                    object oldestKey = _keyList[0];
                    _keyList.Remove(0);
                    _cache.Remove(oldestKey);
                }
            }
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
