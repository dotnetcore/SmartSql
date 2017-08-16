using Microsoft.Extensions.Logging;
using SmartSql.Abstractions;
using SmartSql.Exceptions;
using SmartSql.SqlMap;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmartSql
{
    public class SqlBuilder : ISqlBuilder
    {
        private readonly ILogger _logger;
        public ISmartSqlMapper SmartSqlMapper { get; private set; }
        public IDictionary<String, Statement> MappedStatements => SmartSqlMapper.SqlMapConfig.MappedStatements;

        public SqlBuilder(ILoggerFactory loggerFactory, ISmartSqlMapper smartSqlMapper)
        {
            _logger = loggerFactory.CreateLogger<SqlBuilder>();
            SmartSqlMapper = smartSqlMapper;
        }

        public string BuildSql(RequestContext context)
        {
            if (!MappedStatements.ContainsKey(context.FullSqlId))
            {
                _logger.LogError($"SqlBuilder BuildSql Not Find Statement.Id: {context.FullSqlId}.");
                throw new SmartSqlException($"SmartSqlMapper could not find statement:{context.FullSqlId}");
            }
            var statement = MappedStatements[context.FullSqlId];

            return BuildSql(context, statement);
        }

        public string BuildSql(RequestContext context, Statement statement)
        {
            if (statement == null)
            {
                _logger.LogError($"SqlBuilder BuildSql Not Find Statement.Id: {context.FullSqlId}.");
                throw new SmartSqlException($"SmartSqlMapper could not find statement:{context.FullSqlId}");
            }
            string sql = statement.BuildSql(context).Trim();
            _logger.LogDebug($"SqlBuilder BuildSql Statement.Id: {context.FullSqlId},Sql:[{sql}]");
            return sql;
        }
    }
}
