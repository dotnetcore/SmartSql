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
                SmartSqlMap = smartSqlMap
            };
            var cmdTypeStr = statementNode.Attributes["CommandType"]?.Value;
            var sourceChoiceStr = statementNode.Attributes["SourceChoice"]?.Value;
            if (Enum.TryParse<CommandType>(cmdTypeStr, out CommandType cmdType))
            {
                statement.CommandType = cmdType;
            }
            if (Enum.TryParse<DataSourceChoice>(cmdTypeStr, out DataSourceChoice sourceChoice))
            {
                statement.SourceChoice = sourceChoice;
            }
            string cacheId = statementNode.Attributes["Cache"]?.Value;
            #region Attribute For Cache & ResultMap & ParameterMap
            if (!String.IsNullOrEmpty(cacheId))
            {
                var cache = smartSqlMap.Caches.FirstOrDefault(m => m.Id == cacheId);
                statement.Cache = cache ?? throw new SmartSqlException($"Statement.Id:{statement.Id} can not find Cache.Id:{cacheId}");
            }

            string resultMapId = statementNode.Attributes["ResultMap"]?.Value;
            if (!String.IsNullOrEmpty(resultMapId))
            {
                var resultMap = smartSqlMap.ResultMaps.FirstOrDefault(r => r.Id == resultMapId);
                statement.ResultMap = resultMap ?? throw new SmartSqlException($"Statement.Id:{statement.Id} can not find ResultMap.Id:{resultMapId}");
            }

            string parameterMapId = statementNode.Attributes["ParameterMap"]?.Value;
            if (!String.IsNullOrEmpty(parameterMapId))
            {
                var parameterMap = smartSqlMap.ParameterMaps.FirstOrDefault(r => r.Id == parameterMapId);
                statement.ParameterMap = parameterMap ?? throw new SmartSqlException($"Statement.Id:{statement.Id} can not find ParameterMap.Id:{parameterMapId}");
            }
            #endregion
            var tagNodes = statementNode.ChildNodes;
            IList<Include> includes = new List<Include>();
            foreach (XmlNode tagNode in tagNodes)
            {
                var tag = LoadTag(tagNode, includes);
                if (tag != null) { statement.SqlTags.Add(tag); }
            }
            #region Init Include
            foreach (var include in includes)
            {
                if (include.RefId == statement.Id)
                {
                    throw new SmartSqlException($"Statement.Load Include.RefId can not be self statement.id:{include.RefId}");
                }
                var refStatement = smartSqlMap.Statements.FirstOrDefault(m => m.Id == include.RefId);

                include.Ref = refStatement ?? throw new SmartSqlException($"Statement.Load can not find statement.id:{include.RefId}");
            }
            #endregion
            return statement;
        }

        private ITag LoadTag(XmlNode xmlNode, IList<Include> includes)
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
                        var bodyText = String.Empty;
                        var innerText = xmlNode.InnerText;
                        if (innerText.StartsWith(" "))
                        {
                            bodyText += " ";
                        }
                        bodyText += innerText.Trim().Replace("\r\n", " ");
                        if (innerText.EndsWith(" "))
                        {
                            bodyText += " ";
                        }
                        return new SqlText
                        {
                            BodyText = bodyText
                        };
                    }
                case "Include":
                    {
                        var refId = xmlNode.Attributes?["RefId"]?.Value;
                        var include_tag = new Include
                        {
                            RefId = refId,
                            Prepend = xmlNode.Attributes?["Prepend"]?.Value
                        };
                        includes.Add(include_tag);
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
                case "#comment": { break; }
                default:
                    {
                        throw new SmartSqlException($"Statement.LoadTag unkonw tagName:{xmlNode.Name}.");
                    };
            }
            #endregion
            foreach (XmlNode childNode in xmlNode)
            {
                ITag childTag = LoadTag(childNode, includes);
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
