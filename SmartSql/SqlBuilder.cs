using SmartSql.Abstractions;
using SmartSql.Abstractions.Logging;
using SmartSql.Exceptions;
using SmartSql.SqlMap;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmartSql
{
    public class SqlBuilder : ISqlBuilder
    {
        private static readonly ILog _logger = LogManager.GetLogger(typeof(SqlBuilder));
        public ISmartSqlMapper SmartSqlMapper { get; private set; }
        public IDictionary<String, Statement> MappedStatements { get { return SmartSqlMapper.SqlMapConfig.MappedStatements; } }

        public SqlBuilder(ISmartSqlMapper smartSqlMapper)
        {
            SmartSqlMapper = smartSqlMapper;
        }

        public string BuildSql(RequestContext context)
        {
            var statement = MappedStatements[context.FullSqlId];
            return BuildSql(context, statement);
        }

        public string BuildSql(RequestContext context, Statement statement)
        {
            if (statement == null)
            {
                _logger.Error($"SmartSql.SqlBuilder BuildSql Not Find Statement.Id: {context.FullSqlId}.");
                throw new SmartSqlException($"SmartSqlMapper could not find statement:{context.FullSqlId}");
            }
            string sql = statement.BuildSql(context);
            _logger.Debug($"SmartSql.SqlBuilder BuildSql Statement.Id: {context.FullSqlId},Sql:[{sql}]");
            return sql;
        }
    }
}
