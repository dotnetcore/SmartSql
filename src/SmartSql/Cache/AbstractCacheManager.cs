using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Extensions.Logging;
using SmartSql.Configuration;

namespace SmartSql.Cache
{
    public abstract class AbstractCacheManager : ICacheManager, ISetupSmartSql
    {
        protected ConcurrentDictionary<string, IList<Configuration.Cache>> StatementMappedFlushCache { get; set; }
        protected ConcurrentDictionary<Configuration.Cache, DateTime> CacheMappedLastFlushTime { get; set; }
        protected Timer Timer { get; private set; }
        protected TimeSpan DueTime { get; } = TimeSpan.FromMinutes(1);
        protected TimeSpan PeriodTime { get; } = TimeSpan.FromMinutes(1);
        protected SmartSqlConfig SmartSqlConfig { get; private set; }
        protected ILogger Logger { get; private set; }

        public void Reset()
        {
            StatementMappedFlushCache = new ConcurrentDictionary<String, IList<Configuration.Cache>>();
            CacheMappedLastFlushTime = new ConcurrentDictionary<Configuration.Cache, DateTime>();
            foreach (var sqlMap in SmartSqlConfig.SqlMaps.Values)
            {
                foreach (var cache in sqlMap.Caches.Values)
                {
                    if (cache.FlushOnExecutes != null)
                    {
                        foreach (var onExecute in cache.FlushOnExecutes)
                        {
                            if (!StatementMappedFlushCache.TryGetValue(onExecute.Statement, out var mappedCaches))
                            {
                                mappedCaches = new List<Configuration.Cache>();
                                StatementMappedFlushCache.TryAdd(onExecute.Statement, mappedCaches);
                            }

                            mappedCaches.Add(cache);
                        }
                    }

                    if (cache.FlushInterval != null && cache.FlushInterval.Interval != TimeSpan.MinValue)
                    {
                        CacheMappedLastFlushTime.TryAdd(cache, DateTime.Now);
                    }
                }
            }
        }

        protected void FlushOnExecuted(string fullSqlId)
        {
            if (StatementMappedFlushCache.TryGetValue(fullSqlId, out var caches))
            {
                if (Logger.IsEnabled(LogLevel.Debug))
                {
                    Logger.LogDebug($"FlushOnExecuted -> FullSqlId:[{fullSqlId}] Start");
                }
                foreach (var cache in caches)
                {
                    FlushCache(cache);
                }
                if (Logger.IsEnabled(LogLevel.Debug))
                {
                    Logger.LogDebug($"FlushOnExecuted  -> FullSqlId:[{fullSqlId}] End");
                }
            }
        }

        private void FlushCache(Configuration.Cache cache)
        {
            if (Logger.IsEnabled(LogLevel.Debug))
            {
                Logger.LogDebug($"FlushCache Cache.Id:{cache.Id} .");
            }

            cache.Provider.Flush();
            if (CacheMappedLastFlushTime.TryGetValue(cache, out _))
            {
                CacheMappedLastFlushTime[cache] = DateTime.Now;
            }
        }

        private void FlushOnInterval(object state)
        {
            if (Logger.IsEnabled(LogLevel.Debug))
            {
                Logger.LogDebug("FlushOnInterval Start");
            }

            foreach (var cacheKV in CacheMappedLastFlushTime)
            {
                var cache = cacheKV.Key;
                var lastFlushTime = cacheKV.Value;
                var nextFlushTime = lastFlushTime.Add(cache.FlushInterval.Interval);
                if (DateTime.Now < nextFlushTime) continue;
                if (!cache.Provider.SupportExpire)
                {
                    FlushCache(cache);
                }
            }

            if (Logger.IsEnabled(LogLevel.Debug))
            {
                Logger.LogDebug("FlushOnInterval End");
            }
        }

        protected abstract void ListenInvokeSucceeded();

        public virtual bool TryAddCache(ExecutionContext executionContext)
        {
            var cache = executionContext.Request.Cache;
            if (cache == null)
            {
                return false;
            }

            var cacheKey = executionContext.Request.EnsureCacheKey();

            var isSuccess = cache.Provider.TryAdd(cacheKey, executionContext.Result.GetData());
            if (Logger.IsEnabled(LogLevel.Debug))
            {
                Logger.LogDebug(
                    $"HandleCache Cache.Id:{cache.Id} try add from Cache.Key:{cacheKey.Key} ->{isSuccess}!");
            }

            return isSuccess;
        }


        public virtual bool TryGetCache(ExecutionContext executionContext, out object cacheItem)
        {
            var cache = executionContext.Request.Cache;
            if (cache == null)
            {
                cacheItem = default;
                return false;
            }

            var cacheKey = executionContext.Request.EnsureCacheKey();

            bool isSuccess = cache.Provider.TryGetValue(cacheKey, out cacheItem);
            if (Logger.IsEnabled(LogLevel.Debug))
            {
                Logger.LogDebug($"TryGetValue Cache.Key:{cacheKey.Key}，Success:{isSuccess} !");
            }

            return isSuccess;
        }

        public virtual void Dispose()
        {
            if (Logger.IsEnabled(LogLevel.Debug))
            {
                Logger.LogDebug("Dispose.");
            }

            Timer.Dispose();
            foreach (var sqlMap in SmartSqlConfig.SqlMaps.Values)
            {
                foreach (var cache in sqlMap.Caches.Values)
                {
                    cache.Provider.Dispose();
                }
            }
        }

        public virtual void SetupSmartSql(SmartSqlBuilder smartSqlBuilder)
        {
            SmartSqlConfig = smartSqlBuilder.SmartSqlConfig;
            Logger = SmartSqlConfig.LoggerFactory.CreateLogger<AbstractCacheManager>();
            Reset();
            Timer = new Timer(FlushOnInterval, null, DueTime, PeriodTime);
            ListenInvokeSucceeded();
        }
    }
}