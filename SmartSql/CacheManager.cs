using SmartSql.Abstractions.Cache;
using System;
using System.Collections.Generic;
using System.Text;
using SmartSql.Abstractions;
using SmartSql.SqlMap;
using SmartSql.Exceptions;
using SmartSql.Abstractions.Logging;
using System.Linq;

namespace SmartSql
{
    public class CacheManager : ICacheManager
    {
        private static readonly ILog _logger = LogManager.GetLogger(typeof(CacheManager));
        public ISmartSqlMapper SmartSqlMapper { get; private set; }
        public IDictionary<String, Statement> MappedStatements => SmartSqlMapper.SqlMapConfig.MappedStatements;
        public IDictionary<String, DateTime> MappedLastFlushTimes { get; } = new Dictionary<String, DateTime>();

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

        public void TriggerFlush(RequestContext context)
        {
            String exeFullSqlId = context.FullSqlId;
            if (MappedTriggerFlushs.ContainsKey(exeFullSqlId))
            {
                lock (this)
                {
                    IList<Statement> triggerStatements = MappedTriggerFlushs[exeFullSqlId];
                    foreach (var statement in triggerStatements)
                    {
                        _logger.Debug($"SmartSql.CacheManager FlushCache.OnInterval FullSqlId:{statement.FullSqlId},ExeFullSqlId:{exeFullSqlId}");
                        MappedLastFlushTimes[statement.FullSqlId] = DateTime.Now;
                        statement.CacheProvider.Flush();
                    }
                }
            }
        }


        public CacheManager(ISmartSqlMapper smartSqlMapper)
        {
            SmartSqlMapper = smartSqlMapper;
        }
        public object this[RequestContext context, Type type]
        {
            get
            {
                string fullSqlId = context.FullSqlId;
                var statement = MappedStatements[fullSqlId];
                if (statement == null)
                {
                    throw new SmartSqlException($"SmartSql.CacheManager can not find Statement.Id:{fullSqlId}");
                }
                if (statement.Cache == null) { return null; }

                lock (this)
                {
                    FlushByInterval(statement);
                }
                var cacheKey = new CacheKey(context);
                var cache = statement.CacheProvider[cacheKey, type];
                _logger.Debug($"SmartSql.CacheManager GetCache FullSqlId:{fullSqlId}，Success:{cache != null} !");
                return cache;
            }
            set
            {
                string fullSqlId = context.FullSqlId;
                var statement = MappedStatements[fullSqlId];
                if (statement == null)
                {
                    throw new SmartSqlException($"SmartSql.CacheManager can not find Statement.Id:{fullSqlId}");
                }
                if (statement.Cache == null) { }
                lock (this)
                {
                    FlushByInterval(statement);
                }
                var cacheKey = new CacheKey(context);
                statement.CacheProvider[cacheKey, type] = value;
                _logger.Debug($"SmartSql.CacheManager SetCache FullSqlId:{fullSqlId}");
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
                Flush(statement, fullSqlId, lastInterval);
            }
        }

        private void Flush(Statement statement, string fullSqlId, TimeSpan lastInterval)
        {
            _logger.Debug($"SmartSql.CacheManager FlushCache.OnInterval FullSqlId:{fullSqlId},LastInterval:{lastInterval}");
            MappedLastFlushTimes[fullSqlId] = DateTime.Now;
            statement.CacheProvider.Flush();
        }
    }
}
