using SmartSql.Configuration.Statements;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;

namespace SmartSql.Utils
{
    public class SqlParamAnalyzer
    {
        private Regex _sqlParamsTokens;
        public SqlParamAnalyzer(bool ignoreCase, string dbPrefix)
        {
            var regOptions = RegexOptions.Multiline | RegexOptions.CultureInvariant | RegexOptions.Compiled;
            if (ignoreCase)
            {
                regOptions = regOptions | RegexOptions.IgnoreCase;
            }
            _sqlParamsTokens = new Regex(@"[" + dbPrefix + @"]([\p{L}\p{N}_.]+)", regOptions);
        }
        public IEnumerable<string> Analyse(string realSql)
        {
            var matchs = _sqlParamsTokens.Matches(realSql);
            return matchs.Cast<Match>().Select(m => m.Groups[1].Value);
        }

        public string Replace(string realSql, Func<string, string, string> action)
        {
            if (!_sqlParamsTokens.IsMatch(realSql)) { return realSql; }
            return _sqlParamsTokens.Replace(realSql, match =>
             {
                 string paramName = match.Groups[1].Value;
                 string nameWithPrefix = match.Value;
                 return action(paramName, nameWithPrefix);
             });
        }
    }
}
