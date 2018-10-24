using SmartSql.Cache;
using SmartSql.Exceptions;
using SmartSql.Configuration.Tags;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using SmartSql.Configuration.Statements;
using SmartSql.Configuration.Maps;
using System.Data;
using SmartSql.Abstractions;
using SmartSql.Utils;

namespace SmartSql.Configuration
{
    public class StatementFactory
    {
        public Statement Load(XmlElement statementNode, SmartSqlMap smartSqlMap)
        {
            var statement = new Statement
            {
                Id = statementNode.Attributes["Id"].Value,
                SqlTags = new List<ITag> { },
                ReadDb = statementNode.Attributes["ReadDb"]?.Value,
                SmartSqlMap = smartSqlMap,
                CacheId = statementNode.Attributes["Cache"]?.Value,
                ResultMapId = statementNode.Attributes["ResultMap"]?.Value,
                ParameterMapId = statementNode.Attributes["ParameterMap"]?.Value,
                MultipleResultMapId = statementNode.Attributes["MultipleResultMap"]?.Value,
                IncludeDependencies = new List<Include>()
            };
            if (!String.IsNullOrEmpty(statement.ReadDb))
            {
                var readDb = smartSqlMap.SqlMapConfig.Database.ReadDataSources?.FirstOrDefault(m => m.Name == statement.ReadDb);
                if (readDb == null)
                {
                    throw new SmartSqlException($"Statement.Id:{statement.FullSqlId} can not find ReadDb:{statement.ReadDb}!");
                }
            }
            #region Init CacheId & ResultMapId & ParameterMapId & MultipleResultMapId
            if (statement.CacheId?.IndexOf('.') < 0)
            {
                statement.CacheId = $"{smartSqlMap.Scope}.{statement.CacheId}";
            }
            if (statement.ResultMapId?.IndexOf('.') < 0)
            {
                statement.ResultMapId = $"{smartSqlMap.Scope}.{statement.ResultMapId}";
            }
            if (statement.ParameterMapId?.IndexOf('.') < 0)
            {
                statement.ParameterMapId = $"{smartSqlMap.Scope}.{statement.ParameterMapId}";
            }
            if (statement.MultipleResultMapId?.IndexOf('.') < 0)
            {
                statement.MultipleResultMapId = $"{smartSqlMap.Scope}.{statement.MultipleResultMapId}";
            }
            #endregion
            #region Init CommandType & SourceChoice & Transaction
            var cmdTypeStr = statementNode.Attributes["CommandType"]?.Value;
            var sourceChoiceStr = statementNode.Attributes["SourceChoice"]?.Value;
            var transactionStr = statementNode.Attributes["Transaction"]?.Value;

            if (Enum.TryParse<CommandType>(cmdTypeStr, out CommandType cmdType))
            {
                statement.CommandType = cmdType;
            }
            if (Enum.TryParse<DataSourceChoice>(cmdTypeStr, out DataSourceChoice sourceChoice))
            {
                statement.SourceChoice = sourceChoice;
            }
            if (Enum.TryParse<IsolationLevel>(transactionStr, out IsolationLevel isolationLevel))
            {
                statement.Transaction = isolationLevel;
            }
            #endregion

            var tagNodes = statementNode.ChildNodes;
            foreach (XmlNode tagNode in tagNodes)
            {
                var tag = LoadTag(tagNode, statement);
                if (tag != null) { statement.SqlTags.Add(tag); }
            }
            return statement;
        }

        private ITag LoadTag(XmlNode xmlNode, Statement statement)
        {
            ITag tag = null;
            var prepend = xmlNode.Attributes?["Prepend"]?.Value.Trim();
            var property = xmlNode.Attributes?["Property"]?.Value.Trim();
            var compareValue = xmlNode.Attributes?["CompareValue"]?.Value.Trim();
            #region Init Tag
            switch (xmlNode.Name)
            {
                case "#text":
                case "#cdata-section":
                    {
                        var innerText = xmlNode.InnerText;
                        var bodyText = innerText.Replace(statement.SmartSqlMap.SqlMapConfig.Settings.ParameterPrefix, statement.SmartSqlMap.SqlMapConfig.Database.DbProvider.ParameterPrefix);
                        return new SqlText(bodyText
                            , statement.SmartSqlMap.SqlMapConfig.Database.DbProvider.ParameterPrefix)
                        {
                            Statement = statement
                        };
                    }
                case "Include":
                    {
                        var refId = xmlNode.Attributes?["RefId"]?.Value;
                        if (refId.IndexOf('.') < 0)
                        {
                            refId = $"{statement.SmartSqlMap.Scope}.{refId}";
                        }
                        var include_tag = new Include
                        {
                            RefId = refId,
                            Prepend = xmlNode.Attributes?["Prepend"]?.Value
                        };
                        statement.IncludeDependencies.Add(include_tag);
                        tag = include_tag;
                        break;
                    }
                case "IsEmpty":
                    {
                        tag = new IsEmpty
                        {
                            Prepend = prepend,
                            Property = property,
                            ChildTags = new List<ITag>()
                        };
                        break;
                    }

                case "IsEqual":
                    {
                        tag = new IsEqual
                        {
                            Prepend = prepend,
                            Property = property,
                            CompareValue = compareValue,
                            ChildTags = new List<ITag>()
                        };
                        break;
                    }
                case "IsGreaterEqual":
                    {
                        tag = new IsGreaterEqual
                        {
                            Prepend = prepend,
                            Property = property,
                            CompareValue = compareValue,
                            ChildTags = new List<ITag>()
                        };
                        break;
                    }
                case "IsGreaterThan":
                    {
                        tag = new IsGreaterThan
                        {
                            Prepend = prepend,
                            Property = property,
                            CompareValue = compareValue,
                            ChildTags = new List<ITag>()
                        };
                        break;
                    }
                case "IsLessEqual":
                    {
                        tag = new IsLessEqual
                        {
                            Prepend = prepend,
                            Property = property,
                            CompareValue = compareValue,
                            ChildTags = new List<ITag>()
                        };
                        break;
                    }
                case "IsLessThan":
                    {
                        tag = new IsLessThan
                        {
                            Prepend = prepend,
                            Property = property,
                            CompareValue = compareValue,
                            ChildTags = new List<ITag>()
                        };
                        break;
                    }
                case "IsNotEmpty":
                    {
                        tag = new IsNotEmpty
                        {
                            Prepend = prepend,
                            Property = property,
                            ChildTags = new List<ITag>()
                        };
                        break;
                    }
                case "IsNotEqual":
                    {
                        tag = new IsNotEqual
                        {
                            Prepend = prepend,
                            Property = property,
                            CompareValue = compareValue,
                            ChildTags = new List<ITag>()
                        };
                        break;
                    }
                case "IsNotNull":
                    {
                        tag = new IsNotNull
                        {
                            Prepend = prepend,
                            Property = property,
                            ChildTags = new List<ITag>()
                        };
                        break;
                    }
                case "IsNull":
                    {
                        tag = new IsNull
                        {
                            Prepend = prepend,
                            Property = property,
                            ChildTags = new List<ITag>()
                        };
                        break;
                    }
                case "IsTrue":
                    {
                        tag = new IsTrue
                        {
                            Prepend = prepend,
                            Property = property,
                            ChildTags = new List<ITag>()
                        };
                        break;
                    }
                case "IsFalse":
                    {
                        tag = new IsFalse
                        {
                            Prepend = prepend,
                            Property = property,
                            ChildTags = new List<ITag>()
                        };
                        break;
                    }
                case "IsProperty":
                    {
                        tag = new IsProperty
                        {
                            Prepend = prepend,
                            Property = property,
                            ChildTags = new List<ITag>()
                        };
                        break;
                    }
                case "Placeholder":
                    {
                        tag = new Placeholder
                        {
                            Prepend = xmlNode.Attributes?["Prepend"]?.Value,
                            Property = property,
                            ChildTags = new List<ITag>()
                        };
                        break;
                    }
                case "Switch":
                    {
                        tag = new Switch
                        {
                            Property = property,
                            Prepend = prepend,
                            ChildTags = new List<ITag>()
                        };
                        break;
                    }
                case "Case":
                    {
                        var switchNode = xmlNode.ParentNode;
                        var switchProperty = switchNode.Attributes?["Property"]?.Value.Trim();
                        var switchPrepend = switchNode.Attributes?["Prepend"]?.Value.Trim();
                        tag = new Switch.Case
                        {
                            CompareValue = compareValue,
                            Property = switchProperty,
                            Prepend = switchPrepend,
                            ChildTags = new List<ITag>()
                        };
                        break;
                    }
                case "Default":
                    {
                        var switchNode = xmlNode.ParentNode;
                        var switchProperty = switchNode.Attributes?["Property"]?.Value.Trim();
                        var switchPrepend = switchNode.Attributes?["Prepend"]?.Value.Trim();
                        tag = new Switch.Defalut
                        {
                            Property = switchProperty,
                            Prepend = switchPrepend,
                            ChildTags = new List<ITag>()
                        };
                        break;
                    }
                case "Dynamic":
                    {
                        tag = new Dynamic
                        {
                            Prepend = prepend,
                            ChildTags = new List<ITag>()
                        };
                        break;
                    }
                case "Where":
                    {
                        tag = new Where
                        {
                            ChildTags = new List<ITag>()
                        };
                        break;
                    }
                case "Set":
                    {
                        tag = new Set
                        {
                            ChildTags = new List<ITag>()
                        };
                        break;
                    }
                case "For":
                    {
                        var open = xmlNode.Attributes?["Open"]?.Value.Trim();
                        var separator = xmlNode.Attributes?["Separator"]?.Value.Trim();
                        var close = xmlNode.Attributes?["Close"]?.Value.Trim();
                        var key = xmlNode.Attributes?["Key"]?.Value.Trim();
                        tag = new For
                        {
                            Prepend = prepend,
                            Property = property,
                            Open = open,
                            Close = close,
                            Separator = separator,
                            Key = key,
                            ChildTags = new List<ITag>()
                        };
                        break;
                    }
                case "Env":
                    {
                        var dbProvider = xmlNode.Attributes?["DbProvider"]?.Value.Trim();
                        tag = new Env
                        {
                            Prepend = prepend,
                            DbProvider = dbProvider,
                            ChildTags = new List<ITag>()
                        };
                        break;
                    }
                case "#comment": { return null; }
                default:
                    {
                        throw new SmartSqlException($"Statement.LoadTag unkonw tagName:{xmlNode.Name}.");
                    };
            }
            tag.Statement = statement;
            #endregion
            foreach (XmlNode childNode in xmlNode)
            {
                ITag childTag = LoadTag(childNode, statement);
                if (childTag != null && tag != null)
                {
                    childTag.Parent = tag;
                    (tag as Tag).ChildTags.Add(childTag);
                }
            }
            return tag;
        }
    }
}
