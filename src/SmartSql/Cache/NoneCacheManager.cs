using System;
using System.Collections.Generic;
using System.Text;
using SmartSql.DbSession;

namespace SmartSql.Cache
{
    public class NoneCacheManager : ICacheManager
    {
        
        public void Dispose()
        {
            
        }

        public void ListenInvokeSucceeded()
        {
            
        }

        public void Reset()
        {
            
        }

        public bool TryGetCache(ExecutionContext executionContext, out object cacheItem)
        {
            cacheItem = null;
            return false;
        }

        public bool TryAddCache(ExecutionContext executionContext)
        {
            return false;
        }
    }
}
