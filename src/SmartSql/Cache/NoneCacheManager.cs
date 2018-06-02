using SmartSql.Abstractions;
using SmartSql.Abstractions.Cache;
using SmartSql.Abstractions.DbSession;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmartSql.Cache
{
    public class NoneCacheManager : ICacheManager
    {
        public static ICacheManager Instance = new NoneCacheManager();

        public void Dispose()
        {
        }

        public void RequestCommitted(IDbConnectionSession dbSession)
        {
        }

        public void RequestExecuted(IDbConnectionSession dbSession, RequestContext context)
        {
        }

        public void TryAdd<T>(RequestContext context, T cacheItem)
        {
           
        }

        public bool TryGet<T>(RequestContext context, out T cachedResult)
        {
            cachedResult = default(T);
            return false;
        }
    }
}
