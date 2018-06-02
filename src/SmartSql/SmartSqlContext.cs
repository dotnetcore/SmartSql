using Microsoft.Extensions.Logging;
using SmartSql.Configuration;
using SmartSql.Configuration.Maps;
using SmartSql.Configuration.Statements;
using SmartSql.Exceptions;
using System;
using System.Collections.Generic;

namespace SmartSql
{
    public class SmartSqlContext
    {
        private readonly ILogger<SmartSqlContext> _logger;
        public SmartSqlContext(
            ILogger<SmartSqlContext> logger
            , SmartSqlMapConfig sqlMapConfig)
        {
            _logger = logger;
            SqlMapConfig = sqlMapConfig;
            Setup();
        }
        public SmartSqlMapConfig SqlMapConfig { get; internal set; }
        public IDictionary<String, Statement> MappedStatement { get; private set; }
        public Settings Settings { get { return SqlMapConfig.Settings; } }
        public Database Database { get { return SqlMapConfig.Database; } }
        public DbProvider DbProvider { get { return Database.DbProvider; } }
        public String SmartDbPrefix { get { return Settings.ParameterPrefix; } }
        public String DbPrefix { get { return Database.DbProvider.ParameterPrefix; } }
        public bool IgnoreParameterCase { get { return Settings.IgnoreParameterCase; } }
        public bool IsCacheEnabled { get { return Settings.IsCacheEnabled; } }
        public Dictionary<string, IList<Statement>> ExecuteMappedCacheFlush { get; set; }
        public IList<Configuration.Cache> Caches { get; private set; }
        public Statement GetStatement(string fullSqlId)
        {
            if (MappedStatement.ContainsKey(fullSqlId))
            {
                return MappedStatement[fullSqlId];
            }
            if (_logger.IsEnabled(LogLevel.Error))
            {
                _logger.LogError($"StatementMap could not find statement:{fullSqlId}");
            }
            throw new SmartSqlException($"StatementMap could not find statement:{fullSqlId}");
        }
        public void InitStatementMap()
        {
            IEnumerable<SmartSqlMap> smartSqlMaps = SqlMapConfig.SmartSqlMaps;
            lock (this)
            {
                if (_logger.IsEnabled(LogLevel.Debug))
                {
                    _logger.LogDebug($"StatementMap: Path:{SqlMapConfig.Path} Load MappedStatements Start!");
                }
                MappedStatement = new Dictionary<string, Statement>();
                foreach (var sqlmap in smartSqlMaps)
                {
                    InitSqlMapStatementMap(sqlmap);
                }
                if (_logger.IsEnabled(LogLevel.Debug))
                {
                    _logger.LogDebug($"StatementMap: Path:{SqlMapConfig.Path} Load MappedStatements End!");
                }
            }
        }
        public void InitSqlMapStatementMap(SmartSqlMap sqlmap)
        {
            foreach (var statement in sqlmap.Statements)
            {
                var statementId = $"{sqlmap.Scope}.{statement.Id}";
                if (MappedStatement.ContainsKey(statementId))
                {
                    MappedStatement.Remove(statementId);
                    if (_logger.IsEnabled(LogLevel.Warning))
                    {
                        _logger.LogWarning($"SmartSqlMapConfig Load MappedStatements: StatementId:{statementId}  already exists!");
                    }
                }
                MappedStatement.Add(statementId, statement);
            }
        }
        public void InitCache()
        {
            Caches = new List<Configuration.Cache>();
            foreach (var sqlMap in SqlMapConfig.SmartSqlMaps)
            {
                foreach (var cache in sqlMap.Caches)
                {
                    Caches.Add(cache);
                }
            }
        }
        public void InitExecuteMappedCacheFlush()
        {
            ExecuteMappedCacheFlush = new Dictionary<String, IList<Statement>>();
            foreach (var sqlMap in SqlMapConfig.SmartSqlMaps)
            {
                foreach (var statement in sqlMap.Statements)
                {
                    if (statement.Cache == null) { continue; }
                    if (statement.Cache.FlushOnExecutes == null) { continue; }
                    foreach (var triggerStatement in statement.Cache.FlushOnExecutes)
                    {
                        if (!ExecuteMappedCacheFlush.TryGetValue(triggerStatement.Statement, out IList<Statement> triggerStatements))
                        {
                            triggerStatements = new List<Statement>();
                            ExecuteMappedCacheFlush.Add(triggerStatement.Statement, triggerStatements);
                        }
                        triggerStatements.Add(statement);
                    }
                }
            }
        }
        public void Setup()
        {
            InitStatementMap();
            InitCache();
            InitExecuteMappedCacheFlush();
        }
    }
}
