using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;
using SmartSql.Abstractions;
using System.Reflection;
using SmartSql.Exceptions;

namespace SmartSql.Configuration.Tags
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
        public override void BuildChildSql(RequestContext context)
        {
            context.Sql.Append(Open);
            var reqVal = (GetPropertyValue(context) as IEnumerable).GetEnumerator();
            reqVal.MoveNext();
            bool isDirectValue = IsDirectValue(reqVal.Current);
            if (string.IsNullOrEmpty(Key))
            {
                throw new SmartSqlException("[For] tag [Key] is required!");
            }
            if (ChildTags.Count == 0)
            {
                throw new SmartSqlException("[For] tag must have childTag!");
            }
            if (!(ChildTags[0] is SqlText childText))
            {
                throw new SmartSqlException("[For] ChildTag only support SqlText!");
            }
            var itemSqlStr = childText.BodyText;
            if (isDirectValue)
            {
                BuildItemSql_DirectValue(itemSqlStr, context);
            }
            else
            {
                BuildItemSql_NotDirectValue(itemSqlStr, context);
            }
            context.Sql.Append(Close);
        }

        private void BuildItemSql_DirectValue(string itemSqlStr, RequestContext context)
        {
            string dbPrefix = GetDbProviderPrefix(context);
            string dbPrefixs = $"{context.SmartSqlContext.DbPrefix}{context.SmartSqlContext.SmartDbPrefix}";

            var reqVal = GetPropertyValue(context) as IEnumerable;
            int item_index = 0;
            foreach (var itemVal in reqVal)
            {
                if (item_index > 0)
                {
                    context.Sql.AppendFormat(" {0} ", Separator);
                }
                string patternStr = $"([{dbPrefixs}]{Regex.Escape(Key)})";
                string key_name = $"{Key}{FOR_KEY_SUFFIX}{item_index}";
                context.RequestParameters.Add(key_name, itemVal);
                string key_name_dbPrefix = $"{dbPrefix}{key_name}";
                string item_sql = Regex.Replace(itemSqlStr
                                  , patternStr
                                  , key_name_dbPrefix
                                  , RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.CultureInvariant);

                context.Sql.AppendFormat("{0}", item_sql);
                item_index++;
            }
        }
        private void BuildItemSql_NotDirectValue(string itemSqlStr, RequestContext context)
        {
            string dbPrefix = GetDbProviderPrefix(context);
            string dbPrefixs = $"{context.SmartSqlContext.DbPrefix}{context.SmartSqlContext.SmartDbPrefix}";
            var reqVal = GetPropertyValue(context) as IEnumerable;
            int item_index = 0;
            foreach (var itemVal in reqVal)
            {
                if (item_index > 0)
                {
                    context.Sql.AppendFormat(" {0} ", Separator);
                }
                string item_sql = itemSqlStr;

                var properties = itemVal.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
                foreach (var property in properties)
                {
                    string patternStr = $"([{dbPrefixs}]{Regex.Escape(property.Name)})";
                    bool isHasParam = Regex.IsMatch(item_sql, patternStr);
                    if (!isHasParam) { continue; }

                    var propertyVal = property.GetValue(itemVal);
                    string key_name = $"{Key}{FOR_KEY_SUFFIX}{item_index}";
                    context.RequestParameters.Add(key_name, itemVal);
                    string key_name_dbPrefix = $"{dbPrefix}{key_name}";

                    context.RequestParameters.Add(key_name, propertyVal);

                    item_sql = Regex.Replace(item_sql
                                      , (patternStr)
                                      , key_name_dbPrefix
                                      , RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.CultureInvariant);
                }

                context.Sql.AppendFormat("{0}", item_sql);
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
