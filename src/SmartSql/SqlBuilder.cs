using Microsoft.Extensions.Logging;
using SmartSql.Abstractions;
using System;
using System.Collections.Concurrent;
using System.Text;

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
            if (!String.IsNullOrEmpty(context.RealSql))
            {
                context.IsStatementSql = false;
            }
            else
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
            }

            return context.RealSql;
        }

    }
}
