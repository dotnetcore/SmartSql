using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace SmartSql.Utils
{
    public static class ExecutionContextExtensions
    {
        public static string FormatSql(this ExecutionContext executionContext, bool withParameterValue)
        {
            StringBuilder stringBuilder = new StringBuilder();
            var sourceParameters = executionContext.Request.Parameters.DbParameters.Values;
            string dbParameterStr = string.Join(",", sourceParameters.Select(p => $"{p.ParameterName}={p.Value}"));
            stringBuilder.AppendFormat("Statement.Id:[{0}],", executionContext.Request.FullSqlId);
            stringBuilder.Append("Sql:");
            stringBuilder.AppendLine();
            stringBuilder.Append(executionContext.Request.RealSql);
            stringBuilder.AppendLine();
            stringBuilder.AppendFormat("Parameters:[{0}]", dbParameterStr);

            if (!withParameterValue)
            {
                return stringBuilder.ToString();
            }

            stringBuilder.AppendLine();
            stringBuilder.Append("Sql with parameter value: ");
            stringBuilder.AppendLine();
            string realSql = FormatSqlWithParameters(executionContext);
            stringBuilder.Append(realSql);

            return stringBuilder.ToString();
        }

        private static string FormatSqlWithParameters(this ExecutionContext executionContext)
        {
            var sourceParameters = executionContext.Request.Parameters.DbParameters.Values;
            return executionContext.SmartSqlConfig.SqlParamAnalyzer.Replace(executionContext.Request.RealSql,
                (paramName, nameWithPrefix) =>
                {
                    var paramNameCompare = executionContext.SmartSqlConfig.Settings.IgnoreParameterCase
                        ? StringComparison.CurrentCultureIgnoreCase
                        : StringComparison.CurrentCulture;
                    var dbParam =
                        sourceParameters.FirstOrDefault(
                            m => String.Equals(m.ParameterName, paramName, paramNameCompare));
                    if (dbParam == null)
                    {
                        return nameWithPrefix;
                    }

                    if (dbParam.Value == DBNull.Value)
                    {
                        return "NULL";
                    }

                    switch (dbParam.DbType)
                    {
                        case DbType.AnsiString:
                        case DbType.AnsiStringFixedLength:
                        case DbType.DateTime:
                        case DbType.DateTime2:
                        case DbType.DateTimeOffset:
                        case DbType.Guid:
                        case DbType.String:
                        case DbType.StringFixedLength:
                        case DbType.Time:
                        case DbType.Xml:
                        {
                            return $"'{dbParam.Value}'";
                        }

                        case DbType.Boolean:
                        {
                            return ((bool) dbParam.Value) ? "1" : "0";
                        }
                    }

                    return dbParam.Value.ToString();
                });
        }
    }
}