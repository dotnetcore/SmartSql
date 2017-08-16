using SmartSql.Abstractions.Cache;
using System;
using System.Collections.Generic;
using System.Text;
using SmartSql.Abstractions;
using SmartSql.SqlMap;
using SmartSql.Exceptions;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace SmartSql
{
    public class CacheManager : ICacheManager
    {
        private readonly ILogger _logger;
        public ISmartSqlMapper SmartSqlMapper { get; set; }
        public IDictionary<String, Statement> MappedStatements => SmartSqlMapper.SqlMapConfig.MappedStatements;
        public IDictionary<String, DateTime> MappedLastFlushTimes { get; } = new Dictionary<String, DateTime>();
        public Queue<RequestContext> RequestQueue { get; set; } = new Queue<RequestContext>();
        private IDictionary<String, IList<Statement>> _mappedTriggerFlushs;
        public IDictionary<String, IList<Statement>> MappedTriggerFlushs
        {
            get
            {
                if (_mappedTriggerFlushs == null)
                {
                    lock (this)
                    {
                        if (_mappedTriggerFlushs == null)
                        {
                            _logger.LogDebug($"CacheManager Load MappedTriggerFlushs !");
                            _mappedTriggerFlushs = new Dictionary<String, IList<Statement>>();
                            foreach (var sqlMap in SmartSqlMapper.SqlMapConfig.SmartSqlMaps)
                            {
                                foreach (var statement in sqlMap.Statements)
                                {
                                    if (statement.Cache == null) { continue; }
                                    if (statement.Cache.FlushOnExecutes == null) { continue; }
                                    foreach (var triggerStatement in statement.Cache.FlushOnExecutes)
                                    {
                                        IList<Statement> triggerStatements = null;
                                        if (_mappedTriggerFlushs.ContainsKey(triggerStatement.Statement))
                                        {
                                            triggerStatements = _mappedTriggerFlushs[triggerStatement.Statement];
                                        }
                                        else
                                        {
                                            triggerStatements = new List<Statement>();
                                            _mappedTriggerFlushs[triggerStatement.Statement] = triggerStatements;
                                        }
                                        triggerStatements.Add(statement);
                                    }
                                }
                            }
                        }
                    }
                }

                return _mappedTriggerFlushs;
            }
        }
        public CacheManager(ILoggerFactory loggerFactory, ISmartSqlMapper smartSqlMapper)
        {
            _logger = loggerFactory.CreateLogger<CacheManager>();
            SmartSqlMapper = smartSqlMapper;
        }
        private void Enqueue(RequestContext context)
        {
            RequestQueue.Enqueue(context);
        }
        public void FlushQueue()
        {
            while (RequestQueue.Count > 0)
            {
                var reqContext = RequestQueue.Dequeue();
                Flush(reqContext);
            }
        }
        public void ClearQueue()
        {
            RequestQueue.Clear();
        }

        public void TriggerFlush(RequestContext context)
        {
            var session = SmartSqlMapper.SessionStore.LocalSession;
            if (session != null
                && session.Transaction != null)
            {
                Enqueue(context);
            }
            else
            {
                Flush(context);
            }
        }

        private void Flush(RequestContext context)
        {
            String exeFullSqlId = context.FullSqlId;
            if (MappedTriggerFlushs.ContainsKey(exeFullSqlId))
            {
                lock (this)
                {
                    IList<Statement> triggerStatements = MappedTriggerFlushs[exeFullSqlId];
                    foreach (var statement in triggerStatements)
                    {
                        _logger.LogDebug($"CacheManager FlushCache.OnInterval FullSqlId:{statement.FullSqlId},ExeFullSqlId:{exeFullSqlId}");
                        MappedLastFlushTimes[statement.FullSqlId] = DateTime.Now;
                        statement.CacheProvider.Flush();
                    }
                }
            }
        }


        public object this[RequestContext context, Type type]
        {
            get
            {
                string fullSqlId = context.FullSqlId;

                if (!MappedStatements.ContainsKey(fullSqlId))
                {
                    throw new SmartSqlException($"CacheManager can not find Statement.Id:{fullSqlId}");
                }
                var statement = MappedStatements[fullSqlId];
                if (statement.Cache == null) { return null; }

                lock (this)
                {
                    FlushByInterval(statement);
                }
                var cacheKey = new CacheKey(context);
                var cache = statement.CacheProvider[cacheKey, type];
                _logger.LogDebug($"CacheManager GetCache FullSqlId:{fullSqlId}，Success:{cache != null} !");
                return cache;
            }
            set
            {
                string fullSqlId = context.FullSqlId;
                if (!MappedStatements.ContainsKey(fullSqlId))
                {
                    throw new SmartSqlException($"CacheManager can not find Statement.Id:{fullSqlId}");
                }
                var statement = MappedStatements[fullSqlId];
                if (statement.Cache == null) { return; }
                lock (this)
                {
                    FlushByInterval(statement);
                }
                var cacheKey = new CacheKey(context);
                _logger.LogDebug($"CacheManager SetCache FullSqlId:{fullSqlId}");
                statement.CacheProvider[cacheKey, type] = value;
            }
        }

        private void FlushByInterval(Statement statement)
        {
            if (statement.Cache.FlushInterval.Interval.Ticks == 0) { return; }
            String fullSqlId = statement.FullSqlId;
            DateTime lastFlushTime = DateTime.Now;
            if (!MappedLastFlushTimes.ContainsKey(fullSqlId))
            {
                MappedLastFlushTimes[fullSqlId] = lastFlushTime;
            }
            else
            {
                lastFlushTime = MappedLastFlushTimes[fullSqlId];
            }
            var lastInterval = DateTime.Now - lastFlushTime;
            if (lastInterval >= statement.Cache.FlushInterval.Interval)
            {
                Flush(statement, lastInterval);
            }
        }

        private void Flush(Statement statement, TimeSpan lastInterval)
        {
            _logger.LogDebug($"CacheManager FlushCache.OnInterval FullSqlId:{statement.FullSqlId},LastInterval:{lastInterval}");
            MappedLastFlushTimes[statement.FullSqlId] = DateTime.Now;
            statement.CacheProvider.Flush();
        }

        public void ResetMappedCaches()
        {
            lock (this)
            {
                _mappedTriggerFlushs = null;
            }
        }
    }
}
