using SmartSql.Abstractions.Cache;
using System;
using System.Collections.Generic;
using SmartSql.Abstractions;
using Microsoft.Extensions.Logging;
using SmartSql.Configuration.Statements;
using SmartSql.Abstractions.DbSession;

namespace SmartSql.Cahce
{
    public class CacheManager : ICacheManager
    {
        private readonly ILogger<CacheManager> _logger;
        private readonly SmartSqlContext _smartSqlContext;
        private readonly IDbConnectionSessionStore _dbSessionStore;
        private readonly IDictionary<Guid, SessionRequest> _cachedSessionRequest = new Dictionary<Guid, SessionRequest>();
        private IDictionary<string, DateTime> _cacheMappedLastFlushTime;
        private readonly System.Threading.Timer _timer;
        private readonly TimeSpan _defaultDueTime = TimeSpan.FromMinutes(1);
        private readonly TimeSpan _defaultPeriodTime = TimeSpan.FromMinutes(1);
        public CacheManager(
            ILogger<CacheManager> logger
            , SmartSqlContext smartSqlContext
            , IDbConnectionSessionStore dbSessionStore)
        {
            _logger = logger;
            _smartSqlContext = smartSqlContext;
            _dbSessionStore = dbSessionStore;
            InitCacheMappedLastFlushTime();
            _timer = new System.Threading.Timer(Run, null, _defaultDueTime, _defaultPeriodTime);
        }

        public void RequestExecuted(IDbConnectionSession dbSession, RequestContext context)
        {
            var sessionId = dbSession.Id;
            if (dbSession.Transaction == null)
            {
                FlushOnExecute(context);
            }
            else
            {

                if (_cachedSessionRequest.TryGetValue(sessionId, out SessionRequest sessionRequest))
                {
                    sessionRequest.Requests.Add(context);
                }
                else
                {
                    sessionRequest = new SessionRequest
                    {
                        SessionId = sessionId,
                        Requests = new List<RequestContext> { context }
                    };
                    _cachedSessionRequest.Add(sessionId, sessionRequest);
                }
            }
        }
        public void RequestCommitted(IDbConnectionSession dbSession)
        {
            var sessionId = dbSession.Id;
            if (_cachedSessionRequest.TryGetValue(sessionId, out SessionRequest sessionRequest))
            {
                foreach (var context in sessionRequest.Requests)
                {
                    FlushOnExecute(context);
                }
                _cachedSessionRequest.Remove(sessionId);
            }
        }

        private void Run(object state)
        {
            FlushInterval();
        }

        private void InitCacheMappedLastFlushTime()
        {
            _cacheMappedLastFlushTime = new Dictionary<string, DateTime>();
            foreach (var cacheKV in _smartSqlContext.MappedCache)
            {
                var cache = cacheKV.Value;
                if (cache.FlushInterval == null) { continue; }
                _cacheMappedLastFlushTime.Add(cache.Id, DateTime.Now);
            }
        }

        private void FlushInterval()
        {
            try
            {
                foreach (var cacheKV in _smartSqlContext.MappedCache)
                {
                    var cache = cacheKV.Value;
                    if (cache.FlushInterval == null) { continue; }
                    var lastFlushTime = _cacheMappedLastFlushTime[cache.Id];
                    var nextFlushTime = lastFlushTime.Add(cache.FlushInterval.Interval);
                    if (DateTime.Now >= nextFlushTime)
                    {
                        cache.Provider.Flush();
                        UpdateCacheFlushTime(cache.Id, DateTime.Now);
                        if (_logger.IsEnabled(LogLevel.Debug))
                        {
                            _logger.LogDebug($"CacheManager FlushInterval Cache.Id:{cache.Id}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(new EventId(ex.HResult), ex, ex.Message);

            }

        }

        private void FlushOnExecute(RequestContext requestContext)
        {
            try
            {
                if (_smartSqlContext.ExecuteMappedCacheFlush.TryGetValue(requestContext.FullSqlId, out IList<Statement> needFlushStatements))
                {
                    foreach (var needFlushStatement in needFlushStatements)
                    {
                        needFlushStatement.Cache.Provider.Flush();
                        UpdateCacheFlushTime(needFlushStatement.Cache.Id, DateTime.Now);
                    }
                    if (_logger.IsEnabled(LogLevel.Debug))
                    {
                        _logger.LogDebug($"CacheManager FlushOnExecute FullSqlId:{requestContext.FullSqlId}");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(new EventId(ex.HResult), ex, ex.Message);
            }
        }

        private void UpdateCacheFlushTime(string cacheId, DateTime flushTime)
        {
            _cacheMappedLastFlushTime[cacheId] = flushTime;
        }

        public void Dispose()
        {
            _timer.Dispose();
        }

        public bool TryGet<T>(RequestContext context, out T cachedResult)
        {
            cachedResult = default(T);
            if (context.Cache == null) { return false; }
            var cacheKey = new CacheKey(context);
            var cached = context.Cache.Provider.Contains(cacheKey);
            if (_logger.IsEnabled(LogLevel.Debug))
            {
                _logger.LogDebug($"CacheManager GetCache StatementKey:{context.StatementKey}，Success:{cached} !");
            }
            if (cached)
            {
                var cachedType = typeof(T);
                var cache = context.Cache.Provider[cacheKey, cachedType];
                cachedResult = (T)cache;
            }
            return cached;
        }

        public void TryAdd<T>(RequestContext context, T cacheItem)
        {
            if (context.Cache == null) { return; }
            var cachedType = typeof(T);
            var cacheKey = new CacheKey(context);
            context.Cache.Provider[cacheKey, cachedType] = cacheItem;
            if (_logger.IsEnabled(LogLevel.Debug))
            {
                _logger.LogDebug($"CacheManager SetCache StatementKey:{context.StatementKey}");
            }
        }
    }
    public class SessionRequest
    {
        public Guid SessionId { get; set; }
        public IList<RequestContext> Requests { get; set; }
    }
}


