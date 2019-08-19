using System;
using System.Reflection;
using System.Text.RegularExpressions;
using SmartSql.Configuration;

namespace SmartSql.InvokeSync.Utils
{
    public class TableNameAnalyzer
    {
        private const string INSERT = "INSERT INTO";
        private const string UPDATE = "UPDATE";
        private const string DELETE = "DELETE";
        private readonly Regex _insertTokens;
        private readonly Regex _updateTokens;
        private readonly Regex _deleteTokens;

        public TableNameAnalyzer()
        {
            var regOptions = RegexOptions.Multiline | RegexOptions.CultureInvariant | RegexOptions.Compiled |
                             RegexOptions.IgnoreCase;
            _insertTokens = new Regex(@"(INSERT[\s]+INTO[\s]+)([\p{L}\p{N}_]+)", regOptions);
            _updateTokens = new Regex(@"(UPDATE[\s]+)([\p{L}\p{N}_]+)", regOptions);
            _deleteTokens = new Regex(@"(DELETE[\s]+)(FROM[\s]+){0,1}([\p{L}\p{N}_]+)", regOptions);
        }

        public String Replace(StatementType statementType, string sql, ReplaceEval replaceEval)
        {
            switch (statementType)
            {
                case StatementType type when type.HasFlag(StatementType.Insert):
                {
                    return _insertTokens.Replace(sql, match =>
                    {
                        var operation = match.Groups[1].Value;
                        var tableName = match.Groups[2].Value;
                        return replaceEval(tableName, operation);
                    });
                }

                case StatementType type when type.HasFlag(StatementType.Update):
                {
                    return _updateTokens.Replace(sql, match =>
                    {
                        var operation = match.Groups[1].Value;
                        var tableName = match.Groups[2].Value;
                        return replaceEval(tableName, operation);
                    });
                }

                case StatementType type when type.HasFlag(StatementType.Delete):
                {
                    return _deleteTokens.Replace(sql, match =>
                    {
                        var operation = match.Groups[1].Value + match.Groups[2].Value;
                        var tableName = match.Groups[3].Value;
                        return replaceEval(tableName, operation);
                    });
                }

                default: throw new ArgumentException($"can not Replace for StatementType:[{statementType}].");
            }
        }

        public delegate String ReplaceEval(string tableName, string operation);
    }
}