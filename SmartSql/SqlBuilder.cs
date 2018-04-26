using Microsoft.Extensions.Logging;
using SmartSql.Abstractions;
using SmartSql.Configuration.Statements;
using SmartSql.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmartSql
{
    public class SqlBuilder : ISqlBuilder
    {
        private readonly ILogger _logger;
        private readonly StatementMap _statementMap;

        public SqlBuilder(ILogger<SqlBuilder>  logger, StatementMap statementMap)
        {
            _logger = logger;
            _statementMap = statementMap;
        }

        public string BuildSql(RequestContext context)
        {
            var statement = _statementMap[context.FullSqlId];
            string sql = statement.BuildSql(context).Trim();
            _logger.LogDebug($"SqlBuilder BuildSql Statement.Id: {context.FullSqlId},Sql:[{sql}]");
            return sql;
        }
    }
}
