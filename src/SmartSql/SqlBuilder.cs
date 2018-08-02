using Microsoft.Extensions.Logging;
using SmartSql.Abstractions;
using SmartSql.Abstractions.Config;
using System;
using System.Collections.Concurrent;
using System.Text;

namespace SmartSql
{
    public class SqlBuilder : ISqlBuilder
    {
        private readonly ILogger _logger;
        private readonly IConfigLoader _configLoader;
        private readonly SmartSqlContext _smartSqlContext;
        private readonly ConcurrentDictionary<String, RequestContext> _cachedRequest = new ConcurrentDictionary<String, RequestContext>();
        public SqlBuilder(ILogger<SqlBuilder> logger
            , SmartSqlContext smartSqlContext
            , IConfigLoader configLoader)
        {
            _logger = logger;
            _configLoader = configLoader;
            _smartSqlContext = smartSqlContext;
            _configLoader.OnChanged += _configLoader_OnChanged;
        }

        private void _configLoader_OnChanged(object sender, OnChangedEventArgs eventArgs)
        {
            _cachedRequest.Clear();
            if (_logger.IsEnabled(LogLevel.Debug))
            {
                _logger.LogDebug($"SqlBuilder.CachedRequestContext clear on ConfigLoader.OnChanged!");
            }
        }

        public string BuildSql(RequestContext context)
        {
            if (!context.IsStatementSql)
            {
                return context.RealSql;
            }

            if (_cachedRequest.ContainsKey(context.Key))
            {
                var cachedRequest = _cachedRequest[context.Key];
                context.Sql = cachedRequest.Sql;
                context.RealSql = cachedRequest.RealSql;
            }
            else
            {
                context.Sql = new StringBuilder();
                context.Statement.BuildSql(context);
                context.RealSql = context.Sql.ToString().Trim().Replace(_smartSqlContext.SmartDbPrefix, _smartSqlContext.DbPrefix);
                _cachedRequest.TryAdd(context.Key, context);
            }

            return context.RealSql;
        }

    }
}
