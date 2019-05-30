using SmartSql.Configuration;
using SmartSql.Exceptions;
using System.Threading.Tasks;
using SmartSql.Cache;

namespace SmartSql.Middlewares
{
    public class CachingMiddleware : AbstractMiddleware
    {
        private readonly ICacheManager _cacheManager;
        public CachingMiddleware(SmartSqlConfig smartSqlConfig)
        {
            _cacheManager = smartSqlConfig.CacheManager;
        }
        public override void Invoke<TResult>(ExecutionContext executionContext)
        {
            if (executionContext.DbSession.Transaction == null
                && _cacheManager.TryGetValue(executionContext, out var cacheItem))
            {
                executionContext.Result.SetData(cacheItem, true);
                return;
            }
            InvokeNext<TResult>(executionContext);
        }

        public override async Task InvokeAsync<TResult>(ExecutionContext executionContext)
        {
            if (executionContext.DbSession.Transaction == null
                && _cacheManager.TryGetValue(executionContext, out var cacheItem))
            {
                executionContext.Result.SetData(cacheItem, true);
                return;
            }
            await InvokeNextAsync<TResult>(executionContext);
        }
    }
}
