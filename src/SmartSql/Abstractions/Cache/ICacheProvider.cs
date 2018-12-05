using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace SmartSql.Abstractions.Cache
{
    public interface ICacheProvider : IDisposable
    {
        void Initialize(IDictionary properties);
        object this[CacheKey key, Type type]
        {
            get;
            set;
        }
        bool Contains(CacheKey key);
        bool Remove(CacheKey key);
        void Flush();
    }
}
