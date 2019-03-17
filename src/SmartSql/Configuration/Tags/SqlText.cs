using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SmartSql.Configuration.Tags
{
    public class SqlText : ITag
    {
        private Regex _sqlInParamsTokens;
        private readonly string _dbPrefix;
        public string BodyText { get; private set; }
        public ITag Parent { get; set; }
        public Statement Statement { get; set; }
        private bool _hasInSyntax = false;
        public SqlText(string bodyText, string dbPrefix)
        {
            BodyText = bodyText;
            _dbPrefix = dbPrefix;
            _sqlInParamsTokens = new Regex(@"[i|I][n|N]\s*[" + dbPrefix + @"]([\p{L}\p{N}_.]+)");
            _hasInSyntax = _sqlInParamsTokens.IsMatch(bodyText);
        }

        public void BuildSql(RequestContext context)
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
                  if (paramVal is IEnumerable && !isString)
                  {
                      var enumParams = paramVal as IEnumerable;
                      StringBuilder inParamSql = new StringBuilder();
                      inParamSql.Append("In (");
                      int item_Index = 0;
                      foreach (var itemVal in enumParams)
                      {
                          string itemParamName = $"{paramName}_{item_Index}";
                          inParamSql.AppendFormat("{0}{1},", _dbPrefix, itemParamName);
                          context.Parameters.TryAdd(itemParamName, itemVal);
                          item_Index++;
                      }
                      if (item_Index > 0)
                      {
                          inParamSql.Remove(inParamSql.Length - 1, 1);
                      }
                      inParamSql.Append(")");
                      return inParamSql.ToString();
                  }
                  return match.Value;
              });
            context.SqlBuilder.Append(sql);
        }

        public bool IsCondition(RequestContext context)
        {
            return true;
        }

    }
}
