using SmartSql.Abstractions.DbSession;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmartSql.Abstractions.Cache
{
    public interface ICacheManager : IDisposable
    {
        void RequestExecuted(IDbConnectionSession dbSession, RequestContext context);
        void RequestCommitted(IDbConnectionSession dbSession);
        bool TryGet<T>(RequestContext context, out T cachedResult);
        void TryAdd<T>(RequestContext context, T cacheItem);
    }
}
