using System;
using System.Collections.Generic;
using System.Text;

namespace SmartSql.Cache
{
    public interface ICacheKeyBuilder
    {
        CacheKey Build(RequestContext reqContext);
    }
}
