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



        public bool TryGetValue(ExecutionContext executionContext, out object cacheItem)
        {
            cacheItem = null;
            return false;
        }

        public void HandleCache(ExecutionContext executionContext)
        {
           
        }
    }
}
