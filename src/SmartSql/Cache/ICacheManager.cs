using SmartSql.DbSession;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SmartSql.Cache
{
    public interface ICacheManager : IDisposable
    {
        void Reset();
        bool TryGetCache(ExecutionContext executionContext, out object cacheItem);
        bool TryAddCache(ExecutionContext executionContext);
    }
}