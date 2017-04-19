using SmartSql.Abstractions.Cache;
using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace SmartSql.Cache.None
{
    public class NoneCacheProvider : ICacheProvider
    {
        public object this[CacheKey key, Type type]
        {
            get { return null; }
            set { }
        }

        public void Flush()
        {
           
        }

        public void Initialize(IDictionary properties)
        {
            
        }

        public bool Remove(CacheKey key)
        {
            return true;
        }
    }
}
