using SmartSql.DbSession;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SmartSql.Cache
{
    public interface ICacheManager : IDisposable
    {
        bool TryGetValue(ExecutionContext executionContext, out object cacheItem);
        void ExecuteRequest(ExecutionContext executionContext);
        void BindSessionEventHandler(IDbSession dbSession);
    }
}
