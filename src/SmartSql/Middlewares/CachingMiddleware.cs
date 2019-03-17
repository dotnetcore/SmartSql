using SmartSql.Configuration;
using SmartSql.Exceptions;
using System.Threading.Tasks;
using SmartSql.Cache;

namespace SmartSql.Middlewares
{
    public class CachingMiddleware : IMiddleware
    {
        private readonly ICacheManager _cacheManager;
        public IMiddleware Next { get; set; }
        public CachingMiddleware(SmartSqlConfig smartSqlConfig)
        {
            _cacheManager = smartSqlConfig.CacheManager;
        }
        public void Invoke<TResult>(ExecutionContext executionContext)
        {
            if (_cacheManager.TryGetValue(executionContext, out var cacheItem))
            {
                executionContext.Result.SetData(cacheItem, true);
                return;
            }
            Next.Invoke<TResult>(executionContext);
            _cacheManager.ExecuteRequest(executionContext);
        }

        public async Task InvokeAsync<TResult>(ExecutionContext executionContext)
        {
            if (_cacheManager.TryGetValue(executionContext, out var cacheItem))
            {
                executionContext.Result.SetData(cacheItem, true);
                return;
            }
            await Next.InvokeAsync<TResult>(executionContext);
            _cacheManager.ExecuteRequest(executionContext);
        }
    }
}
