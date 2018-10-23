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
        public SqlBuilder(ILogger<SqlBuilder> logger)
        {
            _logger = logger;
        }

        public string BuildSql(RequestContext context)
        {
            if (!context.IsStatementSql)
            {
                return context.RealSql;
            }

            context.Sql = new StringBuilder();
            context.Statement.BuildSql(context);
            context.RealSql = context.Sql.ToString().Trim();
            return context.RealSql;
        }

    }
}
