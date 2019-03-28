using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SmartSql.Configuration.Tags
{
    public class SqlText : ITag
    {
        private readonly Regex _sqlInParamsTokens;
        private readonly string _dbPrefix;
        public string BodyText { get; private set; }
        public ITag Parent { get; set; }
        public Statement Statement { get; set; }
        private readonly bool _hasInSyntax = false;
        public SqlText(string bodyText, string dbPrefix)
        {
            BodyText = bodyText;
            _dbPrefix = dbPrefix;
            _sqlInParamsTokens = new Regex(@"[i|I][n|N]\s*[" + dbPrefix + @"]([\p{L}\p{N}_.]+)");
            _hasInSyntax = _sqlInParamsTokens.IsMatch(bodyText);
        }

        public void BuildSql(AbstractRequestContext context)
        {
            if (!_hasInSyntax)
            {
                context.SqlBuilder.Append(BodyText);
                return;
            }
            var sql = _sqlInParamsTokens.Replace(BodyText, match =>
              {
                  var paramName = match.Groups[1].Value;
                  if (!context.Parameters.TryGetValue(paramName, out var sqlParameter))
                  {
                      return match.Value;
                  }
                  var paramVal = sqlParameter.Value;
                  bool isString = paramVal is String;
                  if (!(paramVal is IEnumerable enumParams) || isString) return match.Value;
                  StringBuilder inParamSql = new StringBuilder();
                  inParamSql.Append("In (");
                  int itemIndex = 0;
                  foreach (var itemVal in enumParams)
                  {
                      string itemParamName = $"{paramName}_{itemIndex}";
                      inParamSql.AppendFormat("{0}{1},", _dbPrefix, itemParamName);
                      context.Parameters.TryAdd(itemParamName, itemVal);
                      itemIndex++;
                  }
                  if (itemIndex > 0)
                  {
                      inParamSql.Remove(inParamSql.Length - 1, 1);
                  }
                  inParamSql.Append(")");
                  return inParamSql.ToString();
              });
            context.SqlBuilder.Append(sql);
        }

        public bool IsCondition(AbstractRequestContext context)
        {
            return true;
        }

    }
}
