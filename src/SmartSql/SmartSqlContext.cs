using Microsoft.Extensions.Logging;
using SmartSql.Abstractions;
using SmartSql.Configuration;
using SmartSql.Configuration.Maps;
using SmartSql.Configuration.Statements;
using SmartSql.Exceptions;
using SmartSql.Utils;
using System;
using System.Collections.Generic;

namespace SmartSql
{
    public class SmartSqlContext
    {
        private readonly ILogger<SmartSqlContext> _logger;
        public SmartSqlContext(
            ILogger<SmartSqlContext> logger
            , SmartSqlMapConfig sqlMapConfig
            )
        {
            _logger = logger;
            SqlMapConfig = sqlMapConfig;
            Setup();
        }
        public SmartSqlMapConfig SqlMapConfig { get; internal set; }
        public Settings Settings { get { return SqlMapConfig.Settings; } }
        public Database Database { get { return SqlMapConfig.Database; } }
        public DbProvider DbProvider { get { return Database.DbProvider; } }
        public String SmartDbPrefix { get { return Settings.ParameterPrefix; } }
        public String DbPrefix { get { return Database.DbProvider.ParameterPrefix; } }
        public bool IgnoreParameterCase { get { return Settings.IgnoreParameterCase; } }
        public bool IsCacheEnabled { get { return Settings.IsCacheEnabled; } }
        public SqlParamAnalyzer SqlParamAnalyzer { get; private set; }
        public Dictionary<String, Statement> MappedStatement { get; private set; }
        public Dictionary<string, Configuration.Cache> MappedCache { get; private set; }
        public Dictionary<string, IList<Statement>> ExecuteMappedCacheFlush { get; private set; }
        public Dictionary<string, ResultMap> MappedResultMap { get; private set; }
        public Dictionary<string, ParameterMap> MappedParameterMap { get; private set; }
        public Dictionary<string, MultipleResultMap> MappedMultipleResultMap { get; private set; }
        public Statement GetStatement(string fullSqlId)
        {
            if (!MappedStatement.TryGetValue(fullSqlId, out Statement statement))
            {
                if (_logger.IsEnabled(LogLevel.Error))
                {
                    _logger.LogError($"StatementMap could not find statement:{fullSqlId}");
                }
                throw new SmartSqlException($"StatementMap could not find statement:{fullSqlId}");
            }
            return statement;
        }
        public Configuration.Cache GetCache(string fullCacheId)
        {
            if (!MappedCache.TryGetValue(fullCacheId, out Configuration.Cache cache))
            {
                if (_logger.IsEnabled(LogLevel.Error))
                {
                    _logger.LogError($"MappedCache could not find Cache:{fullCacheId}");
                }
                throw new SmartSqlException($"MappedCache could not find Cache:{fullCacheId}");
            }
            return cache;
        }
        public ResultMap GetResultMap(string fullResultMapId)
        {
            if (!MappedResultMap.TryGetValue(fullResultMapId, out ResultMap resultMap))
            {
                if (_logger.IsEnabled(LogLevel.Error))
                {
                    _logger.LogError($"MappedResultMap could not find ResultMap:{fullResultMapId}");
                }
                throw new SmartSqlException($"MappedResultMap could not find ResultMap:{fullResultMapId}");
            }
            return resultMap;
        }
        public ParameterMap GetParameterMap(string fullParameterMapId)
        {
            if (!MappedParameterMap.TryGetValue(fullParameterMapId, out ParameterMap parameterMap))
            {
                if (_logger.IsEnabled(LogLevel.Error))
                {
                    _logger.LogError($"MappedParameterMap could not find ParameterMap:{fullParameterMapId}");
                }
                throw new SmartSqlException($"MappedParameterMap could not find ParameterMap:{fullParameterMapId}");
            }
            return parameterMap;
        }
        public MultipleResultMap GetMultipleResultMap(string fullMultipleResultMapId)
        {
            if (!MappedMultipleResultMap.TryGetValue(fullMultipleResultMapId, out MultipleResultMap multipleResultMap))
            {
                if (_logger.IsEnabled(LogLevel.Error))
                {
                    _logger.LogError($"MappedMultipleResultMap could not find MultipleResultMap:{fullMultipleResultMapId}");
                }
                throw new SmartSqlException($"MappedMultipleResultMap could not find MultipleResultMap:{fullMultipleResultMapId}");
            }
            return multipleResultMap;
        }
        private void InitStatementMap()
        {
            lock (this)
            {
                if (_logger.IsEnabled(LogLevel.Debug))
                {
                    _logger.LogDebug($"StatementMap: Path:{SqlMapConfig.Path} Load MappedStatements Start!");
                }
                MappedStatement = new Dictionary<string, Statement>();
                foreach (var sqlmapKV in SqlMapConfig.SmartSqlMaps)
                {
                    InitSqlMapStatementMap(sqlmapKV.Value);
                }
                if (_logger.IsEnabled(LogLevel.Debug))
                {
                    _logger.LogDebug($"StatementMap: Path:{SqlMapConfig.Path} Load MappedStatements End!");
                }
            }
        }
        private void InitSqlMapStatementMap(SmartSqlMap sqlmap)
        {
            foreach (var statementKV in sqlmap.Statements)
            {
                var statement = statementKV.Value;
                if (MappedStatement.ContainsKey(statement.FullSqlId))
                {
                    throw new SmartSqlException($"SmartSqlMapConfig Load MappedStatements: StatementId:{statement.FullSqlId}  already exists!");
                }
                MappedStatement.Add(statement.FullSqlId, statement);
            }
        }
        private void InitMap()
        {
            MappedCache = new Dictionary<string, Configuration.Cache>();
            MappedResultMap = new Dictionary<string, ResultMap>();
            MappedParameterMap = new Dictionary<string, ParameterMap>();
            MappedMultipleResultMap = new Dictionary<string, MultipleResultMap>();
            foreach (var sqlMapKV in SqlMapConfig.SmartSqlMaps)
            {
                var sqlMap = sqlMapKV.Value;
                foreach (var cacheKV in sqlMap.Caches)
                {
                    MappedCache.Add(cacheKV.Key, cacheKV.Value);
                }
                foreach (var resultMapKV in sqlMap.ResultMaps)
                {
                    MappedResultMap.Add(resultMapKV.Key, resultMapKV.Value);
                }
                foreach (var parameterMapKV in sqlMap.ParameterMaps)
                {
                    MappedParameterMap.Add(parameterMapKV.Key, parameterMapKV.Value);
                }
                foreach (var multipleResultMapKV in sqlMap.MultipleResultMaps)
                {
                    MappedMultipleResultMap.Add(multipleResultMapKV.Key, multipleResultMapKV.Value);
                }
            }
        }
        private void InitExecuteMappedCacheFlush()
        {
            ExecuteMappedCacheFlush = new Dictionary<String, IList<Statement>>();
            foreach (var sqlMap in SqlMapConfig.SmartSqlMaps)
            {
                foreach (var statementKV in sqlMap.Value.Statements)
                {
                    var statement = statementKV.Value;
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
            SqlParamAnalyzer = new SqlParamAnalyzer(IgnoreParameterCase, DbPrefix);
            InitStatementMap();
            InitMap();
            InitExecuteMappedCacheFlush();
        }
    }
}
