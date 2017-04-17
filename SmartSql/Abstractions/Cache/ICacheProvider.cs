using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace SmartSql.Abstractions.Cache
{
    public interface ICacheProvider
    {
        void Initialize(IDictionary properties);
        object this[object key]
        {
            get;
            set;
        }
        object Remove(object key);
        void Flush();
    }
}
