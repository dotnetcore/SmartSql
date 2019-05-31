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
            if (executionContext.Request.Cache == null)
            {
                InvokeNext<TResult>(executionContext);
                return;
            }

            if (executionContext.DbSession.Transaction == null
                && _cacheManager.TryGetCache(executionContext, out var cacheItem))
            {
                executionContext.Result.SetData(cacheItem, true);
            }
            else
            {
                InvokeNext<TResult>(executionContext);
                _cacheManager.TryAddCache(executionContext);
            }
        }

        public override async Task InvokeAsync<TResult>(ExecutionContext executionContext)
        {
            if (executionContext.Request.Cache == null)
            {
                await InvokeNextAsync<TResult>(executionContext);
                return;
            }

            if (executionContext.DbSession.Transaction == null
                && _cacheManager.TryGetCache(executionContext, out var cacheItem))
            {
                executionContext.Result.SetData(cacheItem, true);
            }
            else
            {
                await InvokeNextAsync<TResult>(executionContext);
                _cacheManager.TryAddCache(executionContext);
            }
        }
    }
}