using SmartSql.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;
using SmartSql.Abstractions;
using System.Reflection;
using SmartSql.Exceptions;

namespace SmartSql.SqlMap.Tags
{
    public class For : Tag
    {
        public const string FOR_KEY_SUFFIX = "_For_";
        public override TagType Type => TagType.For;
        public string Open { get; set; }
        public string Separator { get; set; }
        public string Close { get; set; }
        public string Key { get; set; }

        public override bool IsCondition(RequestContext context)
        {
            var reqVal = GetPropertyValue(context);
            if (reqVal == null) { return false; }
            if (reqVal is IEnumerable)
            {
                return (reqVal as IEnumerable).GetEnumerator().MoveNext();
            }
            return false;
        }

        public override string BuildSql(RequestContext context)
        {
            if (IsCondition(context))
            {
                return BuildChildSql(context).ToString();
            }
            return String.Empty;
        }
        public override StringBuilder BuildChildSql(RequestContext context)
        {
            StringBuilder strBuilder = new StringBuilder();
            strBuilder.AppendFormat(" {0}", Prepend);
            strBuilder.Append(Open);
            var reqVal = (GetPropertyValue(context) as IEnumerable).GetEnumerator();
            reqVal.MoveNext();
            bool isDirectValue = IsDirectValue(reqVal.Current);
            if (isDirectValue)
            {
                BuildItemSql_DirectValue(strBuilder, context);
            }
            else
            {
                BuildItemSql_NotDirectValue(strBuilder, context);
            }
            strBuilder.Append(Close);
            return strBuilder;
        }

        private void BuildItemSql_DirectValue(StringBuilder sqlStrBuilder, RequestContext context)
        {
            if (string.IsNullOrEmpty(Key))
            {
                throw new SmartSqlException("[For] tag [Key] is required!");
            }

            var itemSqlStr = base.BuildChildSql(context).ToString();
            string dbPrefix = GetDbProviderPrefix(context);
            var reqVal = GetPropertyValue(context) as IEnumerable;
            int item_index = 0;
            foreach (var itemVal in reqVal)
            {
                if (item_index > 0)
                {
                    sqlStrBuilder.AppendFormat(" {0} ", Separator);
                }
                string key_name = $"{dbPrefix}{Key}{FOR_KEY_SUFFIX}{item_index}";
                context.DapperParameters.Add(key_name, itemVal);
                string item_sql = Regex.Replace(itemSqlStr
                                  , ("([?@:]" + Regex.Escape(Key) + @")(?!\w)(\s+(?i)unknown(?-i))?")
                                  , key_name
                                  , RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.CultureInvariant);

                sqlStrBuilder.AppendFormat("{0}", item_sql);
                item_index++;
            }
        }
        private void BuildItemSql_NotDirectValue(StringBuilder sqlStrBuilder, RequestContext context)
        {
            var itemSqlStr = base.BuildChildSql(context).ToString();
            string dbPrefix = GetDbProviderPrefix(context);
            var reqVal = GetPropertyValue(context) as IEnumerable;
            int item_index = 0;
            foreach (var itemVal in reqVal)
            {
                if (item_index > 0)
                {
                    sqlStrBuilder.AppendFormat(" {0} ", Separator);
                }
                string item_sql = itemSqlStr;

                var properties = itemVal.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
                foreach (var property in properties)
                {
                    string patternStr = "([?@:]" + Regex.Escape(property.Name) + @")(?!\w)(\s+(?i)unknown(?-i))?";
                    bool isHasParam = Regex.IsMatch(item_sql, patternStr);
                    if (!isHasParam) { continue; }

                    var propertyVal = property.GetValue(itemVal);
                    string key_name = $"{dbPrefix}{property.Name}{FOR_KEY_SUFFIX}{item_index}";
                    context.DapperParameters.Add(key_name, propertyVal);

                    item_sql = Regex.Replace(item_sql
                                      , (patternStr)
                                      , key_name
                                      , RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.CultureInvariant);
                }

                sqlStrBuilder.AppendFormat("{0}", item_sql);
                item_index++;
            }
        }
        private bool IsDirectValue(object obj)
        {
            bool isString = obj is String;
            if (isString) { return true; }
            bool isValueType = obj is ValueType;
            if (isValueType) { return true; }
            return false;
        }
    }
}
