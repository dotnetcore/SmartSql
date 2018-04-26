using Microsoft.Extensions.Logging;
using SmartSql.Configuration;
using SmartSql.Configuration.Statements;
using SmartSql.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmartSql
{
    public class StatementMap
    {
        private readonly ILogger<StatementMap> _logger;

        public StatementMap(ILogger<StatementMap> logger)
        {
            _logger = logger;
        }
        public IDictionary<String, Statement> Mapped { get; private set; }
        public Statement this[string fullSqlId]
        {
            get
            {
                if (Mapped.ContainsKey(fullSqlId))
                {
                    return Mapped[fullSqlId];
                }
                _logger.LogError($"StatementMap could not find statement:{fullSqlId}");
                throw new SmartSqlException($"StatementMap could not find statement:{fullSqlId}");
            }
        }

        public IDictionary<String, Statement> Load(SmartSqlMapConfig mapConfig)
        {
            IEnumerable<SmartSqlMap> smartSqlMaps = mapConfig.SmartSqlMaps;
            if (Mapped == null)
            {
                lock (this)
                {
                    if (Mapped == null)
                    {
                        _logger.LogDebug($"StatementMap: Path:{mapConfig.Path} Load MappedStatements Start!");
                        Mapped = new Dictionary<string, Statement>();
                        foreach (var sqlmap in smartSqlMaps)
                        {
                            foreach (var statement in sqlmap.Statements)
                            {
                                var statementId = $"{sqlmap.Scope}.{statement.Id}";
                                if (!Mapped.ContainsKey(statementId))
                                {
                                    Mapped.Add(statementId, statement);
                                }
                                else
                                {
                                    _logger.LogWarning($"SmartSqlMapConfig Load MappedStatements: StatementId:{statementId}  already exists!");
                                }
                            }
                        }
                        _logger.LogDebug($"StatementMap: Path:{mapConfig.Path} Load MappedStatements End!");
                    }
                }
            }
            return Mapped;
        }

    }
}
