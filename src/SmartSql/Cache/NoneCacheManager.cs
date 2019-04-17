using System;
using System.Collections.Generic;
using System.Text;
using SmartSql.DbSession;

namespace SmartSql.Cache
{
    public class NoneCacheManager : ICacheManager
    {
        public void BindSessionEventHandler(IDbSession dbSession)
        {

        }

        public void Dispose()
        {
            
        }

        public void ExecuteRequest(ExecutionContext executionContext)
        {
           
        }

        public bool TryGetValue(ExecutionContext executionContext, out object cacheItem)
        {
            cacheItem = null;
            return false;
        }
    }
}
