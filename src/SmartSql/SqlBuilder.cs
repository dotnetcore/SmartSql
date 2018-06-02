using Microsoft.Extensions.Logging;
using SmartSql.Abstractions;
using SmartSql.Configuration.Statements;
using SmartSql.Exceptions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace SmartSql
{
    public class SqlBuilder : ISqlBuilder
    {
        private readonly ILogger _logger;
        private readonly SmartSqlContext _smartSqlContext;
        private readonly ConcurrentDictionary<String, RequestContext> _cachedRequest = new ConcurrentDictionary<String, RequestContext>();
        public SqlBuilder(ILogger<SqlBuilder> logger,
            SmartSqlContext smartSqlContext)
        {
            _logger = logger;
            _smartSqlContext = smartSqlContext;
        }

        public string BuildSql(RequestContext context)
        {
            if (_cachedRequest.ContainsKey(context.Key))
            {
                var cachedRequest = _cachedRequest[context.Key];
                context.Sql = cachedRequest.Sql;
                context.RealSql = cachedRequest.RealSql;
                context.RequestParameters = context.RequestParameters;
                context.Statement = cachedRequest.Statement;
            }
            else
            {
                context.Sql = new StringBuilder();
                var statement = _smartSqlContext.GetStatement(context.FullSqlId);
                context.Statement = statement;
                statement.BuildSql(context);
                context.RealSql = context.Sql.ToString().Trim().Replace("\r", " ").Replace(_smartSqlContext.SmartDbPrefix, _smartSqlContext.DbPrefix);
                _cachedRequest.TryAdd(context.Key, context);
            }

            if (_logger.IsEnabled(LogLevel.Debug))
            {
                _logger.LogDebug($"SqlBuilder BuildSql Statement.Id: {context.FullSqlId},Sql:[{context.RealSql}]");
            }
            return context.RealSql;
        }

    }
}
