using SmartSql.Abstractions;
using SmartSql.Abstractions.Cache;
using SmartSql.Cache;
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
                statement.Cache = cache ?? throw new SmartSqlException($"Statement.Id:{statement.Id} can not find Cache.Id:{cacheId}");
            }
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

        public static ITag LoadTag(XmlNode xmlNode, IList<Include> includes)
        {
            ITag tag = null;
            bool isIn = xmlNode.Attributes?["In"] != null;
            var prepend = xmlNode.Attributes?["Prepend"]?.Value.Trim();
            var property = xmlNode.Attributes?["Property"]?.Value.Trim();
            var compareValue = xmlNode.Attributes?["CompareValue"]?.Value.Trim();
            #region Init Tag
            switch (xmlNode.Name)
            {
                case "#text":
                case "#cdata-section":
                    {
                        var bodyText = " " + xmlNode.InnerText.Replace("\n", "").Trim();
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
                            Prepend = prepend
                        };
                        includes.Add(include_tag);
                        tag = include_tag;
                        break;
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
                            ChildTags = new List<ITag>()
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
                case "IsTrue":
                    {
                        tag = new IsTrue
                        {
                            In = isIn,
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
                            In = isIn,
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
                            In = isIn,
                            Prepend = prepend,
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
                            //Prepend = prepend,
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
                case "Where":
                    {
                        tag = new Where
                        {
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
                    (tag as Tag).ChildTags.Add(childTag);
                }
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
            context.SmartSqlMap = SmartSqlMap;
            string smartPrefix = SmartSqlMap.SmartSqlMapConfig.Settings.ParameterPrefix;
            String dbPrefix = SmartSqlMap.SmartSqlMapConfig.Database.DbProvider.ParameterPrefix;
            StringBuilder sqlStrBuilder = new StringBuilder();
            foreach (ITag tag in SqlTags)
            {
                sqlStrBuilder.Append(tag.BuildSql(context));
            }
            return sqlStrBuilder.Replace(smartPrefix, dbPrefix).ToString();
        }
    }
}
