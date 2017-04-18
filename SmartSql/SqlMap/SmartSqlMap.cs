using SmartSql.Abstractions;
using SmartSql.Abstractions.Logging;
using System.Linq;
using SmartSql.Common;
using SmartSql.SqlMap.Tags;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using SmartSql.Exceptions;

namespace SmartSql.SqlMap
{
    [XmlRoot(Namespace = "http://SmartSql.net/schemas/SmartSqlMap.xsd")]
    public class SmartSqlMap
    {
        [XmlIgnore]
        public SmartSqlMapConfig SmartSqlMapConfig { get; private set; }
        [XmlIgnore]
        public String FilePath { get; private set; }
        [XmlAttribute]
        public String Scope { get; set; }
        [XmlArray]
        public List<Statement> Statements { get; set; }
        public static SmartSqlMap Load(String filePath, SmartSqlMapConfig smartSqlMapConfig)
        {
            var sqlMap = new SmartSqlMap
            {
                SmartSqlMapConfig = smartSqlMapConfig,
                FilePath = filePath,
                Statements = new List<Statement> { }
            };
            using (var xmlFile = FileLoader.Load(filePath))
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(xmlFile);
                XmlNamespaceManager xmlNsM = new XmlNamespaceManager(xmlDoc.NameTable);
                xmlNsM.AddNamespace("ns", "http://SmartSql.net/schemas/SmartSqlMap.xsd");
                sqlMap.Scope = xmlDoc.SelectSingleNode("//ns:SmartSqlMap", xmlNsM)
                    .Attributes["Scope"].Value;
                var statementNodes = xmlDoc.SelectNodes("//ns:Statement", xmlNsM);
                foreach (XmlElement statementNode in statementNodes)
                {
                    var statement = Statement.Load(statementNode, sqlMap);
                    sqlMap.Statements.Add(statement);
                }
                return sqlMap;
            }

        }
    }

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
        public List<ITag> SqlTags { get; set; }
        public String BuildSql(object paramObj)
        {
            String prefix = SmartSqlMap.SmartSqlMapConfig.Database.DbProvider.ParameterPrefix;
            StringBuilder sqlStrBuilder = new StringBuilder();
            foreach (ITag tag in SqlTags)
            {
                sqlStrBuilder.Append(tag.BuildSql(paramObj, prefix));
            }
            return sqlStrBuilder.ToString();
        }
    }




}
