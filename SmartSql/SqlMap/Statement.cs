using SmartSql.Abstractions;
using SmartSql.Abstractions.Cache;
using SmartSql.Cache;
using SmartSql.Cache.Fifo;
using SmartSql.Cache.None;
using SmartSql.Exceptions;
using SmartSql.SqlMap.Tags;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace SmartSql.SqlMap
{
    public class Statement
    {
        [XmlIgnore]
        public SmartSqlMap SmartSqlMap { get; private set; }
        public static Statement Load(XmlElement statementNode, SmartSqlMap smartSqlMap)
        {
            var statement = new Statement
            {
                Id = statementNode.Attributes["Id"].Value,

                SqlTags = new List<ITag> { },
                SmartSqlMap = smartSqlMap
            };
            string cacheId = statementNode.Attributes["Cache"]?.Value;
            if (!String.IsNullOrEmpty(cacheId))
            {
                var cache = smartSqlMap.Caches.FirstOrDefault(m => m.Id == cacheId);
                statement.Cache = cache ?? throw new SmartSqlException($"SmartSql.Statement.Id:{statement.Id} can not find Cache.Id:{cacheId}");
            }
            var tagNodes = statementNode.ChildNodes;
            foreach (XmlNode tagNode in tagNodes)
            {
                var prepend = tagNode.Attributes?["Prepend"]?.Value;
                var property = tagNode.Attributes?["Property"]?.Value;

                #region Init Tag
                switch (tagNode.Name)
                {
                    case "Include":
                        {
                            var refId = tagNode.Attributes?["RefId"]?.Value;

                            var refStatement = smartSqlMap.Statements.FirstOrDefault(m => m.Id == refId);
                            if (refStatement == null)
                            {
                                throw new SmartSqlException($"SmartSql.Statement.Load can not find statement.id:{refId}");
                            }
                            if (refId == statement.Id)
                            {
                                throw new SmartSqlException($"SmartSql.Statement.Load Include.RefId can not be self statement.id:{refId}");
                            }
                            statement.SqlTags.Add(new Include
                            {
                                RefId = refId,
                                Ref = refStatement
                            });
                            break;
                        }
                    case "Switch":
                        {
                            var switchTag = new Switch
                            {
                                Property = property,
                                Prepend = prepend,
                                Cases = new List<Switch.Case>()
                            };
                            var caseNodes = tagNode.ChildNodes;
                            foreach (XmlNode caseNode in caseNodes)
                            {
                                var caseCompareValue = caseNode.Attributes?["CompareValue"]?.Value;
                                var caseBodyText = caseNode.InnerText.Replace("\n", "");
                                switchTag.Cases.Add(new Switch.Case
                                {
                                    CompareValue = caseCompareValue,
                                    BodyText = caseBodyText
                                });
                            }
                            statement.SqlTags.Add(switchTag);
                            break;
                        }
                    default:
                        {
                            var tag = LoadTag(tagNode);
                            if (tag != null) { statement.SqlTags.Add(tag); }
                            break;
                        };
                }
                #endregion
            }
            return statement;
        }

        public static ITag LoadTag(XmlNode xmlNode)
        {
            ITag tag = null;
            bool isIn = xmlNode.Attributes?["In"] != null;
            var prepend = xmlNode.Attributes?["Prepend"]?.Value;
            var property = xmlNode.Attributes?["Property"]?.Value;
            var compareValue = xmlNode.Attributes?["CompareValue"]?.Value;
            #region Init Tag
            switch (xmlNode.Name)
            {
                case "#text":
                case "#cdata-section":
                    {
                        var bodyText = xmlNode.InnerText.Replace("\n", "");
                        return new SqlText
                        {
                            BodyText = bodyText
                        };
                    }
                case "IsEmpty":
                    {
                        tag = new IsEmpty
                        {
                            In = isIn,
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
                            In = isIn,
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
                            In = isIn,
                            Prepend = prepend,
                            Property = property,
                            CompareValue = compareValue,
                        };
                        break;
                    }
                case "IsGreaterThan":
                    {
                        tag = new IsGreaterThan
                        {
                            In = isIn,
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
                            In = isIn,
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
                            In = isIn,
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
                            In = isIn,
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
                            In = isIn,
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
                            In = isIn,
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
                            In = isIn,
                            Prepend = prepend,
                            Property = property,
                            ChildTags = new List<ITag>()
                        };
                        break;
                    }
                default: { return null; };
            }
            #endregion
            foreach (XmlNode childNode in xmlNode)
            {
                ITag childTag = LoadTag(childNode);
                (tag as Tag).ChildTags.Add(childTag);
            }
            return tag;
        }

        [XmlAttribute]
        public String Id { get; set; }

        public String FullSqlId => $"{SmartSqlMap.Scope}.{Id}";
        public List<ITag> SqlTags { get; set; }
        public Cache Cache { get; set; }
        private ICacheProvider _cacheProvider;
        public ICacheProvider CacheProvider
        {
            get
            {
                #region Init CacheProvider
                if (_cacheProvider == null)
                {
                    lock (this)
                    {
                        if (_cacheProvider == null)
                        {
                            if (Cache == null)
                            {
                                _cacheProvider = new NoneCacheProvider();
                            }
                            else
                            {
                                _cacheProvider = Cache.CreateCacheProvider(this);
                            }
                        }
                    }
                }
                #endregion
                return _cacheProvider;
            }
        }
        public String BuildSql(RequestContext context)
        {
            String prefix = SmartSqlMap.SmartSqlMapConfig.Database.DbProvider.ParameterPrefix;
            StringBuilder sqlStrBuilder = new StringBuilder();
            foreach (ITag tag in SqlTags)
            {
                sqlStrBuilder.Append(tag.BuildSql(context, prefix));
            }
            return sqlStrBuilder.ToString();
        }
    }
}
