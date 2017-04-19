using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using SmartSql.Abstractions.Cache;

namespace SmartSql.Cache.Fifo
{
    public class FifoCacheProvider : ICacheProvider
    {
        private int _cacheSize = 0;
        private Hashtable _cache = null;
        private IList _keyList = null;

        public FifoCacheProvider()
        {
            _cacheSize = 100;
            _cache = Hashtable.Synchronized(new Hashtable());
            _keyList = ArrayList.Synchronized(new ArrayList());
        }

        public bool Remove(CacheKey cacheKey)
        {
            object o = this[cacheKey, typeof(object)];

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
                return _cache[cacheKey];
            }
            set
            {
                _cache[cacheKey] = value;
                _keyList.Add(cacheKey);
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
