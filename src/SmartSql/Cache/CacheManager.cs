using SmartSql.Configuration;
using SmartSql.DbSession;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;

namespace SmartSql.Cache
{

    public class CacheManager : ICacheManager
    {
        private readonly ConcurrentDictionary<Guid, ConcurrentQueue<ExecutionContext>> _sessionMappedExecutionQueue;
        private ConcurrentDictionary<String, IList<Configuration.Cache>> _statementMappedFlushCache;
        private ConcurrentDictionary<Configuration.Cache, DateTime> _cacheMappedLastFlushTime;
        private readonly Timer _timer;
        private readonly TimeSpan _defaultDueTime = TimeSpan.FromMinutes(1);
        private readonly TimeSpan _defaultPeriodTime = TimeSpan.FromMinutes(1);
        private readonly SmartSqlConfig _smartSqlConfig;
        private readonly ILogger _logger;
        public CacheManager(SmartSqlConfig smartSqlConfig)
        {
            _smartSqlConfig = smartSqlConfig;
            _logger = _smartSqlConfig.LoggerFactory.CreateLogger<CacheManager>();
            InitCacheMapped();
            _sessionMappedExecutionQueue = new ConcurrentDictionary<Guid, ConcurrentQueue<ExecutionContext>>();
            _timer = new Timer(FlushOnInterval, null, _defaultDueTime, _defaultPeriodTime);
        }
        private void InitCacheMapped()
        {
            _statementMappedFlushCache = new ConcurrentDictionary<String, IList<Configuration.Cache>>();
            _cacheMappedLastFlushTime = new ConcurrentDictionary<Configuration.Cache, DateTime>();
            foreach (var sqlMap in _smartSqlConfig.SqlMaps.Values)
            {
                foreach (var cache in sqlMap.Caches.Values)
                {
                    if (cache.FlushOnExecutes != null)
                    {
                        foreach (var onExecute in cache.FlushOnExecutes)
                        {
                            if (!_statementMappedFlushCache.TryGetValue(onExecute.Statement, out var mappedCaches))
                            {
                                mappedCaches = new List<Configuration.Cache>();
                                _statementMappedFlushCache.TryAdd(onExecute.Statement, mappedCaches);
                            }
                            mappedCaches.Add(cache);
                        }
                    }
                    if (cache.FlushInterval != null && cache.FlushInterval.Interval != TimeSpan.MinValue)
                    {
                        _cacheMappedLastFlushTime.TryAdd(cache, DateTime.Now);
                    }
                }
            }
        }
        public void ExecuteRequest(ExecutionContext executionContext)
        {
            if (_logger.IsEnabled(LogLevel.Debug))
            {
                _logger.LogDebug("ExecuteRequest Start");
            }
            if (executionContext.DbSession.Transaction == null)
            {
                HandleCache(executionContext);
            }
            else
            {
                if (!_sessionMappedExecutionQueue.TryGetValue(executionContext.DbSession.Id, out var executionQueue))
                {
                    executionQueue = new ConcurrentQueue<ExecutionContext>();
                    _sessionMappedExecutionQueue.TryAdd(executionContext.DbSession.Id, executionQueue);
                }
                executionQueue.Enqueue(executionContext);
            }
            if (_logger.IsEnabled(LogLevel.Debug))
            {
                _logger.LogDebug("ExecuteRequest End");
            }
        }

        private void FlushOnExecuted(ExecutionContext executionContext)
        {
            if (_logger.IsEnabled(LogLevel.Debug))
            {
                _logger.LogDebug($"FlushOnExecuted Start");
            }
            if (executionContext.Request.IsStatementSql)
            {
                if (_statementMappedFlushCache.TryGetValue(executionContext.Request.FullSqlId, out var caches))
                {
                    foreach (var cache in caches)
                    {
                        FlushCache(cache);
                    }
                }
            }
            if (_logger.IsEnabled(LogLevel.Debug))
            {
                _logger.LogDebug($"FlushOnExecuted End");
            }
        }

        private void FlushCache(Configuration.Cache cache)
        {
            if (_logger.IsEnabled(LogLevel.Debug))
            {
                _logger.LogDebug($"FlushCache Cache.Id:{cache.Id} .");
            }
            cache.Provider.Flush();
            if (_cacheMappedLastFlushTime.TryGetValue(cache, out var lastFlushTime))
            {
                _cacheMappedLastFlushTime[cache] = DateTime.Now;
            }
        }

        private void FlushOnInterval(object state)
        {
            if (_logger.IsEnabled(LogLevel.Debug))
            {
                _logger.LogDebug($"FlushOnInterval Start");
            }
            foreach (var cacheKV in _cacheMappedLastFlushTime)
            {
                var cache = cacheKV.Key;
                var lastFlushTime = cacheKV.Value;
                var nextFlushTime = lastFlushTime.Add(cache.FlushInterval.Interval);
                if (DateTime.Now >= nextFlushTime)
                {
                    FlushCache(cache);
                }
            }
            if (_logger.IsEnabled(LogLevel.Debug))
            {
                _logger.LogDebug($"FlushOnInterval End");
            }
        }
        public void BindSessionEventHandler(IDbSession dbSession)
        {
            dbSession.Committed += DbSession_Committed;
            dbSession.Disposed += DbSession_Disposed;
            dbSession.Rollbacked += DbSession_Rollbacked;
        }

        private void DbSession_Rollbacked(object sender, DbSessionEventArgs eventArgs)
        {
            var dbSession = sender as IDbSession;
            _sessionMappedExecutionQueue.TryRemove(dbSession.Id, out var exeQueue);
        }

        private void DbSession_Disposed(object sender, DbSessionEventArgs eventArgs)
        {
            var dbSession = sender as IDbSession;
            if (_sessionMappedExecutionQueue.TryGetValue(dbSession.Id, out var executionQueue))
            {
                _sessionMappedExecutionQueue.TryRemove(dbSession.Id, out var exeQueue);
            }
        }

        private void DbSession_Committed(object sender, DbSessionEventArgs eventArgs)
        {
            var dbSession = sender as IDbSession;
            if (_sessionMappedExecutionQueue.TryGetValue(dbSession.Id, out var executionQueue))
            {
                HandleCacheQueue(executionQueue);
                _sessionMappedExecutionQueue.TryRemove(dbSession.Id, out var exeQueue);
            }
        }

        private void HandleCacheQueue(ConcurrentQueue<ExecutionContext> executionQueue)
        {
            while (executionQueue.TryDequeue(out var executionContext))
            {
                HandleCache(executionContext);
            }
        }

        private void HandleCache(ExecutionContext executionContext)
        {
            FlushOnExecuted(executionContext);
            var cache = executionContext.Request.Cache;
            if (cache != null)
            {
                var cacheKey = new CacheKey(executionContext.Request);
                var isSuccess = cache.Provider.TryAdd(cacheKey, executionContext.Result.GetData());
                if (_logger.IsEnabled(LogLevel.Debug))
                {
                    _logger.LogDebug($"HandleCache Cache.Id:{cache.Id} try add from Cache.Key:{cacheKey.Key} ->{isSuccess}!");
                }
            }
        }

        public bool TryGetValue(ExecutionContext executionContext, out object cacheItem)
        {
            var cache = executionContext.Request.Cache;
            if (cache == null)
            {
                cacheItem = default;
                return false;
            }
            var cacheKey = new CacheKey(executionContext.Request);
            bool isSuccess = cache.Provider.TryGetValue(cacheKey, out cacheItem);
            if (_logger.IsEnabled(LogLevel.Debug))
            {
                _logger.LogDebug($"TryGetValue Cache.Key:{cacheKey.Key}，Success:{isSuccess} !");
            }
            return isSuccess;
        }

        public void Dispose()
        {
            if (_logger.IsEnabled(LogLevel.Debug))
            {
                _logger.LogDebug("Dispose.");
            }
            _timer.Dispose();
            foreach (var sqlMap in _smartSqlConfig.SqlMaps.Values)
            {
                foreach (var cache in sqlMap.Caches.Values)
                {
                    cache.Provider.Dispose();
                }
            }
        }
    }
}
