using SmartSql.Abstractions;
using SmartSql.Abstractions.DbSession;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;
using SmartSql.Abstractions.Command;

namespace SmartSql
{
    public class PreparedCommand : IPreparedCommand
    {
        Regex _sqlParamsTokens = new Regex(@"([\p{L}\p{N}_]+)=[?@:\$]([\p{L}\p{N}_]+)", RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.CultureInvariant | RegexOptions.Compiled);

        private readonly IDbConnectionSessionStore _sessionStore;
        private readonly ISqlBuilder _sqlBuilder;

        public PreparedCommand(IDbConnectionSessionStore sessionStore, ISqlBuilder sqlBuilder)
        {
            _sessionStore = sessionStore;
            _sqlBuilder = sqlBuilder;
        }
        public IDbCommand Prepare(RequestContext context)
        {
            var dbSession = _sessionStore.GetOrAddDbSession(context.DataSource);
            var dbCommand = dbSession.Connection.CreateCommand();
            string sql = _sqlBuilder.BuildSql(context);
            var matches = _sqlParamsTokens.Matches(sql);
            foreach (Match match in matches)
            {
                var cmdParameter = dbCommand.CreateParameter();
                string paramName = match.Groups[2].Value;
                cmdParameter.ParameterName = paramName;
                var paramVal = context.RequestParameters[paramName];
                cmdParameter.Value = paramVal;
                dbCommand.Parameters.Add(cmdParameter);
            }
            return dbCommand;
        }
    }
}
