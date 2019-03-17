using SmartSql.Cache;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmartSql.DistributedCache
{
    public interface ICacheKeyGegenerator
    {
        String Gegenerate(CacheKey cacheKey);
    }
}
