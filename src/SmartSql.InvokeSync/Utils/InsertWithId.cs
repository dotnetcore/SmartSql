using System;
using System.Text.RegularExpressions;

namespace SmartSql.InvokeSync.Utils
{
    public class InsertWithId
    {
        private readonly Regex _insertFirstIdTokens;
        private readonly Regex _insertSecondIdTokens;

        public InsertWithId()
        {
            var regOptions = RegexOptions.Multiline | RegexOptions.CultureInvariant | RegexOptions.Compiled |
                             RegexOptions.IgnoreCase;
            _insertFirstIdTokens = new Regex(@"(INSERT[\s]+INTO[\s]+)([\p{L}\p{N}_\[\]]+)[\s]*(\()", regOptions);
            _insertSecondIdTokens = new Regex(@"(Values[\s]*)(\()", regOptions);
        }

        public String Replace(string sql, string idColName, string idParamName, string paramPrefix)
        {
            sql = _insertFirstIdTokens.Replace(sql, (match) => $"{match.Value}{idColName},");
            return _insertSecondIdTokens.Replace(sql, match => $"{match.Value}{paramPrefix}{idParamName},");
        }
    }
}