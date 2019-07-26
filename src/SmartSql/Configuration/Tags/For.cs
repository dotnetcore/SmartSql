using System;
using System.Collections;
using System.Linq;
using SmartSql.Data;
using SmartSql.Exceptions;
using SmartSql.Reflection;

namespace SmartSql.Configuration.Tags
{
    public class For : Tag
    {
        public const string FOR_KEY_SUFFIX = "_For_";
        public string Open { get; set; }
        public string Separator { get; set; }
        public string Close { get; set; }
        public string Key { get; set; }

        public override bool IsCondition(AbstractRequestContext context)
        {
            var reqVal = EnsurePropertyValue(context);
            if (reqVal == null)
            {
                return false;
            }

            if (reqVal is IEnumerable)
            {
                return (reqVal as IEnumerable).GetEnumerator().MoveNext();
            }

            return false;
        }

        public override void BuildChildSql(AbstractRequestContext context)
        {
            context.SqlBuilder.Append(Open);
            var reqVal = (EnsurePropertyValue(context) as IEnumerable).GetEnumerator();
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

            context.SqlBuilder.Append(Close);
        }

        private void BuildItemSql_DirectValue(string itemSqlStr, AbstractRequestContext context)
        {
            var reqVal = (EnsurePropertyValue(context) as IEnumerable);
            int itemIndex = 0;
            string dbPrefix = GetDbProviderPrefix(context);
            foreach (var itemVal in reqVal)
            {
                if (itemIndex > 0)
                {
                    context.SqlBuilder.AppendFormat(" {0} ", Separator);
                }

                var itemSql = context.ExecutionContext.SmartSqlConfig.SqlParamAnalyzer
                    .Replace(itemSqlStr, (paramName, nameWithPrefix) =>
                    {
                        if (String.Compare(paramName, Key,
                                context.ExecutionContext.SmartSqlConfig.Settings.IgnoreParameterCase) != 0)
                        {
                            return nameWithPrefix;
                        }

                        string keyName = $"{Key}{FOR_KEY_SUFFIX}_{itemIndex}";
                        context.Parameters.TryAdd(keyName, itemVal);
                        return $"{dbPrefix}{keyName}";
                    });
                context.SqlBuilder.AppendFormat("{0}", itemSql);
                itemIndex++;
            }
        }

        private void BuildItemSql_NotDirectValue(string itemSqlStr, AbstractRequestContext context)
        {
            var reqVal = (EnsurePropertyValue(context) as IEnumerable);
            int itemIndex = 0;
            string dbPrefix = GetDbProviderPrefix(context);
            var keyPrefix = $"{Key}.";
            foreach (var itemVal in reqVal)
            {
                if (itemIndex > 0)
                {
                    context.SqlBuilder.AppendFormat(" {0} ", Separator);
                }
                var itemParams = RequestConvert.Instance.ToSqlParameters(itemVal,
                    context.ExecutionContext.SmartSqlConfig.Settings.IgnoreParameterCase);
                
                var itemSql = context.ExecutionContext.SmartSqlConfig.SqlParamAnalyzer
                    .Replace(itemSqlStr, (paramName, nameWithPrefix) =>
                    {
                        if (paramName.StartsWith(keyPrefix))
                        {
                            paramName = paramName.Substring(keyPrefix.Length);
                        }
                        Parameter paramMap = null;
                        context.ParameterMap?.Parameters?.TryGetValue(paramName, out paramMap);
                        var propertyName = paramMap != null ? paramMap.Property : paramName;

                        if (!itemParams.TryGetValue(propertyName, out SqlParameter propertyVal))
                        {
                            return nameWithPrefix;
                        }
                        
                        string keyName = $"{Key}{FOR_KEY_SUFFIX}_{propertyVal.Name}_{itemIndex}";
                        if (!context.Parameters.ContainsKey(keyName))
                        {
                            var itemSqlParameter =
                                new SqlParameter(keyName, propertyVal.Value, propertyVal.ParameterType)
                                {
                                    TypeHandler = paramMap?.Handler ?? propertyVal.TypeHandler
                                };
                            context.Parameters.Add(itemSqlParameter);
                        }

                        return $"{dbPrefix}{keyName}";
                    });
                itemIndex++;
                context.SqlBuilder.AppendFormat("{0}", itemSql);
            }
        }

        private bool IsDirectValue(object obj)
        {
            bool isString = obj is String;
            if (isString)
            {
                return true;
            }

            bool isValueType = obj is ValueType;
            if (isValueType)
            {
                return true;
            }

            return false;
        }
    }
}