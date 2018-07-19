using SmartSql.Configuration.Statements;
using SmartSql.Configuration.Tags;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using SmartSql.Abstractions;

namespace SmartSql.DyRepository
{
    public enum SqlCommandType
    {
        Unknown = 0,
        Insert = 1 << 0,
        Update = 1 << 1,
        Delete = 1 << 2,
        Select = 1 << 3
    }
    public class SqlCommandAnalyzer
    {
        const string DEFAULT_STRING_VALUE = "SmartSql";
        public string BuildStatementFullSql(ISmartSqlMapper smartSqlMapper, string scope, string id)
        {
            var statement = smartSqlMapper.SmartSqlOptions.SmartSqlContext.GetStatement($"{scope}.{id}");
            var reqParams = BuildFullParams(statement);
            var reqContext = new RequestContext
            {
                Scope = scope,
                SqlId = id,
                Request = reqParams
            };
            reqContext.Setup(smartSqlMapper.SmartSqlOptions.SmartSqlContext);
            return smartSqlMapper.SmartSqlOptions.SqlBuilder.BuildSql(reqContext);
        }

        private Dictionary<string, object> BuildFullParams(Statement statement)
        {
            Dictionary<string, object> reqParams = new Dictionary<string, object>();
            foreach (var tag in statement.SqlTags)
            {
                BuildRequestParams(tag, reqParams);
            }
            return reqParams;
        }

        private void AddParamsIfNo(Dictionary<string, object> reqParams, string key, object val)
        {
            if (!reqParams.ContainsKey(key))
            {
                reqParams.Add(key, val);
            }
        }

        private void BuildRequestParams(ITag tag, Dictionary<string, object> reqParams)
        {
            if (!(tag is Tag fTag))
            {
                return;
            }
            switch (tag.Type)
            {
                case TagType.IsEmpty:
                case TagType.IsNull:
                    {
                        AddParamsIfNo(reqParams, fTag.Property, null); break;
                    }

                case TagType.IsProperty:
                case TagType.IsNotNull:
                case TagType.IsNotEmpty: { AddParamsIfNo(reqParams, fTag.Property, DEFAULT_STRING_VALUE); break; }

                case TagType.IsTrue: { AddParamsIfNo(reqParams, fTag.Property, true); break; }
                case TagType.IsFalse: { AddParamsIfNo(reqParams, fTag.Property, false); break; }

                case TagType.For:
                    {
                        var vals = new[] { 1 };
                        AddParamsIfNo(reqParams, fTag.Property, vals);
                        break;
                    }

                case TagType.IsEqual:
                case TagType.IsLessEqual:
                case TagType.IsGreaterEqual:
                    {
                        var compareTag = tag as CompareTag;
                        AddParamsIfNo(reqParams, compareTag.Property, compareTag.CompareValue);
                        break;
                    }

                case TagType.IsNotEqual:
                    {
                        var compareTag = tag as CompareTag;
                        AddParamsIfNo(reqParams, compareTag.Property, compareTag.CompareValue + DEFAULT_STRING_VALUE);
                        break;
                    }
                case TagType.IsGreaterThan:
                    {
                        var compareTag = tag as CompareTag;
                        if (Decimal.TryParse(compareTag.CompareValue, out decimal reqValNum))
                        {
                            reqValNum++;
                            AddParamsIfNo(reqParams, compareTag.Property, reqValNum);
                        }
                        break;
                    }
                case TagType.IsLessThan:
                    {
                        var compareTag = tag as CompareTag;
                        if (Decimal.TryParse(compareTag.CompareValue, out decimal reqValNum))
                        {
                            reqValNum--;
                            AddParamsIfNo(reqParams, compareTag.Property, reqValNum);
                        }
                        break;
                    }

                case TagType.Switch:
                    {
                        var switchTag = tag as Switch;
                        var cTag = switchTag.ChildTags.First();
                        if (!(cTag is Switch.Defalut))
                        {
                            var caseTag = cTag as Switch.Case;
                            AddParamsIfNo(reqParams, switchTag.Property, caseTag.CompareValue);
                        }
                        break;
                    }
            }
            if (fTag.ChildTags != null)
            {
                foreach (var childTag in fTag.ChildTags)
                {
                    BuildRequestParams(childTag, reqParams);
                }
            }
        }

        internal SqlCommandType Analyse(string realSql)
        {
            SqlCommandType commandType = SqlCommandType.Unknown;
            var statements = realSql.Split(';');
            foreach (var statement in statements)
            {
                var statementStr = statement.TrimStart(' ', '(');
                if (statementStr.StartsWith("Insert", StringComparison.CurrentCultureIgnoreCase))
                {
                    commandType = commandType | SqlCommandType.Insert;
                }
                if (statementStr.StartsWith("Delete", StringComparison.CurrentCultureIgnoreCase))
                {
                    commandType = commandType | SqlCommandType.Delete;
                }
                if (statementStr.StartsWith("Update", StringComparison.CurrentCultureIgnoreCase))
                {
                    commandType = commandType | SqlCommandType.Update;
                }
                if (statementStr.StartsWith("Select", StringComparison.CurrentCultureIgnoreCase))
                {
                    commandType = commandType | SqlCommandType.Select;
                }
            }
            return commandType;
        }
        internal SqlCommandType Analyse(ISmartSqlMapper smartSqlMapper, string scope, string id)
        {
            string realSql = BuildStatementFullSql(smartSqlMapper, scope, id);
            return Analyse(realSql);
        }
    }
}
