using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace SmartSql.Abstractions.Cache
{
    public interface ICacheProvider
    {
        void Initialize(IDictionary properties);
        object this[CacheKey key, Type type]
        {
            get;
            set;
        }
        bool Remove(CacheKey key);
        void Flush();
    }
}
